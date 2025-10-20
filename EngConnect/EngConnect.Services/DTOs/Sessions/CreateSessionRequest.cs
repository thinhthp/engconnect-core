namespace EngConnect.Services.DTOs.Sessions;

public class CreateSessionRequest
{
   // public int SessionId { get; set; }

    public int EnrollmentId { get; set; }

    public int ScheduleId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string? MeetingLink { get; set; }

    public string? Note { get; set; }




    
}