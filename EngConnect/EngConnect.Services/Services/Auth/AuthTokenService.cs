using EngConnect.Entities.Entities;
using EngConnect.Repositories.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Auth
{
    public class AuthTokenService : IAuthTokenService
    {
        private readonly EngConnectContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthTokenService(EngConnectContext dbContext, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> CreateRefreshTokenAsync(ApplicationUser user, string jwtId, DateTime accessTokenExpires, CancellationToken cancellationToken = default)
        {
            var expiry = DateTime.UtcNow.AddDays(7);

            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            string refreshTokenString = Convert.ToBase64String(randomBytes);

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenString,
                JwtId = jwtId,
                UserId = user.Id,
                ExpiresAt = expiry,
                IsRevoked = false,
                IsUsed = false
            };

            _dbContext.RefreshTokens.Add(refreshToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return refreshTokenString;
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken = default)
        {
            return await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.UserId == userId && rt.Token == refreshToken, cancellationToken);
        }

        public async Task MarkRefreshTokenUsedAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
        {
            refreshToken.IsUsed = true;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task RevokeAllUserRefreshTokensAsync(string userId, CancellationToken cancellationToken = default)
        {
            var tokens = await _dbContext.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked && !rt.IsUsed)
                .ToListAsync(cancellationToken);

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<(string Token, string JwtId, DateTime Expires)> GenerateToken(ApplicationUser user)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);
            var nowUtc = DateTime.UtcNow;
            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r));
            byte[] key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!);

            var jwtId = Guid.NewGuid().ToString("N");
            var expires = nowUtc.AddHours(1);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, jwtId)
                }.Concat(roleClaims)),
                IssuedAt = nowUtc,
                NotBefore = nowUtc,
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var handler = new JwtSecurityTokenHandler();
            SecurityToken token = handler.CreateToken(tokenDescriptor);
            string tokenString = handler.WriteToken(token);

            return (tokenString, jwtId, expires);
        }
    }
}