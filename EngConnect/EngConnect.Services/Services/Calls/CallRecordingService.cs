using EngConnect.Repositories.Common;
using EngConnect.Services.DTOs.Calls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Calls
{
    public class CallRecordingService : ICallRecordingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _rootStoragePath;

        public CallRecordingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _rootStoragePath = @"C:\Users\PC\Desktop\EXE202\storage";
        }

        public async Task UploadChunkAsync(string userId, UploadCallChunkRequest request, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.CallSessionRepository.GetByIdAsync(request.CallSessionId, cancellationToken);
            if (session == null)
                throw new KeyNotFoundException("Call session not found.");

            if (session.CallerId != userId && session.CalleeId != userId)
                throw new UnauthorizedAccessException("Not a participant of this call.");

            var callFolder = Path.Combine(_rootStoragePath, "calls", request.CallSessionId.ToString());
            var chunksFolder = Path.Combine(callFolder, "chunks");

            Directory.CreateDirectory(chunksFolder);

            var extension = Path.GetExtension(request.File.FileName);
            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".webm";
            }

            var chunkFileName = $"{request.ChunkIndex:D6}{extension}";
            var chunkPath = Path.Combine(chunksFolder, chunkFileName);

            using (var stream = new FileStream(chunkPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await request.File.CopyToAsync(stream, cancellationToken);
            }
        }

        public async Task<string> CompleteRecordingAsync(string userId, CompleteRecordingRequest request, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.CallSessionRepository.GetByIdAsync(request.CallSessionId, cancellationToken);
            if (session == null)
                throw new KeyNotFoundException("Call session not found.");

            if (session.CallerId != userId && session.CalleeId != userId)
                throw new UnauthorizedAccessException("Not a participant of this call.");

            var callFolder = Path.Combine(_rootStoragePath, "calls", request.CallSessionId.ToString());
            var chunksFolder = Path.Combine(callFolder, "chunks");
            if (!Directory.Exists(chunksFolder))
                throw new InvalidOperationException("No chunks found for this call session.");

            var outputFileName = string.IsNullOrWhiteSpace(request.OutputFileName)
                ? "recording.webm"
                : request.OutputFileName;

            var outputPath = Path.Combine(callFolder, outputFileName);

            var chunkFiles = Directory.GetFiles(chunksFolder)
                .OrderBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (!chunkFiles.Any())
                throw new InvalidOperationException("No chunks to merge for this call session.");

            Directory.CreateDirectory(callFolder);

            using (var output = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                foreach (var chunkFile in chunkFiles)
                {
                    using var input = new FileStream(chunkFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    await input.CopyToAsync(output, cancellationToken);
                }
            }

            session.RecordingFilePath = outputPath;
            session.RecordingCompleted = true;
            session.UpdateBy = userId;
            session.UpdateDate = DateTimeOffset.UtcNow;

            await _unitOfWork.CallSessionRepository.UpdateAsync(session, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return outputPath;
        }
    }
}