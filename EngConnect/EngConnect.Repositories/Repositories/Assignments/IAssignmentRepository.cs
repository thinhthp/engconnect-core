using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;

namespace EngConnect.Repositories.Repositories.Assignments;

public interface IAssignmentRepository : IGenericRepository<Assignment>
{
    //Task<Assignment> GetAssignmentById (int id, CancellationToken cancellationToken = default);
    //Task<Assignment> CreateAssignment(Assignment assignment, CancellationToken cancellationToken = default);
    //void UpdateAssignment(Assignment assignment, CancellationToken cancellationToken = default);
}
