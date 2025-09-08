using EngConnect.Services.DTOs.Sessions;

namespace EngConnect.Services.Services.Sessions;

public interface ISessionService
{
    // Task<SessionDTO> CreateSession(CreateSessionRequest request, CancellationToken cancellationToken = default);
    Task<SessionDTO> CancelSession(int sessionId, CancellationToken cancellationToken = default);

    Task<List<SessionDTO>> GetAllSessions(CancellationToken cancellationToken = default);

    Task<SessionDTO> GetSessionById(int sessionId, CancellationToken cancellationToken = default);
}