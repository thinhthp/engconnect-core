using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Repositories.Courses.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Courses
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<Course?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<Course> Items, int TotalCount)> QueryAsync(CourseQueryParameters parameters, CancellationToken cancellationToken = default);
    }
}
