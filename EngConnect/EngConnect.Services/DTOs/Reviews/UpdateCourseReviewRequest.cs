namespace EngConnect.Services.DTOs.Reviews;

public class UpdateCourseReviewRequest
{
    public int CourseReviewId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
