namespace EngConnect.Services.DTOs.Assignments;

public class CreateAssignmentRequest
{
   

    public int SessionId { get; set; }

    public int CourseId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime DueDate { get; set; }

    public string? Note { get; set; }

   


}