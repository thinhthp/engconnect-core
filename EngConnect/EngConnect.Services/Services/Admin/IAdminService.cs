using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Admin
{
    public interface IAdminService
    {
        Task<bool> ApproveTutorAsync(string tutorId, CancellationToken cancellationToken = default);
        Task<bool> RejectTutorAsync(string tutorId, CancellationToken cancellationToken = default);
        Task<bool> ApproveCourseAsync(int courseId, CancellationToken cancellationToken = default);
        Task<bool> RejectCourseAsync(int courseId, CancellationToken cancellationToken = default);
    }
}
