using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;

namespace EngConnect.Repositories.Repositories.Modules
{
    public interface ICourseModuleRepository : IGenericRepository<CourseModule>
    {
        Task<CourseModule?> GetByIdAsync(int moduleId);
        Task<IReadOnlyList<CourseModule>> GetByCourseIdAsync(int courseId);

        Task<int> GetNextPositionAsync(int courseId);
        Task ShiftPositionsUpFromAsync(int courseId, int fromPosition);
        Task ReorderAfterDeletionAsync(int courseId, int deletedPosition);
        Task ApplyReorderOnMoveAsync(int courseId, int oldPosition, int newPosition);
    }
}
