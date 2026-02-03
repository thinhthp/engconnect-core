using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.DTOs.Calls
{
    public class StartCallRequest
    {
        public string CalleeId { get; set; } = null!;
        public string? Note { get; set; }
    }
}
