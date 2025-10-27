namespace EngConnect.Services.DTOs.Reviews;

public class CreateCourseReviewRequest
{
    public int CourseId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
