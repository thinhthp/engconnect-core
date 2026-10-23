using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace EngConnect.Repositories.Repositories.TutorWeeklyAvailabilities
{
    public class TutorWeeklyAvailabilityRepository : GenericRepository<TutorWeeklyAvailability>, ITutorWeeklyAvailabilityRepository
    {
        public TutorWeeklyAvailabilityRepository(EngConnectContext context) : base(context)
        {
        }

        public Task<List<TutorWeeklyAvailability>> GetByTutorAsync(string tutorId, CancellationToken cancellationToken = default)
        {
            return _dbSet
                .Where(a => a.TutorId == tutorId && a.IsActive)
                .OrderBy(a => a.DayOfWeek).ThenBy(a => a.StartTime)
                .ToListAsync(cancellationToken);
        }

        public Task<TutorWeeklyAvailability?> GetByIdAsync(int availabilityId, CancellationToken cancellationToken = default)
        {
            return _dbSet
                .FirstOrDefaultAsync(a => a.AvailabilityId == availabilityId, cancellationToken);
        }

        public Task<bool> OverlapsAsync(string tutorId, int dayOfWeek, TimeOnly start, TimeOnly end, int? excludeAvailabilityId = null, CancellationToken cancellationToken = default)
        {
            return _dbSet
                .AnyAsync(a =>
                    a.TutorId == tutorId &&
                    a.IsActive &&
                    a.DayOfWeek == dayOfWeek &&
                    (excludeAvailabilityId == null || a.AvailabilityId != excludeAvailabilityId) &&
                    // overlap: start < existing.End && end > existing.Start
                    start < a.EndTime && end > a.StartTime,
                    cancellationToken);
        }

        public Task<List<string>> GetDistinctTutorIdsAsync(CancellationToken cancellationToken = default)
        {
            return _dbSet
                .Where(a => a.IsActive)
                .Select(a => a.TutorId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }
    }
}