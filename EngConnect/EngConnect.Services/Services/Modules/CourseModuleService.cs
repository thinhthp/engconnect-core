using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Repositories.Modules;
using EngConnect.Services.DTOs.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Modules
{
    public class CourseModuleService : ICourseModuleService
    {
        private readonly IUnitOfWork _uow;

        public CourseModuleService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CourseModuleDTO> CreateAsync(CreateCourseModuleRequest request, string userId)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Title is required.", nameof(request.Title));

            // Xac dinh pos
            int position;
            if (request.Position.HasValue && request.Position.Value > 0)
            {
                position = request.Position.Value;
                // Shift existing modules at or after this position
                await _uow.CourseModuleRepository.ShiftPositionsUpFromAsync(request.CourseId, position);
            }
            else
            {
                position = await _uow.CourseModuleRepository.GetNextPositionAsync(request.CourseId);
            }

            var entity = new CourseModule
            {
                CourseId = request.CourseId,
                Title = request.Title.Trim(),
                Description = request.Description,
                Note = request.Note,
                Position = position,
                CreateBy = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _uow.CourseModuleRepository.AddAsync(entity);
            await _uow.SaveChangesAsync();

            return ToDto(entity);
        }

        public async Task<IReadOnlyList<CourseModuleDTO>> GetByCourseAsync(int courseId)
        {
            var list = await _uow.CourseModuleRepository.GetByCourseIdAsync(courseId);
            return list.Select(ToDto).ToList();
        }

        public async Task<CourseModuleDTO?> GetByIdAsync(int moduleId)
        {
            var module = await _uow.CourseModuleRepository.GetByIdAsync(moduleId);
            return module == null ? null : ToDto(module);
        }

        public async Task<CourseModuleDTO?> UpdateAsync(int moduleId, UpdateCourseModuleRequest request, string userId)
        {
            var tracked = await _uow.CourseModuleRepository.GetByIdAsync(moduleId);
            if (tracked == null) return null;

            var originalPosition = tracked.Position;

            if (!string.IsNullOrWhiteSpace(request.Title))
                tracked.Title = request.Title.Trim();

            if (request.Description != null)
                tracked.Description = request.Description;

            if (request.Note != null)
                tracked.Note = request.Note;

            if (request.IsActive.HasValue)
                tracked.IsActive = request.IsActive.Value;

            tracked.UpdateBy = userId;
            tracked.UpdateDate = DateTime.UtcNow;

            if (request.Position.HasValue && request.Position.Value > 0 && request.Position.Value != originalPosition)
            {
                var siblings = await _uow.CourseModuleRepository.GetByCourseIdAsync(tracked.CourseId);
                var maxPosition = siblings.Count;
                var desired = request.Position.Value;
                if (desired < 1) desired = 1;
                if (desired > maxPosition) desired = maxPosition;

                await _uow.CourseModuleRepository.ApplyReorderOnMoveAsync(tracked.CourseId, originalPosition, desired);
                tracked.Position = desired;
            }

            _uow.CourseModuleRepository.Update(tracked);
            await _uow.SaveChangesAsync();

            return ToDto(tracked);
        }

        public async Task<bool> DeleteAsync(int moduleId, string userId)
        {
            var module = await _uow.CourseModuleRepository.GetByIdAsync(moduleId);
            if (module == null) return false;

            var deletedPosition = module.Position;
            var courseId = module.CourseId;

            _uow.CourseModuleRepository.Remove(module);
            await _uow.CourseModuleRepository.ReorderAfterDeletionAsync(courseId, deletedPosition);

            await _uow.SaveChangesAsync();
            return true;
        }

        private static CourseModuleDTO ToDto(CourseModule m) => new()
        {
            ModuleId = m.ModuleId,
            CourseId = m.CourseId,
            Title = m.Title,
            Description = m.Description,
            Position = m.Position,
            Note = m.Note,
            IsActive = m.IsActive,
            CreatedAt = m.CreatedAt,
            UpdateDate = m.UpdateDate
        };
    }
}