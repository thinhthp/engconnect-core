using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.DTOs.Payments
{
    public class CreatePaymentLinkRequest
    {
        public string? Note { get; set; }
        public List<OrderItemRequest> Items { get; set; } = new();
        public string? ReturnUrlOverride { get; set; }
        public string? CancelUrlOverride { get; set; }
    }

    public class OrderItemRequest
    {
        public int CourseId { get; set; }
        public decimal Price { get; set; }
        public int Sessions { get; set; }
    }
}