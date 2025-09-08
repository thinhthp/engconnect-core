namespace EngConnect.Services.DTOs.Sessions;

public class SessionDTO
{
    public int SessionId { get; set; }

    public int EnrollmentId { get; set; }

    public int? ScheduleId { get; set; }

    public int SessionNumber { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string? MeetingLink { get; set; }

    public string Status { get; set; } = null!;

    public string? Note { get; set; }

    public bool IsActive { get; set; } = true;

 
}