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

        public async Task<TutorSchedule> GetById(int id)
        {
            return await _context.TutorSchedules.Include(x => x.Tutor).Include(x => x.Availability)
                .Include(x => x.Sessions).FirstOrDefaultAsync(x => x.ScheduleId == id);
        }

        public async Task<TutorSchedule?> CheckValidSchedule(int enrollmentId, int scheduleId, CancellationToken cancellationToken =default)
        {
            return await _context.TutorSchedules
                .Where(s => s.ScheduleId == scheduleId)
                .Where(s=>_context.Enrollments.Where(x => x.Id == enrollmentId).Select(x => x.Course.TutorId)
                    .Contains(s.TutorId)).FirstOrDefaultAsync(cancellationToken);
           
        }

        public  void Update(TutorSchedule tutorSchedule, CancellationToken cancellationToken = default)
        {
             _context.TutorSchedules.Update(tutorSchedule);
            
        }
    }
}