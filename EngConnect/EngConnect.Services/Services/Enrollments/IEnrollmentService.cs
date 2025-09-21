using EngConnect.Entities.Common;
using EngConnect.Entities.Entities;
using EngConnect.Services.DTOs.Enrollments;

namespace EngConnect.Services.Services.Enrollments;

public interface IEnrollmentService
{
    Task<PagedResult<EnrollmentDTO>> GetEnrollmentByLearnerId( int pageSize,int pageNumber,CancellationToken cancellationToken = default);

    Task<EnrollmentDTO> CreateEnrollment(CreateEnrollmentRequest request,
        CancellationToken cancellationToken = default);
    
}