using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EngConnect.Entities.Common;
using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Services.DTOs.Reviews;
using EngConnect.Services.Services.UserContext;

namespace EngConnect.Services.Services.Reviews;

public class CourseReviewService : ICourseReviewService
{
    private readonly IUnitOfWork _uow;
    private readonly IUserContextService _userContext;

    public CourseReviewService(IUnitOfWork uow, IUserContextService userContext)
    {
        _uow = uow;
        _userContext = userContext;
    }

    public async Task<CourseReviewDTO> CreateAsync(CreateCourseReviewRequest request, CancellationToken cancellationToken = default)
    {
        var currentUser = await _userContext.GetCurrentUserAsync(cancellationToken) ?? throw new UnauthorizedAccessException("User is not authenticated");

        var course = await _uow.CourseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null) throw new KeyNotFoundException("Course not found");

        var entity = new CourseReview
        {
            CourseId = request.CourseId,
            LearnerId = currentUser.Id,
            Rating = request.Rating,
            Comment = request.Comment,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreateBy = currentUser.UserName
        };

        await _uow.CourseReviewRepository.AddAsync(entity, cancellationToken);
        await _uow.SaveChangesAsync();

        return MapToDto(entity);
    }

    public async Task<CourseReviewDTO> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.CourseReviewRepository.GetById(id, cancellationToken) ?? throw new KeyNotFoundException("Course review not found");
        return MapToDto(entity);
    }

    public async Task<PagedResult<CourseReviewDTO>> GetByCourseAsync(int courseId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;

      
        var all = await _uow.CourseReviewRepository.GetByCourseId(courseId, cancellationToken);
        var total = all.Count;
        var pagedItems = all
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dtoItems = pagedItems.Select(MapToDto).ToList();

        return new PagedResult<CourseReviewDTO>
        {
            Items = dtoItems,
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<CourseReviewDTO>> GetAllAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;

      
        var all = await _uow.CourseReviewRepository.GetAll(cancellationToken);
        var total = all.Count;
        var pagedItems = all
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dtoItems = pagedItems.Select(MapToDto).ToList();

        return new PagedResult<CourseReviewDTO>
        {
            Items = dtoItems,
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<CourseReviewDTO> UpdateAsync(UpdateCourseReviewRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.CourseReviewRepository.GetById(request.CourseReviewId, cancellationToken) ?? throw new KeyNotFoundException("Course review not found");

        entity.Rating = request.Rating;
        entity.Comment = request.Comment;
        entity.UpdateDate = DateTime.UtcNow;

        var currentUser = await _userContext.GetCurrentUserAsync(cancellationToken);
        if (currentUser != null) entity.UpdateBy = currentUser.UserName;

        await _uow.CourseReviewRepository.UpdateAsync(entity, cancellationToken);
        await _uow.SaveChangesAsync();

        return MapToDto(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.CourseReviewRepository.GetById(id, cancellationToken) ?? throw new KeyNotFoundException("Course review not found");
        entity.IsActive = false;
        await _uow.CourseReviewRepository.UpdateAsync(entity, cancellationToken);
        await _uow.SaveChangesAsync();
    }

    private CourseReviewDTO MapToDto(CourseReview e)
    {
        return new CourseReviewDTO
        {
            CourseReviewId = e.CourseReviewId,
            CourseId = e.CourseId,
            LearnerId = e.LearnerId,
            LearnerUserName = e.Learner.UserName,
            Rating = e.Rating,
            Comment = e.Comment,
            Note = e.Note,
            IsActive = e.IsActive,
            CreatedDate = e.CreatedAt,
            UpdatedDate = e.UpdateDate
        };
    }
}
