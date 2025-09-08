using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Services.DTOs.Sessions;

namespace EngConnect.Services.Services.Sessions;

public class SessionService : ISessionService
{
    private IUnitOfWork _unitOfWork;

    public SessionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // public async Task<SessionDTO> CreateSession(CreateSessionRequest request,
    //     CancellationToken cancellationToken = default)
    // {
    //     // var entity = new Session()
    //     // {
    //     //     EnrollmentId = request.EnrollmentId,
    //     //     ScheduleId = request.ScheduleId,
    //     //     SessionNumber = request.SessionNumber,
    //     //     StartTime = request.StartTime,
    //     //     EndTime = request.EndTime,
    //     //     MeetingLink = request.MeetingLink,
    //     //     Note = request.Note,
    //     //     Status = "Booked",
    //     //     IsActive = true,
    //     //     CreatedAt = DateTime.UtcNow,
    //     //
    //     //
    //     // };
    //     //
    //     // await _unitOfWork.SessionRepository.CreateSession(entity, cancellationToken);
    //     // await _unitOfWork.SaveChangesAsync();
    //     //
    //     // var dto = new SessionDTO()
    //     // {
    //     //     SessionId = entity.SessionId,
    //     //     EnrollmentId = entity.EnrollmentId,
    //     //     ScheduleId = entity.ScheduleId,
    //     //     SessionNumber = entity.SessionNumber,
    //     //     StartTime = entity.StartTime,
    //     //     EndTime = entity.EndTime,
    //     //     MeetingLink = entity.MeetingLink,
    //     //     Note = entity.Note,
    //     //     Status = entity.Status,
    //     //     IsActive = entity.IsActive
    //     // };
    //     //
    //     // return dto;
    // }


public async Task<SessionDTO> CancelSession(int sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _unitOfWork.SessionRepository.GetById(sessionId, cancellationToken);
        if (session == null)
        {
            throw new KeyNotFoundException($"Session with id {sessionId} not found.");
        }

        var enrollment = await _unitOfWork.EnrolmentRepository.GetById(session.EnrollmentId,cancellationToken);
        var now = DateTime.UtcNow;
        var time = session.StartTime - now;
        bool isBefore12h = time.TotalHours >= 12;
        if (isBefore12h)
        {
            if (enrollment.SessionsRemaining <= 0)
            {
                throw new InvalidOperationException("No remaining sessions to deduct.");
            }

            enrollment.SessionsRemaining -= 1;
        }
        
        session.Status = "Cancelled";
        session.IsActive = false;
        session.UpdateDate = DateTime.UtcNow;

        _unitOfWork.SessionRepository.UpdateSession(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        var dto = new SessionDTO()
        {
            SessionId = session.SessionId,
            EnrollmentId = session.EnrollmentId,
            ScheduleId = session.ScheduleId,
            SessionNumber = session.SessionNumber,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            MeetingLink = session.MeetingLink,
            Note = session.Note,
            Status = session.Status,
            IsActive = session.IsActive
        };

        return dto;
    }

    public async Task<List<SessionDTO>> GetAllSessions(CancellationToken cancellationToken = default)
    {
        var sessions = await _unitOfWork.SessionRepository.GetAllSession(cancellationToken);

        return sessions.Select(session => new SessionDTO()
        {
            SessionId = session.SessionId,
            EnrollmentId = session.EnrollmentId,
            ScheduleId = session.ScheduleId,
            SessionNumber = session.SessionNumber,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            MeetingLink = session.MeetingLink,
            Note = session.Note,
            Status = session.Status,
            IsActive = session.IsActive
        }).ToList();
    }

    public async Task<SessionDTO> GetSessionById(int sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _unitOfWork.SessionRepository.GetById(sessionId, cancellationToken);
        if (session == null)
        {
            throw new KeyNotFoundException($"Session with id {sessionId} not found.");
        }

        return new SessionDTO()
        {
            SessionId = session.SessionId,
            EnrollmentId = session.EnrollmentId,
            ScheduleId = session.ScheduleId,
            SessionNumber = session.SessionNumber,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            MeetingLink = session.MeetingLink,
            Note = session.Note,
            Status = session.Status,
            IsActive = session.IsActive
        };
    }
}