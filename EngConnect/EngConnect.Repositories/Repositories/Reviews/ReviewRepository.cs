using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace EngConnect.Repositories.Repositories.Reviews;

public class ReviewRepository : GenericRepository<Review>, IReviewRepository
{
    public ReviewRepository(EngConnectContext context) : base(context)
    {
    }

    public async Task<Review?> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Learner)
            .Include(r => r.Session)
            .FirstOrDefaultAsync(r => r.ReviewId == id, cancellationToken);
    }

    public Task UpdateAsync(Review review, CancellationToken cancellationToken = default)
    {
        Update(review);
        return Task.CompletedTask;
    }
}
