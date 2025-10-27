using System;

namespace EngConnect.Services.DTOs.Reviews;

public class ReviewDTO
{
    public int ReviewId { get; set; }
    public int SessionId { get; set; }
    public string LearnerId { get; set; } = null!;
    public string? LearnerUserName { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string? Note { get; set; }
    public bool IsActive { get; set; } = true;

    
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
