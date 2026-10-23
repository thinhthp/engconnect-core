using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;

namespace EngConnect.Repositories.Repositories.Enrollments;

public interface IEnrolmentRepository : IGenericRepository<Enrollment>
{
    Task<List<Enrollment>> GetEnrollmentByLearnerId(string learnerId, CancellationToken cancellationToken = default);
    Task<Enrollment> CreateEnrollment(Enrollment enrollment, CancellationToken cancellationToken = default);
    Task<Enrollment?> GetById(int id, CancellationToken cancellationToken = default);
    void Update(Enrollment enrollment, CancellationToken cancellationToken = default);
}