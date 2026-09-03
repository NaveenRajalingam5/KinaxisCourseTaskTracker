using KinaxisCourseTaskTracker.Models;

namespace KinaxisCourseTaskTracker.Repositories.Interfaces;

public interface ITaskRepository
{
    Task<IEnumerable<CourseTask>> GetTasksByCourseIdAsync(int courseId);
    Task<CourseTask?> GetTaskByIdAsync(int id);
    Task<TaskProgress?> GetTaskProgressAsync(int userId, int taskId);
    Task<IEnumerable<TaskProgress>> GetUserTaskProgressesAsync(int userId);
    Task AddTaskAsync(CourseTask task);
    Task AddTaskProgressAsync(TaskProgress progress);
    Task DeleteTaskAsync(CourseTask task);
    Task<int> GetCompletedTasksCountAsync(int userId, IEnumerable<int> taskIds);
    Task<int> GetTotalTaskTimeMinutesAsync(int userId);
    Task SaveChangesAsync();
}
