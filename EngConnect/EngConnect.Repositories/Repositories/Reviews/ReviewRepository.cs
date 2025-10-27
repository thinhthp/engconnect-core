using EngConnect.Entities.Entities;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace EngConnect.Repositories.Repositories.Reviews;

public class ReviewRepository : IReviewRepository
{
    private readonly EngConnectContext _context;

    public ReviewRepository(EngConnectContext context)
    {
        _context = context;
    }

    public async Task<Review> AddAsync(Review review, CancellationToken cancellationToken = default)
    {
        await _context.Reviews.AddAsync(review, cancellationToken);
        return review;
    }

    public async Task<Review?> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Include(r => r.Learner)
            .Include(r => r.Session)
            .FirstOrDefaultAsync(r => r.ReviewId == id, cancellationToken);
    }

 

    public Task UpdateAsync(Review review, CancellationToken cancellationToken = default)
    {
       
        _context.Reviews.Update(review);
        return Task.CompletedTask;
    }
}
