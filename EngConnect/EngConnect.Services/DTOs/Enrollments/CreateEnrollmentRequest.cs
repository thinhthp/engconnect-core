namespace EngConnect.Services.DTOs.Enrollments;

public class CreateEnrollmentRequest
{
    public string LearnerId { get; set; }

    public int CourseId { get; set; }

    public int SessionsPurchased { get; set; }

    public int SessionsRemaining { get; set; }

    public string Status { get; set; } = null!;

    public string? Note { get; set; }
}