using System.Threading;
using System.Threading.Tasks;
using EngConnect.Services.DTOs.Reviews;
using EngConnect.Entities.Common;

namespace EngConnect.Services.Services.Reviews;

public interface ICourseReviewService
{
    Task<CourseReviewDTO> CreateAsync(CreateCourseReviewRequest request, CancellationToken cancellationToken = default);
    Task<CourseReviewDTO> GetByIdAsync(int id, CancellationToken cancellationToken = default);

  
    Task<PagedResult<CourseReviewDTO>> GetByCourseAsync(int courseId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<PagedResult<CourseReviewDTO>> GetAllAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);

    Task<CourseReviewDTO> UpdateAsync(UpdateCourseReviewRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
