using EngConnect.Entities.Entities;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace EngConnect.Repositories.Repositories.Sessions;

public class SessionReposository : ISessionRepository
{
    private readonly EngConnectContext _context;

    public SessionReposository(EngConnectContext context)
    {
        _context = context;
    }

    public async Task<Session> CreateSession(Session session, CancellationToken cancellationToken = default)
    {
        await _context.Sessions.AddAsync(session);
        return session;
    }

    // public  void CancelSession(Session session, CancellationToken cancellationToken = default)
    // {
    //     _context.Sessions.Update(session);
    // }



    public async Task<List<Session>> GetAllSession(CancellationToken cancellationToken = default)
    {
        return await _context.Sessions.ToListAsync(cancellationToken);
    }

    public async Task<Session> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Sessions.FirstOrDefaultAsync(s => s.SessionId == id, cancellationToken);
    }

    public void UpdateSession(Session session, CancellationToken cancellationToken = default)
    {
        _context.Sessions.Update(session);
    }

    public async Task<int> CountCancelledSessionByEnrollmentId(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Sessions.Include(x => x.Enrollment)
            .Where(x => x.EnrollmentId == id && x.Status == "Cancelled").CountAsync(cancellationToken);
    }

    public async Task<Session> GetLastSessionByEnrollmentId(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Sessions.Include(x => x.Enrollment).Where(x => x.EnrollmentId == id)
            .OrderByDescending(x => x.SessionNumber).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsByScheduleId(int scheduleId, CancellationToken ct = default)
    {
        return await _context.Sessions
            .AnyAsync(s => s.ScheduleId == scheduleId && s.IsActive == true, ct);
    }

    public async Task<int> CountBookedSessionsByEnrollmentIdAndCourseIdInPeriod(int enrollmentId, int courseId, DateTime start, DateTime end,
        CancellationToken cancellationToken = default)
    {
        return await _context.Sessions
            .Where(s => s.EnrollmentId == enrollmentId
                        && s.Status == "Booked"
                        && s.Enrollment.CourseId == courseId
                        && s.StartTime >= start
                        && s.StartTime < end)
            .CountAsync(cancellationToken);
    }
}