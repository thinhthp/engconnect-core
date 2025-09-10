using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.TutorSchedules
{
    public class ScheduleGenerationService : IScheduleGenerationService
    {
        private readonly IUnitOfWork _uow;

        public ScheduleGenerationService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> GenerateForAllTutorsAsync(int weeksAhead, CancellationToken cancellationToken = default)
        {
            var tutorIds = await _uow.TutorWeeklyAvailabilityRepository.GetDistinctTutorIdsAsync(cancellationToken);
            var total = 0;
            foreach (var tutorId in tutorIds)
            {
                total += await GenerateForTutorAsync(tutorId, weeksAhead, cancellationToken);
            }
            return total;
        }

        public async Task<int> GenerateForTutorAsync(string tutorId, int weeksAhead, CancellationToken cancellationToken = default)
        {
            if (weeksAhead <= 0) return 0;

            var weekly = await _uow.TutorWeeklyAvailabilityRepository.GetByTutorAsync(tutorId, cancellationToken);
            if (weekly.Count == 0) return 0;

            // Start from next day after run; when scheduled at Sunday 23:59, this is Monday.
            var startDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1));
            var endDate = startDate.AddDays(7 * weeksAhead);

            var toCreate = new List<TutorSchedule>(capacity: weekly.Count * 2 * 2);

            foreach (var av in weekly)
            {
                // iterate all days in range that match
                for (var d = startDate; d < endDate; d = d.AddDays(1))
                {
                    if ((int)d.DayOfWeek != av.DayOfWeek) continue;

                    var start = new DateTime(d.Year, d.Month, d.Day, av.StartTime.Hour, av.StartTime.Minute, 0, DateTimeKind.Utc);
                    var end = new DateTime(d.Year, d.Month, d.Day, av.EndTime.Hour, av.EndTime.Minute, 0, DateTimeKind.Utc);

                    if (end <= start) continue; // guard

                    if (!await _uow.TutorScheduleRepository.ExistsAsync(tutorId, start, end, cancellationToken))
                    {
                        toCreate.Add(new TutorSchedule
                        {
                            TutorId = tutorId,
                            AvailabilityId = av.AvailabilityId,
                            StartTime = start,
                            EndTime = end,
                            IsBooked = false,
                            Note = av.Note,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            CreateBy = "System"
                        });
                    }
                }
            }

            if (toCreate.Count == 0) return 0;

            await _uow.TutorScheduleRepository.AddRangeAsync(toCreate, cancellationToken);
            await _uow.SaveChangesAsync();
            return toCreate.Count;
        }
    }
}