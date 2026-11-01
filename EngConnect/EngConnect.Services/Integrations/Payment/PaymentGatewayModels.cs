namespace EngConnect.Services.Integrations.Payment
{
    public static class PaymentGateways
    {
        public const string PayOS = "PAYOS";
    }

    public static class PaymentStatuses
    {
        public const string Pending = "pending";
        public const string Success = "success";
        public const string Canceled = "canceled";
        public const string Failed = "failed";
    }

    public record PaymentLineItem(string Name, int Quantity, long UnitAmount);

    public record PaymentLinkRequest(
        long ReferenceId,
        long Amount,
        string Description,
        string? ReturnUrl,
        string? CancelUrl,
        IReadOnlyList<PaymentLineItem> Items
    );

    public record PaymentLinkResult(
        string CheckoutUrl,
        string TransactionCode
    );

    public record PaymentWebhookResult(
        string TransactionCode,
        string Status,
        string? GatewayCode = null,
        string? Description = null,
        long? Amount = null
    );
}
