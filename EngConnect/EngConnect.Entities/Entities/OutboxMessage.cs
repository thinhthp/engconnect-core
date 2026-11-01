using System;

namespace EngConnect.Entities.Entities
{
    public class OutboxMessage
    {
        public long OutboxMessageId { get; set; }

        public string Type { get; set; } = null!;

        public string Payload { get; set; } = null!;

        public string Status { get; set; } = OutboxMessageStatuses.Pending;

        public int AttemptCount { get; set; }

        public int MaxAttempts { get; set; }

        public DateTime AvailableAt { get; set; } = DateTime.UtcNow; // When the message is available for processing

        public DateTime? LockedUntil { get; set; } // Duration the message is locked for processing

        public string? LastError { get; set; }

        public DateTime? ProcessedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateDate { get; set; }
    }

    public static class OutboxMessageStatuses
    {
        public const string Pending = "Pending";
        public const string Processing = "Processing";
        public const string Processed = "Processed";
        public const string DeadLettered = "DeadLettered";
    }

    public static class OutboxMessageTypes
    {
        public const string Email = "Email";
    }
}
