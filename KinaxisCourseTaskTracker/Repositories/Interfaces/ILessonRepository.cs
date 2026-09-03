using KinaxisCourseTaskTracker.Models;

namespace KinaxisCourseTaskTracker.Repositories.Interfaces;

public interface ILessonRepository
{
    Task<IEnumerable<Lesson>> GetLessonsByCourseIdAsync(int courseId);
    Task<Lesson?> GetByIdWithTopicsAsync(int id);
    Task<Lesson?> GetByIdAsync(int id);
    Task<List<int>> GetLessonIdsByCourseIdAsync(int courseId);
    Task AddAsync(Lesson lesson);
    Task UpdateAsync(Lesson lesson);
    Task DeleteAsync(Lesson lesson);
    Task SaveChangesAsync();
}
