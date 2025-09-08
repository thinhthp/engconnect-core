using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Repositories.Assignments;
using EngConnect.Services.DTOs.Assignments;
using EngConnect.Services.Services.UserContext;

namespace EngConnect.Services.Services.Assignments;

public class AssignmentService
{
     private readonly IAssignmentRepository _assignmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _user;

    public AssignmentService(IAssignmentRepository assignmentRepository, IUnitOfWork unitOfWork,IUserContextService user)
    {
        _assignmentRepository = assignmentRepository;
        _unitOfWork = unitOfWork;
        _user = user;
    }

    public async Task<AssignmentDTO> CreateAssignment(CreateAssignmentRequest request, CancellationToken cancellationToken = default)
    {
        var currentuser =  _user.GetCurrentUserId();
        if (request.SessionId <= 0)
            throw new ArgumentException("SessionId phải lớn hơn 0.");

        if (request.CourseId <= 0)
            throw new ArgumentException("CourseId phải lớn hơn 0.");

        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Title must not be null.");
        if (string.IsNullOrWhiteSpace(request.Description))
            throw new ArgumentException("Description must not be null.");
        if (string.IsNullOrWhiteSpace(request.Note))
            throw new ArgumentException("Note must not be null.");
        

        if (request.DueDate <= DateTime.UtcNow)
            throw new ArgumentException("DueDate must be greater than now.");
        var session = await _unitOfWork.SessionRepository.GetById(request.SessionId);
        if (session == null)
            throw new ArgumentException($"SessionId {request.SessionId} không tồn tại.");

        
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
            throw new ArgumentException($"CourseId {request.CourseId} không tồn tại.");

        var entity = new Assignment
        {
            Title = request.Title,
            SessionId = request.SessionId,
            CourseId = request.CourseId,
            Description = request.Description,
            DueDate = request.DueDate,
            Note = request.Note,
            CreatedAt = DateTime.UtcNow,
            CreateBy = currentuser
        };

        await _assignmentRepository.CreateAssignment(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return new AssignmentDTO
        {
            AssignmentId = entity.AssignmentId,
            SessionId = entity.SessionId,
            CourseId = entity.CourseId,
            Title = entity.Title,
            Description = entity.Description,
            DueDate = entity.DueDate,
            Note = entity.Note
           
        };
    }

    public async Task<AssignmentDTO> UpdateAssignment(UpdateAssignmentRequest request, CancellationToken cancellationToken = default)
    {
        var currentuser =  _user.GetCurrentUserId();
        var entity = await _assignmentRepository.GetAssignmentById(request.AssignmentId, cancellationToken);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Assignment with id {request.AssignmentId} not found.");
        }

        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.DueDate = request.DueDate;
        entity.Note = request.Note;
        entity.UpdateDate = DateTime.UtcNow;
        entity.UpdateBy = currentuser;
        

        _assignmentRepository.UpdateAssignment(entity,cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return new AssignmentDTO
        {
            AssignmentId = entity.AssignmentId,
            SessionId = entity.SessionId,
            CourseId = entity.CourseId,
            Title = entity.Title,
            Description = entity.Description,
            DueDate = entity.DueDate,
            Note = entity.Note
            
        };
    }

    public async Task<AssignmentDTO> GetAssignmentById(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _assignmentRepository.GetAssignmentById(id, cancellationToken);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Assignment with id {id} not found.");
        }

        return new AssignmentDTO
        {
            AssignmentId = entity.AssignmentId,
            CourseId = entity.CourseId,
            SessionId = entity.SessionId,
            Title = entity.Title,
            Description = entity.Description,
            DueDate = entity.DueDate,
            Note = entity.Note
            
        };
    }
}