using EngConnect.Repositories.Repositories.Courses;
using EngConnect.Repositories.Repositories.Lessons;
using EngConnect.Repositories.Repositories.Modules;
using EngConnect.Repositories.Repositories.TutorProfile;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngConnect.Repositories.Repositories.Assignments;
using EngConnect.Repositories.Repositories.Enrollments;
using EngConnect.Repositories.Repositories.Sessions;
using EngConnect.Repositories.Repositories.Submissions;
using EngConnect.Repositories.Repositories.TutorSchedules;
using EngConnect.Repositories.Repositories.TutorWeeklyAvailabilities;

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
        

        IEnrolmentRepository EnrolmentRepository { get; }
        ISubmissionRepository SubmissionRepository { get; }
        IAssignmentRepository AssignmentRepository { get; }
        ISessionRepository SessionRepository { get; }
    
        ITutorWeeklyAvailabilityRepository TutorWeeklyAvailabilityRepository { get; }
        ITutorScheduleRepository TutorScheduleRepository { get; }
    }
}
