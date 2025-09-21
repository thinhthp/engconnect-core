using EngConnect.Entities.Entities;

namespace EngConnect.Repositories.Repositories.Submissions;

public interface ISubmissionRepository
{
    Task<Submission> AddAsync(Submission submission, CancellationToken cancellationToken = default);
    Task<Submission> GetSubmissionById(int id, CancellationToken cancellationToken = default);
    
    Task<Submission> GetSubmissionByLearner(string id, CancellationToken cancellationToken = default);
}