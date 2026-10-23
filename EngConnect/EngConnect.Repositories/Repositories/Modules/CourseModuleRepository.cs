using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace EngConnect.Repositories.Repositories.Modules
{
    public class CourseModuleRepository : GenericRepository<CourseModule>, ICourseModuleRepository
    {
        public CourseModuleRepository(EngConnectContext context) : base(context)
        {
        }

        public async Task<CourseModule?> GetByIdAsync(int moduleId)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ModuleId == moduleId);
        }

        public async Task<IReadOnlyList<CourseModule>> GetByCourseIdAsync(int courseId)
        {
            return await _dbSet
                .Where(m => m.CourseId == courseId)
                .OrderBy(m => m.Position)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> GetNextPositionAsync(int courseId)
        {
            var max = await _dbSet
                .Where(m => m.CourseId == courseId)
                .Select(m => (int?)m.Position)
                .MaxAsync();

            return (max ?? 0) + 1;
        }

        // +1 the pos of modules at or after the inserted one
        public async Task ShiftPositionsUpFromAsync(int courseId, int fromPosition)
        {
            var affected = await _dbSet
                .Where(m => m.CourseId == courseId && m.Position >= fromPosition)
                .ToListAsync();

            foreach (var m in affected)
            {
                m.Position += 1;
            }
        }

        // -1 the pos of modules in the front of the deleted one
        public async Task ReorderAfterDeletionAsync(int courseId, int deletedPosition)
        {
            var affected = await _dbSet
                .Where(m => m.CourseId == courseId && m.Position > deletedPosition)
                .ToListAsync();

            foreach (var m in affected)
            {
                m.Position -= 1;
            }
        }

        // Reorder when moving one module from oldPosition to newPosition
        public async Task ApplyReorderOnMoveAsync(int courseId, int oldPosition, int newPosition)
        {
            if (oldPosition == newPosition) return;

            if (newPosition < oldPosition)
            {
                // Moving up: shift down modules between newPosition and oldPosition - 1
                var affected = await _dbSet
                    .Where(m => m.CourseId == courseId &&
                                m.Position >= newPosition &&
                                m.Position < oldPosition)
                    .ToListAsync();
                foreach (var m in affected)
                    m.Position += 1;
            }
            else
            {
                // Moving down: shift up modules between oldPosition + 1 and newPosition
                var affected = await _dbSet
                    .Where(m => m.CourseId == courseId &&
                                m.Position <= newPosition &&
                                m.Position > oldPosition)
                    .ToListAsync();
                foreach (var m in affected)
                    m.Position -= 1;
            }
        }
    }
}