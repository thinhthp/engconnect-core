namespace EngConnect.Services.Services.Outbox
{
    public interface IOutboxService
    {
        Task EnqueueAsync(string type, string payload, CancellationToken cancellationToken = default);

        Task EnqueueEmailAsync(string toEmail, string subject, string htmlContent, CancellationToken cancellationToken = default);
    }
}
