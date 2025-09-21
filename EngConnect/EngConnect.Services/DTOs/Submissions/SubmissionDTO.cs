namespace EngConnect.Services.DTOs.Submissions;

public class SubmissionDTO
{
    public int SubmissionId { get; set; }

    public int AssignmentId { get; set; }

    public string LearnerId { get; set; }

    public string? Content { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public string Status { get; set; } = null!;

    public decimal? Score { get; set; }

    public string? Feedback { get; set; }

    public DateTime? GradedAt { get; set; }

    public string? Note { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdateDate { get; set; }

    public string? CreateBy { get; set; } = "None";

    public string? UpdateBy { get; set; }

}