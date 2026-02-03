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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EngConnectContext _context;

        // public ISomethingRepository SomethingRepository { get; private set; }
        // Right here baby
        public ITutorProfileRepository TutorProfileRepository { get; private set; }
        public ICourseRepository CourseRepository { get; private set; }
        public IEnrolmentRepository EnrolmentRepository { get; private set; }
        public ISubmissionRepository SubmissionRepository { get; private set; }
        public IAssignmentRepository AssignmentRepository { get; private set; }
        public ISessionRepository SessionRepository { get; }
        public IOrderRepository OrderRepository { get; private set; }
        public ICourseModuleRepository CourseModuleRepository { get; private set; }
        public ILessonRepository LessonRepository { get; private set; }
        public ITutorWeeklyAvailabilityRepository TutorWeeklyAvailabilityRepository { get; private set; }
        public ITutorScheduleRepository TutorScheduleRepository { get; private set; }
        public IPaymentRepository PaymentRepository { get; private set; }
        public IReviewRepository ReviewRepository { get; private set; }
        public ICourseReviewRepository CourseReviewRepository { get; private set; }
        public IChatThreadRepository ChatThreadRepository { get; private set; }
        public IChatParticipantRepository ChatParticipantRepository { get; private set; }
        public IChatMessageRepository ChatMessageRepository { get; private set; }
        public ICallSessionRepository CallSessionRepository { get; }
        public ICallParticipantRepository CallParticipantRepository { get; }
        public ICallSignalRepository CallSignalRepository { get; }

        public UnitOfWork(EngConnectContext context)
        {
            _context = context;

            // SomethingRepository = new SomethingRepository(_context);
            // Right here baby
            TutorProfileRepository = new TutorProfileRepository(_context);
            CourseRepository = new CourseRepository(_context);
            EnrolmentRepository = new EnrollmentRepository(_context);
            SubmissionRepository = new SubmissionRepository(_context);
            AssignmentRepository = new AssignmentRepository(_context);
            SessionRepository = new SessionReposository(_context);
            CourseModuleRepository = new CourseModuleRepository(_context);
            LessonRepository = new LessonRepository(_context);
            TutorWeeklyAvailabilityRepository = new TutorWeeklyAvailabilityRepository(_context);
            TutorScheduleRepository = new TutorScheduleRepository(_context);
            OrderRepository = new OrderRepository(_context);
            PaymentRepository = new PaymentRepository(_context);
            ReviewRepository = new ReviewRepository(_context);
            CourseReviewRepository = new CourseReviewRepository(_context);
            ChatThreadRepository = new ChatThreadRepository(_context);
            ChatParticipantRepository = new ChatParticipantRepository(_context);
            ChatMessageRepository = new ChatMessageRepository(_context);
            CallSessionRepository = new CallSessionRepository(_context);
            CallParticipantRepository = new CallParticipantRepository(_context);
            CallSignalRepository = new CallSignalRepository(_context);
        }

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
            _context?.Dispose();
        }
    }
}
