using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace EngConnect.Repositories.Repositories.TutorSchedules
{
    public class TutorScheduleRepository : GenericRepository<TutorSchedule>, ITutorScheduleRepository
    {
        public TutorScheduleRepository(EngConnectContext context) : base(context)
        {
        }

        public Task<bool> ExistsAsync(string tutorId, DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken = default)
        {
            return _dbSet
                .AnyAsync(s => s.TutorId == tutorId && s.StartTime == startUtc && s.EndTime == endUtc, cancellationToken);
        }

        public Task<List<TutorSchedule>> GetRangeByTutorAsync(string tutorId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default)
        {
            return _dbSet
                .Where(s => s.TutorId == tutorId && s.StartTime >= fromUtc && s.EndTime <= toUtc)
                .OrderBy(s => s.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<TutorSchedule> GetById(int id)
        {
            return await _dbSet.Include(x => x.Tutor).Include(x => x.Availability)
                .Include(x => x.Sessions).FirstOrDefaultAsync(x => x.ScheduleId == id);
        }

        public async Task<TutorSchedule?> CheckValidSchedule(int enrollmentId, int scheduleId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(s => s.ScheduleId == scheduleId)
                .Where(s => _context.Enrollments.Where(x => x.Id == enrollmentId).Select(x => x.Course.TutorId)
                    .Contains(s.TutorId)).FirstOrDefaultAsync(cancellationToken);
        }

        public void Update(TutorSchedule tutorSchedule, CancellationToken cancellationToken = default)
        {
            base.Update(tutorSchedule);
        }
    }
}