namespace EngConnect.Services.DTOs.Reviews;

public class CreateReviewRequest
{
   
    public int SessionId { get; set; }

   
    public int Rating { get; set; }

 
    public string? Comment { get; set; }
}
