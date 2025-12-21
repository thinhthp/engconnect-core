using EngConnect.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Auth
{
    public interface IAuthTokenService
    {
        Task<string> CreateRefreshTokenAsync(ApplicationUser user, string jwtId, DateTime accessTokenExpires, CancellationToken cancellationToken = default);

        Task<RefreshToken?> GetRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken = default);

        Task MarkRefreshTokenUsedAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);

        Task RevokeAllUserRefreshTokensAsync(string userId, CancellationToken cancellationToken = default);
    }
}