using EngConnect.Repositories.Repositories.Courses;
using EngConnect.Repositories.Repositories.Lessons;
using EngConnect.Repositories.Repositories.Modules;
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
        ITutorWeeklyAvailabilityRepository TutorWeeklyAvailabilityRepository { get; }
        ITutorScheduleRepository TutorScheduleRepository { get; }
    }
}
