using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.DTOs.TutorSchedules
{
    public record ScheduleSlotResponse(
        int ScheduleId,
        DateTime StartTime,
        DateTime EndTime,
        bool IsBooked,
        string? Note,
        int? AvailabilityId
    );
}
