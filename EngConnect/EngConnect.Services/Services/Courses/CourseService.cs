using EngConnect.Entities.Common;
using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Repositories.Courses.Filters;
using EngConnect.Services.DTOs.Courses;
using EngConnect.Services.Services.Courses.Filters;
using EngConnect.Services.Services.UserContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Courses
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserContextService _userContext;

        public CourseService(IUnitOfWork uow, IUserContextService userContext)
        {
            _uow = uow;
            _userContext = userContext;
        }

        public async Task<int> CreateCourseAsync(CreateCourseRequest request, CancellationToken cancellationToken = default)
        {
            if (!_userContext.IsAuthenticated)
                throw new UnauthorizedAccessException("User must be authenticated.");

            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Title is required.", nameof(request.Title));

            if (request.TotalSessions <= 0)
                throw new ArgumentException("TotalSessions must be > 0.", nameof(request.TotalSessions));

            if (request.Price < 0)
                throw new ArgumentException("Price must be >= 0.", nameof(request.Price));

            string tutorId = _userContext.GetCurrentUserId()
                               ?? throw new UnauthorizedAccessException("Cannot resolve current user id.");

            var course = new Course
            {
                TutorId = tutorId,
                Title = request.Title.Trim(),
                Description = request.Description,
                Level = request.Level,
                TotalSessions = request.TotalSessions,
                Price = request.Price,
                IntroVideo = request.IntroVideo,
                Status = "pending", // default; admin could approve later
                Note = request.Note,
                IsActive = true,
                CreateBy = tutorId
            };

            await _uow.CourseRepository.AddAsync(course, cancellationToken);
            await _uow.SaveChangesAsync();
            return course.CourseId;
        }

        public async Task<PagedResult<CourseDTO>> GetCoursesAsync(CourseSearchFilter filter, CancellationToken cancellationToken = default)
        {
            var repoParams = new CourseQueryParameters
            {
                Search = filter.Search,
                Level = filter.Level,
                Status = filter.Status,
                TutorId = filter.TutorId,
                MinPrice = filter.MinPrice,
                MaxPrice = filter.MaxPrice,
                IsActive = filter.IsActive,
                SortBy = filter.SortBy,
                SortDir = filter.SortDir,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            var (items, total) = await _uow.CourseRepository.QueryAsync(repoParams, cancellationToken);

            var dto = items.Select(MapToListItem).ToList();

            return new PagedResult<CourseDTO>
            {
                Items = dto,
                TotalCount = total,
                PageNumber = repoParams.PageNumber,
                PageSize = repoParams.PageSize
            };
        }

        public async Task<CourseDTO?> GetCourseByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _uow.CourseRepository.GetByIdAsync(id, cancellationToken);
            if (entity == null) return null;
            return MapToListItem(entity);
        }

        private static CourseDTO MapToListItem(Course c) => new()
        {
            CourseId = c.CourseId,
            TutorId = c.TutorId!,
            Title = c.Title,
            Level = c.Level,
            TotalSessions = c.TotalSessions,
            Price = c.Price,
            Status = c.Status,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        };
    }
}
