using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Services.Services.UserContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Admin
{
    using Microsoft.AspNetCore.Identity;
    using EngConnect.Entities.Entities;

    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly UserManager<ApplicationUser> _userManager;

        private const string StatusApproved = "approved";
        private const string StatusRejected = "rejected";
        private const string TutorRejectedNote = "Rejected by admin, please check your mail for detail.";

        public AdminService(IUnitOfWork unitOfWork, IUserContextService userContext, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _userManager = userManager;
        }

        public async Task<bool> ApproveCourseAsync(int courseId, CancellationToken cancellationToken = default)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId, cancellationToken);
            if (course == null)
                throw new KeyNotFoundException("Course not found.");

            // No-op if already approved
            if (string.Equals(course.Status, StatusApproved, StringComparison.OrdinalIgnoreCase))
                return true;

            course.Status = StatusApproved;

            var adminUser = await _userContext.GetCurrentUserAsync(cancellationToken);
            if (adminUser != null && !string.IsNullOrWhiteSpace(adminUser.UserName))
                course.UpdateBy = adminUser.UserName;

            var affected = await _unitOfWork.SaveChangesAsync();
            return affected > 0;
        }

        public async Task<bool> RejectCourseAsync(int courseId, CancellationToken cancellationToken = default)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId, cancellationToken);
            if (course == null)
                throw new KeyNotFoundException("Course not found.");

            // No-op if already rejected
            if (string.Equals(course.Status, StatusRejected, StringComparison.OrdinalIgnoreCase))
                return true;

            course.Status = StatusRejected;
            course.Note = TutorRejectedNote;

            var adminUser = await _userContext.GetCurrentUserAsync(cancellationToken);
            if (adminUser != null && !string.IsNullOrWhiteSpace(adminUser.UserName))
                course.UpdateBy = adminUser.UserName;

            var affected = await _unitOfWork.SaveChangesAsync();
            return affected > 0;
        }

        public async Task<bool> ApproveTutorAsync(string tutorId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(tutorId))
                throw new ArgumentException("TutorId is required.", nameof(tutorId));

            var tutor = await _unitOfWork.TutorProfileRepository.GetByTutorIdAsync(tutorId, cancellationToken);
            if (tutor == null)
                throw new KeyNotFoundException("Tutor not found.");

            // No-op if already approved
            if (tutor.Approved)
                return true;

            tutor.Approved = true;

            var adminUser = await _userContext.GetCurrentUserAsync(cancellationToken);
            if (adminUser != null && !string.IsNullOrWhiteSpace(adminUser.UserName))
                tutor.UpdateBy = adminUser.UserName;

            var affected = await _unitOfWork.SaveChangesAsync();
            return affected > 0;
        }

        public async Task<bool> RejectTutorAsync(string tutorId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(tutorId))
                throw new ArgumentException("TutorId is required.", nameof(tutorId));

            var tutor = await _unitOfWork.TutorProfileRepository.GetByTutorIdAsync(tutorId, cancellationToken);
            if (tutor == null)
                throw new KeyNotFoundException("Tutor not found.");

            bool changed = false;

            if (tutor.Approved)
            {
                tutor.Approved = false;
                changed = true;
            }

            if (!string.Equals(tutor.Note, TutorRejectedNote, StringComparison.Ordinal))
            {
                tutor.Note = TutorRejectedNote;
                changed = true;
            }

            if (!changed)
                return true;

            var adminUser = await _userContext.GetCurrentUserAsync(cancellationToken);
            if (adminUser != null && !string.IsNullOrWhiteSpace(adminUser.UserName))
                tutor.UpdateBy = adminUser.UserName;

            var affected = await _unitOfWork.SaveChangesAsync();
            return affected > 0;
        }

        public async Task<bool> BanUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId is required.", nameof(userId));

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            if (!user.IsActive)
                return true; 

            user.IsActive = false;
            user.UpdateDate = DateTime.UtcNow;

            var adminUser = await _userContext.GetCurrentUserAsync(cancellationToken);
            if (adminUser != null && !string.IsNullOrWhiteSpace(adminUser.UserName))
                user.UpdateBy = adminUser.UserName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

            return true;
        }

        public async Task<bool> UnbanUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId is required.", nameof(userId));

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            if (user.IsActive)
                return true; 

            user.IsActive = true;
            user.UpdateDate = DateTime.UtcNow;

            var adminUser = await _userContext.GetCurrentUserAsync(cancellationToken);
            if (adminUser != null && !string.IsNullOrWhiteSpace(adminUser.UserName))
                user.UpdateBy = adminUser.UserName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

            return true;
        }
    }
}