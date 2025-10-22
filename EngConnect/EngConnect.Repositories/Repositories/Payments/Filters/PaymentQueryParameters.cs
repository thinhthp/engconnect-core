using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Payments.Filters
{
    public class PaymentQueryParameters
    {
        public string? Search { get; set; }
        public string? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SortBy { get; set; } = "createdAt"; // createdAt | transactionDate | amount | status | orderId
        public string? SortDir { get; set; } = "desc";     // asc | desc
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}