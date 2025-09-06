using EngConnect.Entities.Entities;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Lessons
{
    public class LessonRepository : ILessonRepository
    {
        private readonly EngConnectContext _context;
        public LessonRepository(EngConnectContext context)
        {
            _context = context;
        }

        public async Task<Lesson?> GetByIdAsync(int lessonId)
        {
            return await _context.Lessons
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.LessonId == lessonId);
        }

        public async Task<IReadOnlyList<Lesson>> GetByModuleIdAsync(int moduleId)
        {
            return await _context.Lessons
                .Where(l => l.ModuleId == moduleId)
                .OrderBy(l => l.Position)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Lesson lesson)
        {
            await _context.Lessons.AddAsync(lesson);
        }

        public void Update(Lesson lesson)
        {
            _context.Lessons.Update(lesson);
        }

        public void Remove(Lesson lesson)
        {
            _context.Lessons.Remove(lesson);
        }

        public async Task<int> GetNextPositionAsync(int moduleId)
        {
            var max = await _context.Lessons
                .Where(l => l.ModuleId == moduleId)
                .Select(l => (int?)l.Position)
                .MaxAsync();

            return (max ?? 0) + 1;
        }

        public async Task ShiftPositionsUpFromAsync(int moduleId, int fromPosition)
        {
            var affected = await _context.Lessons
                .Where(l => l.ModuleId == moduleId && l.Position >= fromPosition)
                .ToListAsync();

            foreach (var l in affected)
                l.Position += 1;
        }

        public async Task ReorderAfterDeletionAsync(int moduleId, int deletedPosition)
        {
            var affected = await _context.Lessons
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
                var affected = await _context.Lessons
                    .Where(l => l.ModuleId == moduleId &&
                                l.Position >= newPosition &&
                                l.Position < oldPosition)
                    .ToListAsync();
                foreach (var l in affected)
                    l.Position += 1;
            }
            else
            {
                var affected = await _context.Lessons
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