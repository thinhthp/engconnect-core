using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Reviews;

public interface ICourseReviewRepository : IGenericRepository<CourseReview>
{
    Task<CourseReview?> GetById(int id, CancellationToken cancellationToken = default);
    Task<List<CourseReview>> GetByCourseId(int courseId, CancellationToken cancellationToken = default);
    Task<List<CourseReview>> GetAll(CancellationToken cancellationToken = default);
    Task UpdateAsync(CourseReview review, CancellationToken cancellationToken = default);
}
