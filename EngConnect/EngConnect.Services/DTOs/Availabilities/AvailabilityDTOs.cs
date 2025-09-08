using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.DTOs.Availabilities
{
    public record AvailabilitySlotRequest(
        int DayOfWeek, // 0=Sunday .. 6=Saturday
        TimeOnly StartTime,
        TimeOnly EndTime,
        string? Note
    );

    public record BulkAvailabilityRequest(
        IEnumerable<AvailabilitySlotRequest> Slots,
        bool ReplaceExisting = true
    );

    public record AvailabilitySlotResponse(
        int AvailabilityId,
        int DayOfWeek,
        TimeOnly StartTime,
        TimeOnly EndTime,
        string? Note
    );
}