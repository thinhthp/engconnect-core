using EngConnect.Entities.Entities;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace EngConnect.Repositories.Repositories.Assignments;

public class AssignmentRepository : IAssignmentRepository
{
    private EngConnectContext _context;

    public AssignmentRepository(EngConnectContext context)
    {
        _context = context;
    }

    public async Task<Assignment> GetAssignmentById(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Assignments
            .FirstOrDefaultAsync(a => a.AssignmentId == id, cancellationToken);
    }

    public async Task<Assignment> CreateAssignment(Assignment assignment, CancellationToken cancellationToken = default)
    {
        await _context.Assignments.AddAsync(assignment, cancellationToken);
        return assignment;
    }

    public void UpdateAssignment(Assignment assignment, CancellationToken cancellationToken = default)
    {
       _context.Assignments.Update(assignment);
        
    }
}