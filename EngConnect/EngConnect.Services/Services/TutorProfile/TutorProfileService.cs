using EngConnect.Entities.Common;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Repositories.TutorProfile.Filters;
using EngConnect.Services.Caching.TutorProfile;
using EngConnect.Services.DTOs.TutorProfile;
using EngConnect.Services.Services.TutorProfile.Filters;
using EngConnect.Services.Services.UserContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.TutorProfile
{
    public class TutorProfileService : ITutorProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ITutorProfileCache _tutorProfileCache;

        public TutorProfileService(IUnitOfWork unitOfWork, IUserContextService userContext, ITutorProfileCache tutorProfileCache)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _tutorProfileCache = tutorProfileCache;
        }

        public async Task<TutorProfileDTO?> GetCurrentAsync(CancellationToken cancellationToken = default)
        {
            string? userId = _userContext.GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId))
                return null;

            // try cache
            var cached = await _tutorProfileCache.GetAsync(userId, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            // fallback to DB
            var profile = await _unitOfWork.TutorProfileRepository.GetByTutorIdAsync(userId, cancellationToken);
            if (profile == null)
                return null;

            var dto = Map(profile);

            // populate cache
            await _tutorProfileCache.SetAsync(dto, cancellationToken);

            return dto;
        }

        public async Task<TutorProfileDTO> CreateForCurrentUserAsync(CreateTutorProfileRequest request, CancellationToken cancellationToken = default)
        {
            string? userId = _userContext.GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("User is not authenticated.");

            bool exists = await _unitOfWork.TutorProfileRepository.ExistsAsync(userId, cancellationToken);
            if (exists)
                throw new InvalidOperationException("Tutor profile already exists for this user.");

            var profile = new Entities.Entities.TutorProfile
            {
                TutorId = userId,
                Nickname = request.Nickname,
                ProfilePictureUrl = request.ProfilePictureUrl,
                ExperienceYears = request.ExperienceYears,
                Bio = request.Bio,
                Language = request.Language,
                CvFile = request.CvFile,
                DemoVideo = request.DemoVideo,
                Approved = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreateBy = userId
            };

            await _unitOfWork.TutorProfileRepository.AddAsync(profile, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            var dto = Map(profile);

            // cache
            await _tutorProfileCache.SetAsync(dto, cancellationToken);

            return dto;
        }

        private static TutorProfileDTO Map(Entities.Entities.TutorProfile p) => new()
        {
            TutorId = p.TutorId!,
            ExperienceYears = p.ExperienceYears,
            Bio = p.Bio,
            Language = p.Language,
            CvFile = p.CvFile,
            Nickname = p.Nickname,
            ProfilePictureUrl = p.ProfilePictureUrl,
            DemoVideo = p.DemoVideo,
            Approved = p.Approved,
            IsActive = p.IsActive,
            Note = p.Note,
            CreatedAt = p.CreatedAt,
            UpdateDate = p.UpdateDate
        };

        public async Task<PagedResult<TutorProfileDTO>> GetForAdminAsync(TutorProfileSearchFilter filter, CancellationToken cancellationToken = default)
        {
            var repoParams = new TutorProfileQueryParameters
            {
                Search = filter.Search,
                Approved = filter.Approved,
                IsActive = filter.IsActive,
                Language = filter.Language,
                TutorId = filter.TutorId,
                SortBy = filter.SortBy,
                SortDir = filter.SortDir,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            var (items, total) = await _unitOfWork.TutorProfileRepository.QueryAsync(repoParams, cancellationToken);
            var dto = items.Select(Map).ToList();

            return new PagedResult<TutorProfileDTO>
            {
                Items = dto,
                TotalCount = total,
                PageNumber = repoParams.PageNumber,
                PageSize = repoParams.PageSize
            };
        }

        public async Task<TutorProfileDTO> UpdateForCurrentUserAsync(UpdateTutorProfileRequest request, CancellationToken cancellationToken = default)
        {
            string? userId = _userContext.GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("User is not authenticated.");

            var profile = await _unitOfWork.TutorProfileRepository.GetByTutorIdAsync(userId, cancellationToken);
            if (profile == null)
                throw new KeyNotFoundException("Tutor profile not found.");

            // only overwrite provided fields
            if (request.Nickname is not null) profile.Nickname = request.Nickname;
            if (request.ProfilePictureUrl is not null) profile.ProfilePictureUrl = request.ProfilePictureUrl;
            if (request.ExperienceYears.HasValue) profile.ExperienceYears = request.ExperienceYears;
            if (request.Bio is not null) profile.Bio = request.Bio;
            if (request.Language is not null) profile.Language = request.Language;
            if (request.CvFile is not null) profile.CvFile = request.CvFile;
            if (request.DemoVideo is not null) profile.DemoVideo = request.DemoVideo;

            profile.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.TutorProfileRepository.UpdateAsync(profile, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            var dto = Map(profile);

            // refresh cache
            await _tutorProfileCache.SetAsync(dto, cancellationToken);

            return dto;
        }
    }
}
