namespace EngConnect.Services.Services.Outbox.Email
{
    public class EmailOutboxPayload
    {
        public string ToEmail { get; set; } = null!;

        public string Subject { get; set; } = null!;

        public string HtmlContent { get; set; } = null!;
    }
}
