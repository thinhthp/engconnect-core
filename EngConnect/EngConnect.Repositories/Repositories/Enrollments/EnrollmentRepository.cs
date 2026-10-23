using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace EngConnect.Repositories.Repositories.Enrollments;

public class EnrollmentRepository : GenericRepository<Enrollment>, IEnrolmentRepository
{
    public EnrollmentRepository(EngConnectContext context) : base(context)
    {
    }

    public async Task<List<Enrollment>> GetEnrollmentByLearnerId(string learnerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.LearnerId == learnerId)
            .Include(x => x.Course)
            .OrderByDescending(x => x.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<Enrollment> CreateEnrollment(Enrollment enrollment, CancellationToken cancellationToken = default)
    {
        return AddAsync(enrollment, cancellationToken);
    }

    public async Task<Enrollment?> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Include(x => x.Course).ThenInclude(x => x.Tutor)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public void Update(Enrollment enrollment, CancellationToken cancellationToken = default)
    {
        base.Update(enrollment);
    }
}