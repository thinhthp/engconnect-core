using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Repositories.TutorProfile.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.TutorProfile
{
    public interface ITutorProfileRepository : IGenericRepository<Entities.Entities.TutorProfile>
    {
        Task<Entities.Entities.TutorProfile?> GetByTutorIdAsync(string tutorId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string tutorId, CancellationToken cancellationToken = default);
        Task<(List<Entities.Entities.TutorProfile> Items, int Total)> QueryAsync(TutorProfileQueryParameters parameters, CancellationToken cancellationToken = default);
        Task UpdateAsync(EngConnect.Entities.Entities.TutorProfile profile, CancellationToken cancellationToken = default);
    }
}