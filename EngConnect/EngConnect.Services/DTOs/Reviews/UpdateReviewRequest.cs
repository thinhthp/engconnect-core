namespace EngConnect.Services.DTOs.Reviews;

public class UpdateReviewRequest
{
    public int ReviewId { get; set; }

    public int Rating { get; set; }
    public string? Comment { get; set; }
    public bool? IsActive { get; set; }
}
