using EngConnect.Services.DTOs.Reviews;

namespace EngConnect.Services.Services.Reviews;

public interface IReviewService
{
    Task<ReviewDTO> CreateAsync(CreateReviewRequest request, CancellationToken cancellationToken = default);
    Task<ReviewDTO> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    
    Task<ReviewDTO> UpdateAsync(UpdateReviewRequest request, CancellationToken cancellationToken = default);
   
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
