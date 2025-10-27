using EngConnect.Services.DTOs.Account;
using EngConnect.Entities.Common;

namespace EngConnect.Services.Services.Students;

public interface IStudentService
{
    Task<PagedResult<UserResponse>> GetStudentsAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
}
