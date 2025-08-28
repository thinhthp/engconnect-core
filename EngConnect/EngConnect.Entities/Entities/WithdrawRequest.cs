using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Entities.Entities
{
    public class WithdrawRequest
    {
        public int RequestId { get; set; }

        public string TutorId { get; set; }

        public decimal Amount { get; set; }

        public string PayoutInfo { get; set; } = null!;

        public string Status { get; set; } = null!;

        public string? Note { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateDate { get; set; }

        public string? CreateBy { get; set; } = "None";

        public string? UpdateBy { get; set; }

        public virtual TutorProfile Tutor { get; set; } = null!;
    }
}
