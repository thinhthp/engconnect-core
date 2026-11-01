using EngConnect.Entities.Common;
using EngConnect.Entities.Entities;
using EngConnect.Repositories.Repositories.TutorProfile;
using EngConnect.Repositories.Repositories.TutorProfile.Filters;
using EngConnect.Services.DTOs.Account;
using EngConnect.Services.Services.TutorProfile.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EngConnect.Services.Services.Tutors
{
    public class TutorService : ITutorService
    {
        private readonly ITutorProfileRepository _tutorProfileRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public TutorService(ITutorProfileRepository tutorProfileRepository, UserManager<ApplicationUser> userManager)
        {
            _tutorProfileRepository = tutorProfileRepository;
            _userManager = userManager;
        }

        public async Task<PagedResult<UserResponse>> GetTutorsAsync(TutorProfileSearchFilter filter, CancellationToken cancellationToken = default)
        {
            var repoParams = new TutorProfileQueryParameters
            {
                Search = filter.Search,
                Approved = true,

                Language = filter.Language,
                TutorId = filter.TutorId,
                SortBy = filter.SortBy,
                SortDir = filter.SortDir,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            var (profiles, total) = await _tutorProfileRepository.QueryAsync(repoParams, cancellationToken);

            var tutorIds = profiles.Select(p => p.TutorId!).Where(id => !string.IsNullOrWhiteSpace(id)).ToList();

            var users = await _userManager.Users
                .Where(u => tutorIds.Contains(u.Id) && u.EmailConfirmed)
                .ToListAsync(cancellationToken);

            var items = users.Select(u => new UserResponse
            {
                Id = u.Id,
                Name = u.UserName,
                Email = u.Email,
                Phone = u.PhoneNumber,
                IsActive = u.IsActive,
                CreateDate = u.CreatedAt,
                UpdateDate = u.UpdateDate,
                CreateBy = u.CreateBy,
                UpdateBy = u.UpdateBy
            }).ToList();

            return new PagedResult<UserResponse>
            {
                Items = items,
                TotalCount = total,
                PageNumber = repoParams.PageNumber,
                PageSize = repoParams.PageSize
            };
        }
    }
}
