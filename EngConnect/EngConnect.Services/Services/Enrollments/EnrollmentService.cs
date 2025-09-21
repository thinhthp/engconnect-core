using EngConnect.Entities.Common;
using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Repositories.Enrollments;
using EngConnect.Services.DTOs.Courses;
using EngConnect.Services.DTOs.Enrollments;
using EngConnect.Services.Services.UserContext;

namespace EngConnect.Services.Services.Enrollments;

public class EnrollmentService : IEnrollmentService
{
  
    private readonly IUserContextService _user;
    private readonly IUnitOfWork _unitOfWork;

    public EnrollmentService( IUserContextService user, IUnitOfWork unitOfWork)
    {
       
        _user = user;
        _unitOfWork = unitOfWork;
    }
    
    public async  Task<PagedResult<EnrollmentDTO>> GetEnrollmentByLearnerId(int pageSize, int pageNumber, CancellationToken cancellationToken = default)
    {
        var user =  _user.GetCurrentUserId();
        if (string.IsNullOrEmpty(user))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }
        var result = await _unitOfWork.EnrolmentRepository.GetEnrollmentByLearnerId(user, cancellationToken);
        var response =  result.Skip((pageNumber-1)*pageSize).Take(pageSize).Select(e => new EnrollmentDTO()
        {
            Id = e.Id,
            CourseId = e.CourseId,
            Note = e.Note,
            Status = e.Status,
            SessionsPurchased = e.SessionsPurchased,
            SessionsRemaining = e.SessionsRemaining
        }).ToList();
        var count =  response.Count();
        return new PagedResult<EnrollmentDTO>
        {
            Items = response,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = count
        };
    }

    public async Task<EnrollmentDTO> CreateEnrollment(CreateEnrollmentRequest request, CancellationToken cancellationToken = default)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId);
        if (course == null)
        {
            throw new KeyNotFoundException("course is not found.");
        }
        var userId = _user.GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

       
        var entity = new Enrollment()
        {
            LearnerId = userId,
            CourseId = request.CourseId,
            Note = request.Note,
            Status = request.Status,
            SessionsPurchased = course.TotalSessions,
            SessionsRemaining = course.TotalSessions, 
            CreatedAt = DateTime.UtcNow ,
            CreateBy  = userId
        };

        await _unitOfWork.EnrolmentRepository.CreateEnrollment(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

       
        var dto = new EnrollmentDTO()
        {
            Id = entity.Id,
            CourseId = entity.CourseId,
            Note = entity.Note,
            Status = entity.Status,
            SessionsPurchased = entity.SessionsPurchased,
            SessionsRemaining = entity.SessionsRemaining
        };

        return dto;
    }

}