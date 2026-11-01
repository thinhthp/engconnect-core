using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;

namespace EngConnect.Repositories.Repositories.TutorWeeklyAvailabilities
{
    public interface ITutorWeeklyAvailabilityRepository : IGenericRepository<TutorWeeklyAvailability>
    {
        Task<List<TutorWeeklyAvailability>> GetByTutorAsync(string tutorId, CancellationToken cancellationToken = default);
        Task<TutorWeeklyAvailability?> GetByIdAsync(int availabilityId, CancellationToken cancellationToken = default);
        Task<bool> OverlapsAsync(string tutorId, int dayOfWeek, TimeOnly start, TimeOnly end, int? excludeAvailabilityId = null, CancellationToken cancellationToken = default);
        Task<List<string>> GetDistinctTutorIdsAsync(CancellationToken cancellationToken = default);
    }
}
