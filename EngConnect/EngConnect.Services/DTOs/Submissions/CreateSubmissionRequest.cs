namespace EngConnect.Services.DTOs.Submissions;

public class CreateSubmissionRequest
{
    public int AssignmentId { get; set; }
    public string? Content { get; set; }
    public string? Note { get; set; }
}