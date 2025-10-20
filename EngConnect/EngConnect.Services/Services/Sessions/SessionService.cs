using System.Runtime.InteropServices;
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

    public async Task<SessionDTO> CreateSession(CreateSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        var enrollment = await _unitOfWork.EnrolmentRepository.GetById(request.EnrollmentId,cancellationToken);
        var lastSession = await _unitOfWork.SessionRepository.GetLastSessionByEnrollmentId(request.EnrollmentId,cancellationToken);
        var tutorschedule =
            await _unitOfWork.TutorScheduleRepository.CheckValidSchedule(request.EnrollmentId, request.ScheduleId,
                cancellationToken);
        if (tutorschedule == null)
        {
            throw new InvalidOperationException("Schedule is not valid for this enrollment (tutor mismatch or not found).");
        }
        if (await _unitOfWork.SessionRepository.ExistsByScheduleId(request.ScheduleId, cancellationToken))
            throw new InvalidOperationException("This schedule has already been booked.");
        // Tính tuần của StartTime (tuần bắt đầu từ thứ 2)
        DateTime startOfWeek = StartOfWeek(tutorschedule.StartTime, DayOfWeek.Monday);
        DateTime endOfWeek = startOfWeek.AddDays(7);

        
        var bookedSessionsCount = await _unitOfWork.SessionRepository.CountBookedSessionsByEnrollmentIdAndCourseIdInPeriod(
            request.EnrollmentId, enrollment.CourseId, startOfWeek, endOfWeek, cancellationToken);

        if (bookedSessionsCount >= 2)
        {
            throw new InvalidOperationException("A student can only book up to 2 slots per course per week.");
        }
        var nextSession = (lastSession?.SessionNumber ?? 0)  + 1;
        var hour = (tutorschedule.StartTime - DateTime.UtcNow).TotalHours;
        if (hour < 12)
        {
            throw new InvalidOperationException("Session must be booked at least 12 hours before start time.");
        }
        
        var entity = new Session()
        {
            EnrollmentId = request.EnrollmentId,
            ScheduleId = request.ScheduleId,
            SessionNumber = nextSession,
            StartTime = tutorschedule.StartTime,
            EndTime = tutorschedule.EndTime,
            MeetingLink = request.MeetingLink,
            Note = request.Note,
            Status = "Booked",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        
        
        };
        await _unitOfWork.SessionRepository.CreateSession(entity, cancellationToken);
        enrollment.SessionsRemaining -= 1;
       _unitOfWork.EnrolmentRepository.Update(enrollment,cancellationToken);
        tutorschedule.IsBooked = true;
         _unitOfWork.TutorScheduleRepository.Update(tutorschedule,cancellationToken);
       
        await _unitOfWork.SaveChangesAsync();
        
        var dto = new SessionDTO()
        {
            SessionId = entity.SessionId,
            EnrollmentId = entity.EnrollmentId,
            ScheduleId = entity.ScheduleId,
            SessionNumber = entity.SessionNumber,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            MeetingLink = entity.MeetingLink,
            Note = entity.Note,
            Status = entity.Status,
            IsActive = entity.IsActive
        };
        
        return dto;
    }


public async Task<SessionDTO> CancelSession(int sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _unitOfWork.SessionRepository.GetById(sessionId, cancellationToken);
        if (session == null)
        {
            throw new KeyNotFoundException($"Session with id {sessionId} not found.");
        }

        var enrollment = await _unitOfWork.EnrolmentRepository.GetById(session.EnrollmentId,cancellationToken);
        var now = DateTime.UtcNow;
        var timeToStart = session.StartTime - now;
     
        if (timeToStart.TotalHours < 0)
            throw new InvalidOperationException("Session already started; cannot cancel.");

        var count = await _unitOfWork.SessionRepository.CountCancelledSessionByEnrollmentId(session.EnrollmentId,cancellationToken);
        bool moreThan12h = timeToStart.TotalHours > 12;
       
        if ( moreThan12h && count == 0)
        {
            enrollment.SessionsRemaining += 1;
        }
        
        session.Status = "Cancelled";
        //session.IsActive = false;
        session.UpdateDate = DateTime.UtcNow;
        _unitOfWork.EnrolmentRepository.Update(enrollment,cancellationToken);

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
    private DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
    {
        int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.Date.AddDays(-1 * diff).Date;
    }

    public async Task<SessionDTO> UpdateMeetingLink(int sessionId, UpdateMeetingLinkRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.MeetingLink))
            throw new ArgumentException("MeetingLink is required.", nameof(request));

        var session = await _unitOfWork.SessionRepository.GetById(sessionId, cancellationToken);
        if (session == null)
            throw new KeyNotFoundException($"Session with id {sessionId} not found.");

        if (string.Equals(session.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Cannot update meeting link for a cancelled session.");

        session.MeetingLink = request.MeetingLink.Trim();
        session.UpdateDate = DateTime.UtcNow;

        _unitOfWork.SessionRepository.UpdateSession(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return new SessionDTO
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