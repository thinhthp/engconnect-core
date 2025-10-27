using EngConnect.Entities.Common;
using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Repositories.TutorProfile.Filters;
using EngConnect.Services.DTOs.Account;
using EngConnect.Services.Services.TutorProfile.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EngConnect.Services.DTOs.TutorProfile;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Tutors
{
    public class TutorService : ITutorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public TutorService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
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

          
            var (profiles, total) = await _unitOfWork.TutorProfileRepository.QueryAsync(repoParams, cancellationToken);

          
            var tutorIds = profiles.Select(p => p.TutorId!).Where(id => !string.IsNullOrWhiteSpace(id)).ToList();

           
            var users = await _userManager.Users
                .Where(u => tutorIds.Contains(u.Id) && u.EmailConfirmed )
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
