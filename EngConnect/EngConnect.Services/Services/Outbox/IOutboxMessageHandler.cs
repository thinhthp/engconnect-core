namespace EngConnect.Services.Services.Outbox
{
    public interface IOutboxMessageHandler
    {
        string EventType { get; }

        Task HandleAsync(string payload, CancellationToken cancellationToken = default);
    }
}
