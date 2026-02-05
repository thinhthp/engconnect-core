using EngConnect.Entities.Entities;
using EngConnect.Repositories.Data;
using EngConnect.Services.DTOs.Calls;
using EngConnect.Services.Services.Calls;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace EngConnect.Api.Hubs
{
    public class CallHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> UserConnections = new();

        private readonly ICallService _callService;

        public CallHub(ICallService callService)
        {
            _callService = callService;
        }

        public override Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                UserConnections[userId] = Context.ConnectionId;
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                UserConnections.TryRemove(userId, out _);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task<long> StartCall(StartCallRequest request)
        {
            var callerId = GetUserIdOrThrow();

            var callSessionId = await _callService.StartCallAsync(callerId, request);

            if (UserConnections.TryGetValue(request.CalleeId, out var calleeConnId))
            {
                await Clients.Client(calleeConnId).SendAsync("IncomingCall", new
                {
                    CallSessionId = callSessionId,
                    FromUserId = callerId,
                    Note = request.Note,
                    CreatedAt = DateTimeOffset.UtcNow
                });
            }

            return callSessionId;
        }

        public async Task ChangeCallStatus(CallStatusChangeRequest request)
        {
            var userId = GetUserIdOrThrow();

            var newStatus = await _callService.ChangeCallStatusAsync(userId, request);

            var callSession = await _callService.GetByIdAsync(request.CallSessionId);
            var otherUserId = callSession?.CallerId == userId
                ? callSession.CalleeId
                : callSession?.CallerId;
            if (!string.IsNullOrEmpty(otherUserId) &&
                UserConnections.TryGetValue(otherUserId, out var otherConnId))
            {
                await Clients.Client(otherConnId).SendAsync("CallStatusChanged", new
                {
                    CallSessionId = request.CallSessionId,
                    NewStatus = newStatus,
                    ChangedByUserId = userId
                });
            }
        }

        public async Task SendSignal(CallSignalRequest request)
        {
            var senderId = GetUserIdOrThrow();

            var result = await _callService.AddSignalAsync(senderId, request);

            if (UserConnections.TryGetValue(request.ReceiverId, out var receiverConnId))
            {
                await Clients.Client(receiverConnId).SendAsync("ReceiveSignal", new
                {
                    CallSessionId = result.CallSessionId,
                    FromUserId = result.FromUserId,
                    Type = result.Type,
                    Payload = result.Payload,
                    CreatedAt = result.CreatedAt
                });
            }
        }

        private string? GetUserId()
        {
            return Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private string GetUserIdOrThrow()
        {
            var id = GetUserId();
            if (string.IsNullOrEmpty(id))
                throw new HubException("Unauthenticated.");

            return id;
        }
    }
}