using EngConnect.Repositories.Repositories.Assignments;
using EngConnect.Repositories.Repositories.Chat;
using EngConnect.Repositories.Repositories.Courses;
using EngConnect.Repositories.Repositories.Enrollments;
using EngConnect.Repositories.Repositories.Lessons;
using EngConnect.Repositories.Repositories.Modules;
using EngConnect.Repositories.Repositories.Orders;
using EngConnect.Repositories.Repositories.Payments;
using EngConnect.Repositories.Repositories.Reviews;
using EngConnect.Repositories.Repositories.Sessions;
using EngConnect.Repositories.Repositories.Submissions;
using EngConnect.Repositories.Repositories.TutorProfile;
using EngConnect.Repositories.Repositories.TutorSchedules;
using EngConnect.Repositories.Repositories.TutorWeeklyAvailabilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Common
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();

        // ISomethingRepository SomethingRepository { get; }
        // Right here baby
        ITutorProfileRepository TutorProfileRepository { get; }
        ICourseRepository CourseRepository { get; }
        ICourseModuleRepository CourseModuleRepository { get; }
        ILessonRepository LessonRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        IEnrolmentRepository EnrolmentRepository { get; }
        ISubmissionRepository SubmissionRepository { get; }
        IAssignmentRepository AssignmentRepository { get; }
        ISessionRepository SessionRepository { get; }
        IReviewRepository ReviewRepository { get; }
        ICourseReviewRepository CourseReviewRepository { get; }
        ITutorWeeklyAvailabilityRepository TutorWeeklyAvailabilityRepository { get; }
        ITutorScheduleRepository TutorScheduleRepository { get; }
        IOrderRepository OrderRepository { get; }
        IChatThreadRepository ChatThreadRepository { get; }
        IChatParticipantRepository ChatParticipantRepository { get; }
        IChatMessageRepository ChatMessageRepository { get; }
    }
}
