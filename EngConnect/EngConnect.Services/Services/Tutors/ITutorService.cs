using EngConnect.Entities.Common;
using EngConnect.Services.DTOs.Account;
using EngConnect.Services.Services.TutorProfile.Filters;
using System.Threading;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Tutors
{
    public interface ITutorService
    {
        Task<PagedResult<UserResponse>> GetTutorsAsync(TutorProfileSearchFilter filter, CancellationToken cancellationToken = default);
    }
}
