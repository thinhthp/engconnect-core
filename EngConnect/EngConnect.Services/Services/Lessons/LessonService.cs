using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Services.DTOs.Lessons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Lessons
{
    public class LessonService : ILessonService
    {
        private readonly IUnitOfWork _uow;

        public LessonService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<LessonDTO> CreateAsync(CreateLessonRequest request, string userId)
        {
            if (request.ModuleId <= 0) throw new ArgumentException("ModuleId is required.", nameof(request.ModuleId));
            if (string.IsNullOrWhiteSpace(request.Title)) throw new ArgumentException("Title is required.", nameof(request.Title));

            int position;
            if (request.Position.HasValue && request.Position.Value > 0)
            {
                position = request.Position.Value;
                await _uow.LessonRepository.ShiftPositionsUpFromAsync(request.ModuleId, position);
            }
            else
            {
                position = await _uow.LessonRepository.GetNextPositionAsync(request.ModuleId);
            }

            var entity = new Lesson
            {
                ModuleId = request.ModuleId,
                Title = request.Title.Trim(),
                ContentUrl = request.ContentUrl,
                LessonType = request.LessonType,
                Duration = request.Duration,
                Note = request.Note,
                Position = position,
                CreateBy = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _uow.LessonRepository.AddAsync(entity);
            await _uow.SaveChangesAsync();

            return ToDto(entity);
        }

        public async Task<IReadOnlyList<LessonDTO>> GetByModuleAsync(int moduleId)
        {
            var list = await _uow.LessonRepository.GetByModuleIdAsync(moduleId);
            return list.Select(ToDto).ToList();
        }

        public async Task<LessonDTO?> GetByIdAsync(int lessonId)
        {
            var entity = await _uow.LessonRepository.GetByIdAsync(lessonId);
            return entity == null ? null : ToDto(entity);
        }

        public async Task<LessonDTO?> UpdateAsync(int lessonId, UpdateLessonRequest request, string userId)
        {
            var entity = await _uow.LessonRepository.GetByIdAsync(lessonId);
            if (entity == null) return null;

            var originalPosition = entity.Position;
            var moduleId = entity.ModuleId ?? 0;

            if (!string.IsNullOrWhiteSpace(request.Title))
                entity.Title = request.Title.Trim();
            if (request.ContentUrl != null)
                entity.ContentUrl = request.ContentUrl;
            if (request.LessonType != null)
                entity.LessonType = request.LessonType;
            if (request.Duration.HasValue)
                entity.Duration = request.Duration;
            if (request.Note != null)
                entity.Note = request.Note;
            if (request.IsActive.HasValue)
                entity.IsActive = request.IsActive.Value;

            entity.UpdateBy = userId;
            entity.UpdateDate = DateTime.UtcNow;

            if (moduleId > 0 && request.Position.HasValue && request.Position.Value > 0 && request.Position.Value != originalPosition)
            {
                var siblings = await _uow.LessonRepository.GetByModuleIdAsync(moduleId);
                var maxPosition = siblings.Count;
                var desired = request.Position.Value;
                if (desired < 1) desired = 1;
                if (desired > maxPosition) desired = maxPosition;

                await _uow.LessonRepository.ApplyReorderOnMoveAsync(moduleId, originalPosition, desired);
                entity.Position = desired;
            }

            _uow.LessonRepository.Update(entity);
            await _uow.SaveChangesAsync();

            return ToDto(entity);
        }

        public async Task<bool> DeleteAsync(int lessonId, string userId)
        {
            var entity = await _uow.LessonRepository.GetByIdAsync(lessonId);
            if (entity == null) return false;

            var deletedPosition = entity.Position;
            var moduleId = entity.ModuleId ?? 0;

            _uow.LessonRepository.Remove(entity);
            if (moduleId > 0)
                await _uow.LessonRepository.ReorderAfterDeletionAsync(moduleId, deletedPosition);

            await _uow.SaveChangesAsync();
            return true;
        }

        private static LessonDTO ToDto(Lesson l) => new()
        {
            LessonId = l.LessonId,
            ModuleId = l.ModuleId,
            Title = l.Title,
            ContentUrl = l.ContentUrl,
            LessonType = l.LessonType,
            Duration = l.Duration,
            Position = l.Position,
            Note = l.Note,
            IsActive = l.IsActive,
            CreatedAt = l.CreatedAt,
            UpdateDate = l.UpdateDate
        };
    }
}