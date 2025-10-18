using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Integrations.PayOS
{
    public class PayOSOptions
    {
        public string BaseUrl { get; set; } = "01";
        public string ClientId { get; set; } = "02";
        public string ApiKey { get; set; } = "01";
        public string ChecksumKey { get; set; } = "01";
        public string ReturnUrl { get; set; } = "05";
        public string CancelUrl { get; set; } = "03";
        public string WebhookSecretHeader { get; set; } = "VerySecret";
    }
}