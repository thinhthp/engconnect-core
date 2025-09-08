using EngConnect.Entities.Entities;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace EngConnect.Repositories.Repositories.Submissions;

public class SubmissionRepository : ISubmissionRepository
{
    private EngConnectContext _context;

    public SubmissionRepository(EngConnectContext context)
    {
        _context = context;
    }

    public async Task<Submission> AddAsync(Submission submission, CancellationToken cancellationToken = default)
    {
         await _context.Submissions.AddAsync(submission);
         return submission;
    }

    public async Task<Submission> GetSubmissionById(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Submissions.Include(x=>x.Assignment).Include(x=>x.Learner).FirstOrDefaultAsync(x => x.SubmissionId == id);
        
    }

    public async Task<Submission> GetSubmissionByLearner(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Submissions.Include(x=>x.Assignment).Include(x=>x.Learner).FirstOrDefaultAsync(x => x.LearnerId == id);
    }
}