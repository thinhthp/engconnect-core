namespace EngConnect.Services.Services.Outbox.Common
{
    public interface IOutboxProcessorService
    {
        Task ProcessPendingAsync(CancellationToken cancellationToken = default);
    }
}
