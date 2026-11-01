using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Repositories.Reviews;
using EngConnect.Repositories.Repositories.Sessions;
using EngConnect.Services.DTOs.Reviews;
using EngConnect.Services.Services.UserContext;

namespace EngConnect.Services.Services.Reviews;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _uow;
    private readonly IReviewRepository _reviewRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IUserContextService _userContext;

    public ReviewService(IUnitOfWork uow, IReviewRepository reviewRepository, ISessionRepository sessionRepository, IUserContextService userContext)
    {
        _uow = uow;
        _reviewRepository = reviewRepository;
        _sessionRepository = sessionRepository;
        _userContext = userContext;
    }

    public async Task<ReviewDTO> CreateAsync(CreateReviewRequest request, CancellationToken cancellationToken = default)
    {
        var currentUser = await _userContext.GetCurrentUserAsync(cancellationToken) ?? throw new UnauthorizedAccessException("User is not authenticated");

        var session = await _sessionRepository.GetById(request.SessionId, cancellationToken);
        if (session == null) throw new KeyNotFoundException("Session not found");

        var entity = new Review()
        {
            SessionId = request.SessionId,
            LearnerId = currentUser.Id,
            Rating = request.Rating,
            Comment = request.Comment,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreateBy = currentUser.UserName
        };

        await _reviewRepository.AddAsync(entity, cancellationToken);
        await _uow.SaveChangesAsync();

        return MapToDto(entity);
    }

    public async Task<ReviewDTO> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _reviewRepository.GetById(id, cancellationToken) ?? throw new KeyNotFoundException("Review not found");
        return MapToDto(entity);
    }

    public async Task<ReviewDTO> UpdateAsync(UpdateReviewRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _reviewRepository.GetById(request.ReviewId, cancellationToken) ?? throw new KeyNotFoundException("Review not found");

        entity.Rating = request.Rating;
        entity.Comment = request.Comment;
        entity.UpdateDate = DateTime.UtcNow;

        var currentUser = await _userContext.GetCurrentUserAsync(cancellationToken);
        if (currentUser != null) entity.UpdateBy = currentUser.UserName;

        await _reviewRepository.UpdateAsync(entity, cancellationToken);
        await _uow.SaveChangesAsync();

        return MapToDto(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _reviewRepository.GetById(id, cancellationToken) ?? throw new KeyNotFoundException("Review not found");

        entity.IsActive = false;
        await _reviewRepository.UpdateAsync(entity, cancellationToken);
        await _uow.SaveChangesAsync();
    }

    private ReviewDTO MapToDto(Review e)
    {
        return new ReviewDTO
        {
            ReviewId = e.ReviewId,
            SessionId = e.SessionId,
            LearnerId = e.LearnerId,
            LearnerUserName = e.Learner?.UserName,
            Rating = e.Rating,
            Comment = e.Comment,
            Note = e.Note,
            IsActive = e.IsActive,
            CreatedDate = e.CreatedAt,
            UpdatedDate = e.UpdateDate,
        };
    }
}
