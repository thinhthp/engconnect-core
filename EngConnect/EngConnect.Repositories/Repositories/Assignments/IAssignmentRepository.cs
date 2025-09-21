using EngConnect.Entities.Entities;

namespace EngConnect.Repositories.Repositories.Assignments;

public interface IAssignmentRepository
{
    Task<Assignment> GetAssignmentById (int id, CancellationToken cancellationToken = default);
    Task<Assignment> CreateAssignment(Assignment assignment, CancellationToken cancellationToken = default);

    void UpdateAssignment(Assignment assignment, CancellationToken cancellationToken = default);
}
