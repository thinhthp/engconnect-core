namespace EngConnect.Services.Services.Outbox
{
    public class OutboxOptions
    {
        public const string SectionName = "Outbox";

        public int PollingIntervalSeconds { get; set; } = 5;

        public int BatchSize { get; set; } = 20;

        public int MaxAttempts { get; set; } = 5;

        public int ProcessingTimeoutSeconds { get; set; } = 120;
    }
}
