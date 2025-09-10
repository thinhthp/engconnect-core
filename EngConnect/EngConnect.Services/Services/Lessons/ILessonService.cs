using EngConnect.Services.DTOs.Lessons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Lessons
{
    public interface ILessonService
    {
        Task<LessonDTO> CreateAsync(CreateLessonRequest request, string userId);
        Task<IReadOnlyList<LessonDTO>> GetByModuleAsync(int moduleId);
        Task<LessonDTO?> GetByIdAsync(int lessonId);
        Task<LessonDTO?> UpdateAsync(int lessonId, UpdateLessonRequest request, string userId);
        Task<bool> DeleteAsync(int lessonId, string userId);
    }
}
