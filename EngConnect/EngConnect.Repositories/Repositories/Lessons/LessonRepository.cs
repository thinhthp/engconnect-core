using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace EngConnect.Repositories.Repositories.Lessons
{
    public class LessonRepository : GenericRepository<Lesson>, ILessonRepository
    {
        public LessonRepository(EngConnectContext context) : base(context)
        {
        }

        public async Task<Lesson?> GetByIdAsync(int lessonId)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.LessonId == lessonId);
        }

        public async Task<IReadOnlyList<Lesson>> GetByModuleIdAsync(int moduleId)
        {
            return await _dbSet
                .Where(l => l.ModuleId == moduleId)
                .OrderBy(l => l.Position)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> GetNextPositionAsync(int moduleId)
        {
            var max = await _dbSet
                .Where(l => l.ModuleId == moduleId)
                .Select(l => (int?)l.Position)
                .MaxAsync();

            return (max ?? 0) + 1;
        }

        public async Task ShiftPositionsUpFromAsync(int moduleId, int fromPosition)
        {
            var affected = await _dbSet
                .Where(l => l.ModuleId == moduleId && l.Position >= fromPosition)
                .ToListAsync();

            foreach (var l in affected)
                l.Position += 1;
        }

        public async Task ReorderAfterDeletionAsync(int moduleId, int deletedPosition)
        {
            var affected = await _dbSet
                .Where(l => l.ModuleId == moduleId && l.Position > deletedPosition)
                .ToListAsync();

            foreach (var l in affected)
                l.Position -= 1;
        }

        public async Task ApplyReorderOnMoveAsync(int moduleId, int oldPosition, int newPosition)
        {
            if (oldPosition == newPosition) return;

            if (newPosition < oldPosition)
            {
                var affected = await _dbSet
                    .Where(l => l.ModuleId == moduleId &&
                                l.Position >= newPosition &&
                                l.Position < oldPosition)
                    .ToListAsync();
                foreach (var l in affected)
                    l.Position += 1;
            }
            else
            {
                var affected = await _dbSet
                    .Where(l => l.ModuleId == moduleId &&
                                l.Position <= newPosition &&
                                l.Position > oldPosition)
                    .ToListAsync();
                foreach (var l in affected)
                    l.Position -= 1;
            }
        }
    }
}