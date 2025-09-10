using EngConnect.Entities.Entities;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.TutorSchedules
{
    public class TutorScheduleRepository : ITutorScheduleRepository
    {
        private readonly EngConnectContext _context;

        public TutorScheduleRepository(EngConnectContext context)
        {
            _context = context;
        }

        public Task<bool> ExistsAsync(string tutorId, DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken = default)
        {
            return _context.TutorSchedules
                .AnyAsync(s => s.TutorId == tutorId && s.StartTime == startUtc && s.EndTime == endUtc, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<TutorSchedule> items, CancellationToken cancellationToken = default)
        {
            await _context.TutorSchedules.AddRangeAsync(items, cancellationToken);
        }

        public Task<List<TutorSchedule>> GetRangeByTutorAsync(string tutorId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default)
        {
            return _context.TutorSchedules
                .Where(s => s.TutorId == tutorId && s.StartTime >= fromUtc && s.EndTime <= toUtc)
                .OrderBy(s => s.StartTime)
                .ToListAsync(cancellationToken);
        }
    }
}