using EngConnect.Entities.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.UserContext
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserContextService(
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

        public string? GetCurrentUserId() =>
            _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public async Task<ApplicationUser?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
        {
            string? id = GetCurrentUserId();
            if (string.IsNullOrEmpty(id))
                return null;

            return await _userManager.FindByIdAsync(id);
        }

        public async Task<IList<string>> GetCurrentUserRolesAsync(CancellationToken cancellationToken = default)
        {
            ApplicationUser? user = await GetCurrentUserAsync(cancellationToken);
            if (user == null)
                return Array.Empty<string>();

            return await _userManager.GetRolesAsync(user);
        }
    }
}