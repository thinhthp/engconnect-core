using EngConnect.Repositories.Common;
using EngConnect.Services.DTOs.TutorProfile;
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

        public TutorProfileService(IUnitOfWork unitOfWork, IUserContextService userContext)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
        }

        public async Task<TutorProfileDTO?> GetCurrentAsync(CancellationToken cancellationToken = default)
        {
            string? userId = _userContext.GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId))
                return null;

            var profile = await _unitOfWork.TutorProfileRepository.GetByTutorIdAsync(userId, cancellationToken);
            return profile == null ? null : Map(profile);
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

            return Map(profile);
        }

        private static TutorProfileDTO Map(Entities.Entities.TutorProfile p) => new()
        {
            TutorId = p.TutorId!,
            ExperienceYears = p.ExperienceYears,
            Bio = p.Bio,
            Language = p.Language,
            CvFile = p.CvFile,
            DemoVideo = p.DemoVideo,
            Approved = p.Approved,
            IsActive = p.IsActive,
            Note = p.Note,
            CreatedAt = p.CreatedAt,
            UpdateDate = p.UpdateDate
        };
    }
}
