using EngConnect.Entities.Entities;

namespace EngConnect.Repositories.Repositories.Reviews;

public interface IReviewRepository
{
    Task<Review> AddAsync(Review review, CancellationToken cancellationToken = default);
    Task<Review?> GetById(int id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Review review, CancellationToken cancellationToken = default);
}
