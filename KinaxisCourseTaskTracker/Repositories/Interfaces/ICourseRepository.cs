using KinaxisCourseTaskTracker.Models;

namespace KinaxisCourseTaskTracker.Repositories.Interfaces;

public interface ICourseRepository
{
    Task<IEnumerable<Course>> GetActiveCoursesAsync();
    Task<Course?> GetByIdWithLessonsAndTasksAsync(int id);
    Task<Course?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(Course course);
    Task UpdateAsync(Course course);
    Task SaveChangesAsync();
}
