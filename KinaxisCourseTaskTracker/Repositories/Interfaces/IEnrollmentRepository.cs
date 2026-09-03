using KinaxisCourseTaskTracker.Models;

namespace KinaxisCourseTaskTracker.Repositories.Interfaces;

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetEnrollmentAsync(int userId, int courseId);
    Task<IEnumerable<Enrollment>> GetUserEnrollmentsAsync(int userId);
    Task<bool> IsUserEnrolledAsync(int userId, int courseId);
    Task AddEnrollmentAsync(Enrollment enrollment);
    Task<LessonProgress?> GetLessonProgressAsync(int userId, int lessonId);
    Task AddLessonProgressAsync(LessonProgress progress);
    Task<int> GetCompletedLessonsCountAsync(int userId, IEnumerable<int> lessonIds);
    Task<int> GetTotalLessonTimeMinutesAsync(int userId);
    Task SaveChangesAsync();
}
