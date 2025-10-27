using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EngConnect.Entities.Entities;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace EngConnect.Repositories.Repositories.Reviews;

public class CourseReviewRepository : ICourseReviewRepository
{
    private readonly EngConnectContext _context;

    public CourseReviewRepository(EngConnectContext context)
    {
        _context = context;
    }

    public async Task<CourseReview> AddAsync(CourseReview review, CancellationToken cancellationToken = default)
    {
        await _context.CourseReviews.AddAsync(review, cancellationToken);
        return review;
    }

    public async Task<CourseReview?> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await _context.CourseReviews
            .Include(r => r.Learner)
            .Include(r => r.Course)
            .FirstOrDefaultAsync(r => r.CourseReviewId == id, cancellationToken);
    }

    public async Task<List<CourseReview>> GetByCourseId(int courseId, CancellationToken cancellationToken = default)
    {
        return await _context.CourseReviews
            .Include(r => r.Learner)
            .Where(r => r.CourseId == courseId && r.IsActive)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CourseReview>> GetAll(CancellationToken cancellationToken = default)
    {
        return await _context.CourseReviews
            .Include(r => r.Learner)
            .Include(r => r.Course)
            .Where(r => r.IsActive)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task UpdateAsync(CourseReview review, CancellationToken cancellationToken = default)
    {
        _context.CourseReviews.Update(review);
        return Task.CompletedTask;
    }
}
