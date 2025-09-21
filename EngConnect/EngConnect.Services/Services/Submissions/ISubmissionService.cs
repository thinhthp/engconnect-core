using EngConnect.Entities.Entities;
using EngConnect.Services.DTOs.Submissions;

namespace EngConnect.Services.Services.Submissions;

public interface ISubmissionService
{
    Task<SubmissionDTO> SubmitAsync(SubmissionDTO submissionDto, CancellationToken cancellationToken = default);
    Task<SubmissionDTO> GetSubmissionById(int id, CancellationToken cancellationToken = default);
    Task<SubmissionDTO> GetSubmissionByLearner(CancellationToken cancellationToken = default);
}