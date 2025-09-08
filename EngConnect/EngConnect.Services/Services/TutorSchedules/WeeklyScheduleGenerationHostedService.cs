using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.TutorSchedules
{
    public class WeeklyScheduleGenerationHostedService : BackgroundService
    {
        private readonly ILogger<WeeklyScheduleGenerationHostedService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public WeeklyScheduleGenerationHostedService(
            ILogger<WeeklyScheduleGenerationHostedService> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        var now = DateTime.UtcNow;
        //        var nextRun = NextSundayAtUtc2359(now);
        //        var delay = nextRun - now;
        //        if (delay < TimeSpan.Zero) delay = TimeSpan.Zero;

        //        _logger.LogInformation("WeeklyScheduleGeneration: next run at {NextRun:u} (in {Delay})", nextRun, delay);
        //        try
        //        {
        //            await Task.Delay(delay, stoppingToken);
        //        }
        //        catch (TaskCanceledException)
        //        {
        //            break;
        //        }
        //        if (stoppingToken.IsCancellationRequested) break;

        //        try
        //        {
        //            using var scope = _scopeFactory.CreateScope();
        //            var service = scope.ServiceProvider.GetRequiredService<IScheduleGenerationService>();
        //            var created = await service.GenerateForAllTutorsAsync(weeksAhead: 2, stoppingToken);
        //            _logger.LogInformation("WeeklyScheduleGeneration: created {Count} schedule slots.", created);
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "WeeklyScheduleGeneration failed.");
        //        }
        //    }
        //}

        //private static DateTime NextSundayAtUtc2359(DateTime fromUtc)
        //{
        //    int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)fromUtc.DayOfWeek + 7) % 7;
        //    var candidate = fromUtc.Date.AddDays(daysUntilSunday).AddHours(23).AddMinutes(59);
        //    if (candidate <= fromUtc)
        //    {
        //        candidate = candidate.AddDays(7);
        //    }
        //    return DateTime.SpecifyKind(candidate, DateTimeKind.Utc);
        //}

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;

#if DEBUG
                var delay = TimeSpan.FromSeconds(60);
                var nextRun = now.Add(delay);
#else
                var nextRun = NextSundayAtUtc2359(now);
                var delay = nextRun - now;
                if (delay < TimeSpan.Zero) delay = TimeSpan.Zero;
#endif

                _logger.LogInformation("WeeklyScheduleGeneration: next run at {NextRun:u} (in {Delay})", nextRun, delay);

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                if (stoppingToken.IsCancellationRequested) break;

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<IScheduleGenerationService>();
                    var created = await service.GenerateForAllTutorsAsync(weeksAhead: 2, stoppingToken);
                    _logger.LogInformation("WeeklyScheduleGeneration: created {Count} schedule slots.", created);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "WeeklyScheduleGeneration failed.");
                }
            }
        }

        private static DateTime NextSundayAtUtc2359(DateTime fromUtc)
        {
            int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)fromUtc.DayOfWeek + 7) % 7;
            var candidate = fromUtc.Date.AddDays(daysUntilSunday).AddHours(23).AddMinutes(59);
            if (candidate <= fromUtc)
            {
                candidate = candidate.AddDays(7);
            }
            return DateTime.SpecifyKind(candidate, DateTimeKind.Utc);
        }
    }
}