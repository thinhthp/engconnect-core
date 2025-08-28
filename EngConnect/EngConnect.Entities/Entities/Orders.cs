using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Entities.Entities
{
    public class Orders
    {
        public int OrderId { get; set; }

        public string LearnerId { get; set; }

        public decimal TotalAmount { get; set; }

        public string? PaymentMethod { get; set; }

        public string Status { get; set; } = null!;

        public string? Note { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateDate { get; set; }

        public string? CreateBy { get; set; } = "None";

        public string? UpdateBy { get; set; }

        public virtual ApplicationUser Learner { get; set; } = null!;

        public virtual ICollection<OrderItem> Orderitems { get; set; } = new List<OrderItem>();

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
