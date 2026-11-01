namespace EngConnect.Services.Services.Outbox
{
    public interface IOutboxProcessorService
    {
        Task ProcessPendingAsync(CancellationToken cancellationToken = default);
    }
}
