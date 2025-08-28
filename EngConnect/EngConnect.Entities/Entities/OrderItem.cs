using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Entities.Entities
{
    public class OrderItem
    {
        public int ItemId { get; set; }

        public int OrderId { get; set; }

        public int CourseId { get; set; }

        public decimal Price { get; set; }

        public int Sessions { get; set; }

        public virtual Course Course { get; set; } = null!;

        public virtual Orders Order { get; set; } = null!;
    }
}
