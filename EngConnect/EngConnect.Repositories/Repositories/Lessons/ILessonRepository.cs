using EngConnect.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Lessons
{
    public interface ILessonRepository
    {
        Task<Lesson?> GetByIdAsync(int lessonId);
        Task<IReadOnlyList<Lesson>> GetByModuleIdAsync(int moduleId);
        Task AddAsync(Lesson lesson);
        void Update(Lesson lesson);
        void Remove(Lesson lesson);

        Task<int> GetNextPositionAsync(int moduleId);
        Task ShiftPositionsUpFromAsync(int moduleId, int fromPosition);
        Task ReorderAfterDeletionAsync(int moduleId, int deletedPosition);
        Task ApplyReorderOnMoveAsync(int moduleId, int oldPosition, int newPosition);
    }
}
