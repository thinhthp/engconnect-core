using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.DTOs.Payments
{
    public class CreatePaymentLinkResponse
    {
        public int OrderId { get; set; }
        public int PaymentId { get; set; }
        public string TransactionCode { get; set; } = string.Empty;
        public string CheckoutUrl { get; set; } = string.Empty;
    }
}