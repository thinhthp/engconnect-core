using EngConnect.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.TutorSchedules
{
    public interface ITutorScheduleRepository
    {
        Task<bool> ExistsAsync(string tutorId, DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<TutorSchedule> items, CancellationToken cancellationToken = default);
        Task<List<TutorSchedule>> GetRangeByTutorAsync(string tutorId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default);
        Task<TutorSchedule> GetById(int id);
    }
}
