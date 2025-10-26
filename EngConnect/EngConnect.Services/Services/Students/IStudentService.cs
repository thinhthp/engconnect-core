using EngConnect.Services.DTOs.Account;

namespace EngConnect.Services.Services.Students;

public interface IStudentService
{
    Task<List<UserResponse>> GetStudentsAsync(CancellationToken cancellationToken = default);
}
