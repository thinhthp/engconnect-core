namespace EngConnect.Services.Integrations.Payment
{
    public interface IPaymentGateway
    {
        string Name { get; }

        Task<PaymentLinkResult> CreatePaymentLinkAsync(PaymentLinkRequest request, CancellationToken cancellationToken = default);

        Task<PaymentWebhookResult?> VerifyAndParseWebhookAsync(string payload, CancellationToken cancellationToken = default);
    }
}
