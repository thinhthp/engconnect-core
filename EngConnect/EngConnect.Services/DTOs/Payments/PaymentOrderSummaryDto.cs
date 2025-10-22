using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.DTOs.Payments
{
    public class PaymentOrderSummaryDto
    {
        public int OrderId { get; set; }
        public string LearnerId { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string? PaymentMethod { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}