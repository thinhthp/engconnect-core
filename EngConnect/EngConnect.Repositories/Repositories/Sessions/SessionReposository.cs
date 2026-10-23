using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace EngConnect.Repositories.Repositories.Sessions;

public class SessionRepository : GenericRepository<Session>, ISessionRepository
{
    public SessionRepository(EngConnectContext context) : base(context)
    {
    }

    public Task<Session> CreateSession(Session session, CancellationToken cancellationToken = default)
    {
        return AddAsync(session, cancellationToken);
    }

    public async Task<List<Session>> GetAllSession(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<Session> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.SessionId == id, cancellationToken);
    }

    public void UpdateSession(Session session, CancellationToken cancellationToken = default)
    {
        Update(session);
    }

    public async Task<int> CountCancelledSessionByEnrollmentId(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Include(x => x.Enrollment)
            .Where(x => x.EnrollmentId == id && x.Status == "Cancelled").CountAsync(cancellationToken);
    }

    public async Task<Session> GetLastSessionByEnrollmentId(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Include(x => x.Enrollment).Where(x => x.EnrollmentId == id)
            .OrderByDescending(x => x.SessionNumber).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsByScheduleId(int scheduleId, CancellationToken ct = default)
    {
        return await _dbSet
            .AnyAsync(s => s.ScheduleId == scheduleId && s.IsActive == true, ct);
    }

    public async Task<int> CountBookedSessionsByEnrollmentIdAndCourseIdInPeriod(int enrollmentId, int courseId, DateTime start, DateTime end,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.EnrollmentId == enrollmentId
                        && s.Status == "Booked"
                        && s.Enrollment.CourseId == courseId
                        && s.StartTime >= start
                        && s.StartTime < end)
            .CountAsync(cancellationToken);
    }
}