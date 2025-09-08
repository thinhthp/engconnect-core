using EngConnect.Entities.Common;
using EngConnect.Entities.Entities;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace EngConnect.Repositories.Repositories.Enrollments;

public class EnrollmentRepository : IEnrolmentRepository
{
    private readonly EngConnectContext _context;

    public EnrollmentRepository(EngConnectContext context)
    {
        _context = context;
    }
    public async Task<List<Enrollment>> GetEnrollmentByLearnerId(string learnerId, CancellationToken cancellationToken = default)
    {  
        var response =  await _context.Enrollments
            .Where(x => x.LearnerId == learnerId)
            .Include(x => x.Course)
            .OrderByDescending(x => x.CreatedAt)
            .AsNoTracking() 
            .ToListAsync(cancellationToken);
        return response;
    }

    public async Task<Enrollment> CreateEnrollment(Enrollment enrollment, CancellationToken cancellationToken = default)
    {
       await _context.Enrollments.AddAsync(enrollment);
        return enrollment;
    }

    public async Task<Enrollment> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Enrollments.FirstOrDefaultAsync(x => x.Id == id);
    }
}