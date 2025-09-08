using EngConnect.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.TutorWeeklyAvailabilities
{
    public interface ITutorWeeklyAvailabilityRepository
    {
        Task<List<TutorWeeklyAvailability>> GetByTutorAsync(string tutorId, CancellationToken cancellationToken = default);
        Task<TutorWeeklyAvailability?> GetByIdAsync(int availabilityId, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<TutorWeeklyAvailability> items, CancellationToken cancellationToken = default);
        Task RemoveRangeAsync(IEnumerable<TutorWeeklyAvailability> items, CancellationToken cancellationToken = default);
        Task<bool> OverlapsAsync(string tutorId, int dayOfWeek, TimeOnly start, TimeOnly end, int? excludeAvailabilityId = null, CancellationToken cancellationToken = default);
        Task<List<string>> GetDistinctTutorIdsAsync(CancellationToken cancellationToken = default);
    }
}
