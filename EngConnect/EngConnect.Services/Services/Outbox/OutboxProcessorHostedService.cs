using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EngConnect.Services.Services.Outbox
{
    // A bg to wake up the processor service (scoped) (ProcessorService = Scan -> Process)
    public class OutboxProcessorHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IOptions<OutboxOptions> _options;
        private readonly ILogger<OutboxProcessorHostedService> _logger;

        public OutboxProcessorHostedService(
            IServiceScopeFactory scopeFactory,
            IOptions<OutboxOptions> options,
            ILogger<OutboxProcessorHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _options = options;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Outbox processor started. Polling every {IntervalSeconds}s with batch size {BatchSize}.",
                Math.Max(_options.Value.PollingIntervalSeconds, 1),
                Math.Max(_options.Value.BatchSize, 1));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var processor = scope.ServiceProvider.GetRequiredService<IOutboxProcessorService>(); // Since its scoped
                    await processor.ProcessPendingAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Outbox processor tick failed.");
                }

                var delay = TimeSpan.FromSeconds(Math.Max(_options.Value.PollingIntervalSeconds, 1));
                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            _logger.LogInformation("Outbox processor stopped.");
        }
    }
}
