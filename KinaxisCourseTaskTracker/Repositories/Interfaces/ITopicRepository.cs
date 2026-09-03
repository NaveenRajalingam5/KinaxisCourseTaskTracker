using KinaxisCourseTaskTracker.Models;

namespace KinaxisCourseTaskTracker.Repositories.Interfaces;

public interface ITopicRepository
{
    Task<IEnumerable<Topic>> GetTopicsByLessonIdAsync(int lessonId);
    Task<Topic?> GetByIdAsync(int id);
    Task AddAsync(Topic topic);
    Task UpdateAsync(Topic topic);
    Task DeleteAsync(Topic topic);
    Task SaveChangesAsync();
}
