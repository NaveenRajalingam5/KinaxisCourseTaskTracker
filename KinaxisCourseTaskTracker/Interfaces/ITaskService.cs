using KinaxisCourseTaskTracker.DTOs.Tasks;

namespace KinaxisCourseTaskTracker.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskDto>> GetCourseTasksAsync(int courseId, int? userId = null);
    Task<TaskDto?> GetTaskByIdAsync(int taskId, int? userId = null);
    Task<TaskDto> CreateTaskAsync(CreateTaskDto createDto);
    Task<TaskDto?> UpdateTaskAsync(int taskId, UpdateTaskDto updateDto);
    Task<bool> DeleteTaskAsync(int taskId);
    Task<TaskDto?> StartTaskAsync(int userId, int taskId);
    Task<TaskDto?> UpdateTaskProgressAsync(int userId, int taskId, int timeSpentMinutes);
    Task<TaskDto?> SubmitTaskAsync(int userId, int taskId, SubmitTaskDto submitDto, string? filePath = null);
    Task<TaskDto?> CompleteTaskAsync(int userId, int taskId);
    Task<IEnumerable<TaskDto>> GetLearnerTaskProgressAsync(int userId);
}
