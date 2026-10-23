using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;

namespace EngConnect.Repositories.Repositories.Reviews;

public interface IReviewRepository : IGenericRepository<Review>
{
    Task<Review?> GetById(int id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Review review, CancellationToken cancellationToken = default);
}
