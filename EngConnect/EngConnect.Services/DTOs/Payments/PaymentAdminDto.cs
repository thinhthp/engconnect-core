using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.DTOs.Payments
{
    public class PaymentAdminDto
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string? TransactionCode { get; set; }
        public string Status { get; set; } = null!;
        public DateTime TransactionDate { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }

        public PaymentOrderSummaryDto Order { get; set; } = null!;
    }
}