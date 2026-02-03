using EngConnect.Entities.Entities;
using EngConnect.Services.DTOs.Calls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Calls
{
    public interface ICallService
    {
        Task<long> StartCallAsync(string callerId, StartCallRequest request, CancellationToken cancellationToken = default);
        Task<string> ChangeCallStatusAsync(string userId, CallStatusChangeRequest request, CancellationToken cancellationToken = default);
        Task<(long CallSessionId, string FromUserId, string Type, string Payload, DateTimeOffset CreatedAt)> AddSignalAsync(string senderId, CallSignalRequest request, CancellationToken cancellationToken = default);
        Task<CallSession?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    }
}