using EngConnect.Services.DTOs.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Modules
{
    public interface ICourseModuleService
    {
        Task<CourseModuleDTO> CreateAsync(CreateCourseModuleRequest request, string userId);
        Task<IReadOnlyList<CourseModuleDTO>> GetByCourseAsync(int courseId);
        Task<CourseModuleDTO?> GetByIdAsync(int moduleId);
        Task<CourseModuleDTO?> UpdateAsync(int moduleId, UpdateCourseModuleRequest request, string userId);
        Task<bool> DeleteAsync(int moduleId, string userId);
    }
}
