using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Repositories.Assignments;
using EngConnect.Repositories.Repositories.Submissions;
using EngConnect.Services.DTOs.Submissions;
using EngConnect.Services.Services.UserContext;

namespace EngConnect.Services.Services.Submissions;

public class SubmissionService : ISubmissionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IUserContextService _user;

    public SubmissionService(
        IUnitOfWork unitOfWork,
        IAssignmentRepository assignmentRepository,
        ISubmissionRepository submissionRepository,
        IUserContextService user)
    {
        _unitOfWork = unitOfWork;
        _assignmentRepository = assignmentRepository;
        _submissionRepository = submissionRepository;
        _user = user;
    }

    public async Task<SubmissionDTO> SubmitAsync(SubmissionDTO dto, CancellationToken cancellationToken = default)
    {
        var currentUser = await _user.GetCurrentUserAsync();
        if (currentUser == null)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }
        var assignment =
            await _assignmentRepository.GetByIdAsync(dto.AssignmentId, cancellationToken) ?? throw new InvalidOperationException("Assignment is not found");

        var entity = new Submission()
        {
            AssignmentId = dto.AssignmentId,
            LearnerId = currentUser.Id,
            Content = dto.Content,
            SubmittedAt = DateTime.UtcNow,
            Status = assignment.DueDate < DateTime.UtcNow ? "Late" : "Submitted",
            Score = null,
            Feedback = null,
            GradedAt = null,
            Note = dto.Note,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreateBy = currentUser.UserName

        };
        await _submissionRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
        return new SubmissionDTO
        {
            SubmissionId = entity.SubmissionId,
            AssignmentId = entity.AssignmentId,
            LearnerId = entity.LearnerId,
            Content = entity.Content,
            SubmittedAt = entity.SubmittedAt,
            Status = entity.Status,
            Note = entity.Note,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            CreateBy = entity.CreateBy
        };
    }

    public async Task<SubmissionDTO> GetSubmissionById(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _submissionRepository.GetSubmissionById(id, cancellationToken);
        if (entity == null)
        {
            throw new KeyNotFoundException("Submission is not found");
        }

        return new SubmissionDTO()
        {
            SubmissionId = entity.SubmissionId,
            AssignmentId = entity.AssignmentId,
            LearnerId = entity.LearnerId,
            Content = entity.Content,
            SubmittedAt = entity.SubmittedAt,
            Status = entity.Status,
            Note = entity.Note,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            CreateBy = entity.CreateBy,
            Feedback = entity.Feedback,
            GradedAt = entity.GradedAt,
            Score = entity.Score,
            UpdateBy = entity.UpdateBy,
            UpdateDate = entity.UpdateDate

        };
    }

    public async Task<SubmissionDTO> GetSubmissionByLearner(CancellationToken cancellationToken = default)
    {
        var currentuser = _user.GetCurrentUserId();
        if (currentuser == null)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }
        var entity = await _submissionRepository.GetSubmissionByLearner(currentuser, cancellationToken);
        if (entity == null)
        {
            throw new KeyNotFoundException("Submission is not found");
        }

        return new SubmissionDTO()
        {
            SubmissionId = entity.SubmissionId,
            AssignmentId = entity.AssignmentId,
            LearnerId = entity.LearnerId,
            Content = entity.Content,
            SubmittedAt = entity.SubmittedAt,
            Status = entity.Status,
            Note = entity.Note,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            CreateBy = entity.CreateBy,
            Feedback = entity.Feedback,
            GradedAt = entity.GradedAt,
            Score = entity.Score,
            UpdateBy = entity.UpdateBy,
            UpdateDate = entity.UpdateDate

        };
    }
}