using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Data;
using EngConnect.Services.DTOs.Calls;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Calls
{
    public class CallService : ICallService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly EngConnectContext _context;

        public CallService(IUnitOfWork unitOfWork, EngConnectContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<long> StartCallAsync(string callerId, StartCallRequest request, CancellationToken cancellationToken = default)
        {
            if (callerId == request.CalleeId)
                throw new InvalidOperationException("Cannot call yourself.");

            var calleeExists = await _context.Users.AnyAsync(u => u.Id == request.CalleeId, cancellationToken);
            if (!calleeExists)
                throw new KeyNotFoundException("Callee user not found.");

            var now = DateTimeOffset.UtcNow;

            var session = new CallSession
            {
                CallerId = callerId,
                CalleeId = request.CalleeId,
                Status = "pending",
                CreatedAt = now,
                IsActive = true,
                Note = request.Note,
                CreateBy = callerId
            };

            var participants = new[]
            {
                new CallParticipant
                {
                    UserId = callerId,
                    IsCaller = true,
                    IsConnected = false,
                    IsMuted = false,
                    IsVideoEnabled = true,
                    JoinedAt = now,
                    IsActive = true,
                    CreateBy = callerId
                },
                new CallParticipant
                {
                    UserId = request.CalleeId,
                    IsCaller = false,
                    IsConnected = false,
                    IsMuted = false,
                    IsVideoEnabled = true,
                    JoinedAt = now,
                    IsActive = true,
                    CreateBy = callerId
                }
            };

            session.Participants = participants.ToList();

            await _unitOfWork.CallSessionRepository.AddAsync(session, cancellationToken);
            await _unitOfWork.CallParticipantRepository.AddRangeAsync(participants, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return session.CallSessionId;
        }

        public async Task<string> ChangeCallStatusAsync(string userId, CallStatusChangeRequest request, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.CallSessionRepository.GetByIdAsync(request.CallSessionId, cancellationToken);
            if (session == null)
                throw new KeyNotFoundException("Call session not found.");

            if (session.CallerId != userId && session.CalleeId != userId)
                throw new UnauthorizedAccessException("Not a participant of this call.");

            var now = DateTimeOffset.UtcNow;

            switch (request.NewStatus)
            {
                case "accepted":
                    session.Status = "active";
                    session.StartedAt ??= now;
                    break;
                case "rejected":
                case "missed":
                    session.Status = request.NewStatus;
                    session.EndedAt ??= now;
                    session.IsActive = false;
                    break;
                case "ended":
                    session.Status = "ended";
                    session.EndedAt ??= now;
                    session.IsActive = false;
                    break;
                default:
                    throw new InvalidOperationException("Unsupported status.");
            }

            session.UpdateBy = userId;
            session.UpdateDate = now;

            await _unitOfWork.CallSessionRepository.UpdateAsync(session, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return session.Status;
        }

        public async Task<(long CallSessionId, string FromUserId, string Type, string Payload, DateTimeOffset CreatedAt)>
            AddSignalAsync(string senderId, CallSignalRequest request, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.CallSessionRepository.GetByIdAsync(request.CallSessionId, cancellationToken);
            if (session == null)
                throw new KeyNotFoundException("Call session not found.");

            if (session.CallerId != senderId && session.CalleeId != senderId)
                throw new UnauthorizedAccessException("Not a participant of this call.");

            var now = DateTimeOffset.UtcNow;

            var signal = new CallSignal
            {
                CallSessionId = request.CallSessionId,
                SenderId = senderId,
                ReceiverId = request.ReceiverId,
                Type = request.Type,
                Payload = request.Payload,
                CreatedAt = now,
                IsActive = true,
                CreateBy = senderId
            };

            await _unitOfWork.CallSignalRepository.AddAsync(signal, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return (request.CallSessionId, senderId, request.Type, request.Payload, now);
        }

        public async Task<CallSession?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.CallSessionRepository.GetByIdAsync(id, cancellationToken);
        }
    }
}