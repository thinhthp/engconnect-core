using EngConnect.Entities.Entities;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Modules
{
    public class CourseModuleRepository : ICourseModuleRepository
    {
        private readonly EngConnectContext _context;

        public CourseModuleRepository(EngConnectContext context)
        {
            _context = context;
        }

        public async Task<CourseModule?> GetByIdAsync(int moduleId)
        {
            return await _context.CourseModules
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ModuleId == moduleId);
        }

        public async Task<IReadOnlyList<CourseModule>> GetByCourseIdAsync(int courseId)
        {
            return await _context.CourseModules
                .Where(m => m.CourseId == courseId)
                .OrderBy(m => m.Position)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(CourseModule module)
        {
            await _context.CourseModules.AddAsync(module);
        }

        public void Update(CourseModule module)
        {
            _context.CourseModules.Update(module);
        }

        public void Remove(CourseModule module)
        {
            _context.CourseModules.Remove(module);
        }

        public async Task<int> GetNextPositionAsync(int courseId)
        {
            var max = await _context.CourseModules
                .Where(m => m.CourseId == courseId)
                .Select(m => (int?)m.Position)
                .MaxAsync();

            return (max ?? 0) + 1;
        }

        // +1 the pos of modules at or after the inserted one
        public async Task ShiftPositionsUpFromAsync(int courseId, int fromPosition)
        {
            var affected = await _context.CourseModules
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
            var affected = await _context.CourseModules
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
                var affected = await _context.CourseModules
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
                var affected = await _context.CourseModules
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