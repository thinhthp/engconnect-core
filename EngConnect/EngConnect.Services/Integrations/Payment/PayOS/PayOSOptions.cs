namespace EngConnect.Services.Integrations.Payment.PayOS
{
    public class PayOSOptions
    {
        public const string SectionName = "PayOS";

        public string BaseUrl { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ChecksumKey { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
        public string WebhookSecretHeader { get; set; } = string.Empty;
    }
}
