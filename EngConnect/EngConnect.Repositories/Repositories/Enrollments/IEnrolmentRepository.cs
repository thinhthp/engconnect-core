using EngConnect.Entities.Entities;

namespace EngConnect.Repositories.Repositories.Enrollments;

public interface IEnrolmentRepository
{
    Task<List<Enrollment>>GetEnrollmentByLearnerId(string learnerId, CancellationToken cancellationToken = default);
    Task<Enrollment> CreateEnrollment(Enrollment enrollment, CancellationToken cancellationToken = default);
    Task<Enrollment> GetById(int id, CancellationToken cancellationToken = default);
}