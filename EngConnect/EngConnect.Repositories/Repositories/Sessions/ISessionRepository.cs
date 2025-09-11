using EngConnect.Entities.Entities;

namespace EngConnect.Repositories.Repositories.Sessions;

public interface ISessionRepository
{
    Task<Session> CreateSession(Session session, CancellationToken cancellationToken = default);
    // void CancelSession(Session session, CancellationToken cancellationToken = default);
    Task<List<Session>> GetAllSession(CancellationToken cancellationToken = default);
    Task<Session> GetById(int id, CancellationToken cancellationToken = default);
    void UpdateSession(Session session, CancellationToken cancellationToken = default);
    Task<int> CountCancelledSessionByEnrollmentId(int id, CancellationToken cancellationToken = default);
    Task<Session> GetLastSessionByEnrollmentId(int id, CancellationToken cancellationToken = default);

}