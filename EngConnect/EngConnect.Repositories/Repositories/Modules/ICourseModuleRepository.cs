using EngConnect.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Modules
{
    public interface ICourseModuleRepository
    {
        Task<CourseModule?> GetByIdAsync(int moduleId);
        Task<IReadOnlyList<CourseModule>> GetByCourseIdAsync(int courseId);
        Task AddAsync(CourseModule module);
        void Update(CourseModule module);
        void Remove(CourseModule module);

        Task<int> GetNextPositionAsync(int courseId);                                 // For create
        Task ShiftPositionsUpFromAsync(int courseId, int fromPosition);               // For create
        Task ReorderAfterDeletionAsync(int courseId, int deletedPosition);            // For delete
        Task ApplyReorderOnMoveAsync(int courseId, int oldPosition, int newPosition); // For update
    }
}
