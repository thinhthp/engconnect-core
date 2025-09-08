using EngConnect.Entities.Entities;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.TutorWeeklyAvailabilities
{
    public class TutorWeeklyAvailabilityRepository : ITutorWeeklyAvailabilityRepository
    {
        private readonly EngConnectContext _context;

        public TutorWeeklyAvailabilityRepository(EngConnectContext context)
        {
            _context = context;
        }

        public Task<List<TutorWeeklyAvailability>> GetByTutorAsync(string tutorId, CancellationToken cancellationToken = default)
        {
            return _context.TutorWeeklyAvailabilities
                .Where(a => a.TutorId == tutorId && a.IsActive)
                .OrderBy(a => a.DayOfWeek).ThenBy(a => a.StartTime)
                .ToListAsync(cancellationToken);
        }

        public Task<TutorWeeklyAvailability?> GetByIdAsync(int availabilityId, CancellationToken cancellationToken = default)
        {
            return _context.TutorWeeklyAvailabilities
                .FirstOrDefaultAsync(a => a.AvailabilityId == availabilityId, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<TutorWeeklyAvailability> items, CancellationToken cancellationToken = default)
        {
            await _context.TutorWeeklyAvailabilities.AddRangeAsync(items, cancellationToken);
        }

        public Task RemoveRangeAsync(IEnumerable<TutorWeeklyAvailability> items, CancellationToken cancellationToken = default)
        {
            _context.TutorWeeklyAvailabilities.RemoveRange(items);
            return Task.CompletedTask;
        }

        public Task<bool> OverlapsAsync(string tutorId, int dayOfWeek, TimeOnly start, TimeOnly end, int? excludeAvailabilityId = null, CancellationToken cancellationToken = default)
        {
            return _context.TutorWeeklyAvailabilities
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
            return _context.Set<TutorWeeklyAvailability>()
                .Where(a => a.IsActive)
                .Select(a => a.TutorId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }
    }
}