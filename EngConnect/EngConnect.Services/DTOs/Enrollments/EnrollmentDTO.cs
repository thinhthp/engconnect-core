namespace EngConnect.Services.DTOs.Enrollments;

public class EnrollmentDTO
{
    public int Id { get; set; }

    

    public int CourseId { get; set; }

    public int SessionsPurchased { get; set; }

    public int SessionsRemaining { get; set; }

    public string Status { get; set; } = null!;

    //public DateTime PurchaseDate { get; set; }

    public string? Note { get; set; }

}