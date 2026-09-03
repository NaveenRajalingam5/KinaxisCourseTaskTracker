using KinaxisCourseTaskTracker.DTOs.Enrollment;

namespace KinaxisCourseTaskTracker.Interfaces;

public interface IEnrollmentService
{
    Task<EnrollmentDto> EnrollAsync(int userId, int courseId);
    Task<IEnumerable<EnrollmentDto>> GetLearnerEnrollmentsAsync(int userId);
    Task<EnrollmentDto?> GetEnrollmentDetailsAsync(int userId, int courseId);
}
