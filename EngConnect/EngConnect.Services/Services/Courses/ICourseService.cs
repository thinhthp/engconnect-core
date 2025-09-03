using EngConnect.Entities.Common;
using EngConnect.Services.DTOs.Courses;
using EngConnect.Services.Services.Courses.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Courses
{
    public interface ICourseService
    {
        Task<int> CreateCourseAsync(CreateCourseRequest request, CancellationToken cancellationToken = default);
        Task<PagedResult<CourseDTO>> GetCoursesAsync(CourseSearchFilter filter, CancellationToken cancellationToken = default);
        Task<CourseDTO?> GetCourseByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
