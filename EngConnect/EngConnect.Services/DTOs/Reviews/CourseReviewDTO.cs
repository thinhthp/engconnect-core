namespace EngConnect.Services.DTOs.Reviews;

public class CourseReviewDTO
{
    public int CourseReviewId { get; set; }
    public int CourseId { get; set; }
    public string LearnerId { get; set; } = null!;
    public string? LearnerUserName { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string? Note { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

