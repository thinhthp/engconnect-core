using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace EngConnect.Repositories.Repositories.Assignments;

public class AssignmentRepository : GenericRepository<Assignment>, IAssignmentRepository
{
    public AssignmentRepository(EngConnectContext context) : base(context)
    {
    }

    //public async Task<Assignment> GetAssignmentById(int id, CancellationToken cancellationToken = default)
    //{
    //    return await _dbSet.FirstOrDefaultAsync(a => a.AssignmentId == id, cancellationToken);
    //}

    //public Task<Assignment> CreateAssignment(Assignment assignment, CancellationToken cancellationToken = default)
    //{
    //    return AddAsync(assignment, cancellationToken);
    //}

    //public void UpdateAssignment(Assignment assignment, CancellationToken cancellationToken = default)
    //{
    //    Update(assignment);
    //}
}