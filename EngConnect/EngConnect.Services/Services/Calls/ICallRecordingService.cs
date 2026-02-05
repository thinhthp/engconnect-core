using EngConnect.Services.DTOs.Calls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Calls
{
    public interface ICallRecordingService
    {
        Task UploadChunkAsync(string userId, UploadCallChunkRequest request, CancellationToken cancellationToken = default);
        Task<string> CompleteRecordingAsync(string userId, CompleteRecordingRequest request, CancellationToken cancellationToken = default);
    }
}
