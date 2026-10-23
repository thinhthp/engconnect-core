using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace EngConnect.Repositories.Repositories.Submissions;

public class SubmissionRepository : GenericRepository<Submission>, ISubmissionRepository
{
    public SubmissionRepository(EngConnectContext context) : base(context)
    {
    }

    public async Task<Submission?> GetSubmissionById(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Include(x => x.Assignment).Include(x => x.Learner)
            .FirstOrDefaultAsync(x => x.SubmissionId == id, cancellationToken);
    }

    public async Task<Submission?> GetSubmissionByLearner(string id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Include(x => x.Assignment).Include(x => x.Learner)
            .FirstOrDefaultAsync(x => x.LearnerId == id, cancellationToken);
    }
}