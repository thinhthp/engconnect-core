using EngConnect.Entities.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Reviews;

public interface ICourseReviewRepository
{
    Task<CourseReview> AddAsync(CourseReview review, CancellationToken cancellationToken = default);
    Task<CourseReview?> GetById(int id, CancellationToken cancellationToken = default);
    Task<List<CourseReview>> GetByCourseId(int courseId, CancellationToken cancellationToken = default);
    Task<List<CourseReview>> GetAll(CancellationToken cancellationToken = default);
    Task UpdateAsync(CourseReview review, CancellationToken cancellationToken = default);
}
