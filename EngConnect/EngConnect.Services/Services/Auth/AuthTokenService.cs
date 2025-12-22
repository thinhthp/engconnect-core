using EngConnect.Entities.Entities;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Auth
{
    public class AuthTokenService : IAuthTokenService
    {
        private readonly EngConnectContext _dbContext;

        public AuthTokenService(EngConnectContext dbContext)
        {
            _dbContext = dbContext;
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
    }
}