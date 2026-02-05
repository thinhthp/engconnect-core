using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.DTOs.Calls
{
    public class UploadCallChunkRequest
    {
        public long CallSessionId { get; set; }
        public int ChunkIndex { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}
