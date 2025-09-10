using EngConnect.Services.DTOs.TutorSchedules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.TutorSchedules
{
    public interface ITutorScheduleService
    {
        Task<IReadOnlyList<ScheduleSlotResponse>> GetByTutorAsync(
            string tutorId,
            DateTime? from = null,
            DateTime? to = null,
            bool onlyAvailable = true,
            CancellationToken ct = default);
    }
}
