using EngConnect.Services.DTOs.TutorProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.TutorProfile
{
    public interface ITutorProfileService
    {
        Task<TutorProfileDTO?> GetCurrentAsync(CancellationToken cancellationToken = default);
        Task<TutorProfileDTO> CreateForCurrentUserAsync(CreateTutorProfileRequest request, CancellationToken cancellationToken = default);
    }
}
