using EngConnect.Services.DTOs.Assignments;

namespace EngConnect.Services.Services.Assignments;

public interface IAssignmentService
{
    Task<AssignmentDTO> CreateAssignment(CreateAssignmentRequest request, CancellationToken cancellationToken = default);

    Task<AssignmentDTO> UpdateAssignment(UpdateAssignmentRequest request, CancellationToken cancellationToken = default);

    Task<AssignmentDTO> GetAssignmentById(int id, CancellationToken cancellationToken = default);

  
    Task DeleteAssignment(int id, CancellationToken cancellationToken = default);
}