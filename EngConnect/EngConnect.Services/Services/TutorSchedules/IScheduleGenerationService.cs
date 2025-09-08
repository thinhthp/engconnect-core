using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.TutorSchedules
{
    public interface IScheduleGenerationService
    {
        Task<int> GenerateForAllTutorsAsync(int weeksAhead, CancellationToken cancellationToken = default);
        Task<int> GenerateForTutorAsync(string tutorId, int weeksAhead, CancellationToken cancellationToken = default);
    }
}