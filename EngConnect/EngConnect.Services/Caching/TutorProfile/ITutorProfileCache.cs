using EngConnect.Services.DTOs.TutorProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Caching.TutorProfile
{
    public interface ITutorProfileCache
    {
        Task<TutorProfileDTO?> GetAsync(string tutorId, CancellationToken cancellationToken = default);
        Task SetAsync(TutorProfileDTO profile, CancellationToken cancellationToken = default);
        Task RemoveAsync(string tutorId, CancellationToken cancellationToken = default);
    }
}
