using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;

namespace EngConnect.Repositories.Repositories.Submissions;

public interface ISubmissionRepository : IGenericRepository<Submission>
{
    Task<Submission?> GetSubmissionById(int id, CancellationToken cancellationToken = default);
    Task<Submission?> GetSubmissionByLearner(string id, CancellationToken cancellationToken = default);
}