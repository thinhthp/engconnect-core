using EngConnect.Repositories.Common;
using EngConnect.Services.DTOs.TutorSchedules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.TutorSchedules
{
    public class TutorScheduleService : ITutorScheduleService
    {
        private readonly IUnitOfWork _uow;

        public TutorScheduleService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IReadOnlyList<ScheduleSlotResponse>> GetByTutorAsync(
            string tutorId,
            DateTime? from = null,
            DateTime? to = null,
            bool onlyAvailable = true,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(tutorId))
                throw new ArgumentException("tutorId is required.", nameof(tutorId));

            var fromUtc = (from ?? DateTime.UtcNow).ToUniversalTime();
            var toUtc = (to ?? fromUtc.AddDays(14)).ToUniversalTime();
            if (toUtc <= fromUtc)
                throw new ArgumentException("'to' must be greater than 'from'.");

            var items = await _uow.TutorScheduleRepository.GetRangeByTutorAsync(tutorId, fromUtc, toUtc, ct);

            var result = items
                //.Where(s => s.IsActive && (!onlyAvailable || !s.IsBooked))
                .Where(s => s.IsActive)
                .OrderBy(s => s.StartTime)
                .Select(s => new ScheduleSlotResponse(
                    s.ScheduleId,
                    s.StartTime,
                    s.EndTime,
                    s.IsBooked,
                    s.Note,
                    s.AvailabilityId))
                .ToList();

            return result;
        }
    }
}