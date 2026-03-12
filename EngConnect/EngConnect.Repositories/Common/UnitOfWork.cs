using EngConnect.Repositories.Data;
using EngConnect.Repositories.Repositories.Assignments;
using EngConnect.Repositories.Repositories.Calls;
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
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EngConnectContext _context;

        private ITutorProfileRepository? _tutorProfileRepository;
        private ICourseRepository? _courseRepository;
        private IEnrolmentRepository? _enrolmentRepository;
        private ISubmissionRepository? _submissionRepository;
        private IAssignmentRepository? _assignmentRepository;
        private ISessionRepository? _sessionRepository;
        private IOrderRepository? _orderRepository;
        private ICourseModuleRepository? _courseModuleRepository;
        private ILessonRepository? _lessonRepository;
        private ITutorWeeklyAvailabilityRepository? _tutorWeeklyAvailabilityRepository;
        private ITutorScheduleRepository? _tutorScheduleRepository;
        private IPaymentRepository? _paymentRepository;
        private IReviewRepository? _reviewRepository;
        private ICourseReviewRepository? _courseReviewRepository;
        private IChatThreadRepository? _chatThreadRepository;
        private IChatParticipantRepository? _chatParticipantRepository;
        private IChatMessageRepository? _chatMessageRepository;
        private ICallSessionRepository? _callSessionRepository;
        private ICallParticipantRepository? _callParticipantRepository;
        private ICallSignalRepository? _callSignalRepository;

        public UnitOfWork(EngConnectContext context)
        {
            _context = context;
        }

        public ITutorProfileRepository TutorProfileRepository => _tutorProfileRepository ??= new TutorProfileRepository(_context);
        public ICourseRepository CourseRepository => _courseRepository ??= new CourseRepository(_context);
        public IEnrolmentRepository EnrolmentRepository => _enrolmentRepository ??= new EnrollmentRepository(_context);
        public ISubmissionRepository SubmissionRepository => _submissionRepository ??= new SubmissionRepository(_context);
        public IAssignmentRepository AssignmentRepository => _assignmentRepository ??= new AssignmentRepository(_context);
        public ISessionRepository SessionRepository => _sessionRepository ??= new SessionReposository(_context);
        public IOrderRepository OrderRepository => _orderRepository ??= new OrderRepository(_context);
        public ICourseModuleRepository CourseModuleRepository => _courseModuleRepository ??= new CourseModuleRepository(_context);
        public ILessonRepository LessonRepository => _lessonRepository ??= new LessonRepository(_context);
        public ITutorWeeklyAvailabilityRepository TutorWeeklyAvailabilityRepository => _tutorWeeklyAvailabilityRepository ??= new TutorWeeklyAvailabilityRepository(_context);
        public ITutorScheduleRepository TutorScheduleRepository => _tutorScheduleRepository ??= new TutorScheduleRepository(_context);
        public IPaymentRepository PaymentRepository => _paymentRepository ??= new PaymentRepository(_context);
        public IReviewRepository ReviewRepository => _reviewRepository ??= new ReviewRepository(_context);
        public ICourseReviewRepository CourseReviewRepository => _courseReviewRepository ??= new CourseReviewRepository(_context);
        public IChatThreadRepository ChatThreadRepository => _chatThreadRepository ??= new ChatThreadRepository(_context);
        public IChatParticipantRepository ChatParticipantRepository => _chatParticipantRepository ??= new ChatParticipantRepository(_context);
        public IChatMessageRepository ChatMessageRepository => _chatMessageRepository ??= new ChatMessageRepository(_context);
        public ICallSessionRepository CallSessionRepository => _callSessionRepository ??= new CallSessionRepository(_context);
        public ICallParticipantRepository CallParticipantRepository => _callParticipantRepository ??= new CallParticipantRepository(_context);
        public ICallSignalRepository CallSignalRepository => _callSignalRepository ??= new CallSignalRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("A concurrency error occurred while saving changes.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while updating the database.", ex);
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}