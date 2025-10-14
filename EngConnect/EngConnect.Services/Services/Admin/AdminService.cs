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
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;

        private const string StatusApproved = "approved";
        private const string StatusRejected = "rejected";
        private const string TutorRejectedNote = "Rejected by admin, please check your mail for detail.";

        public AdminService(IUnitOfWork unitOfWork, IUserContextService userContext)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
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

            var adminId = _userContext.GetCurrentUserId();
            if (!string.IsNullOrWhiteSpace(adminId))
                course.UpdateBy = adminId;

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

            var adminId = _userContext.GetCurrentUserId();
            if (!string.IsNullOrWhiteSpace(adminId))
                course.UpdateBy = adminId;

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

            var adminId = _userContext.GetCurrentUserId();
            if (!string.IsNullOrWhiteSpace(adminId))
                tutor.UpdateBy = adminId;

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

            var adminId = _userContext.GetCurrentUserId();
            if (!string.IsNullOrWhiteSpace(adminId))
                tutor.UpdateBy = adminId;

            var affected = await _unitOfWork.SaveChangesAsync();
            return affected > 0;
        }
    }
}