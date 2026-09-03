using KinaxisCourseTaskTracker.DTOs.Tasks;
using KinaxisCourseTaskTracker.Interfaces;
using KinaxisCourseTaskTracker.Models;
using KinaxisCourseTaskTracker.Repositories.Interfaces;

namespace KinaxisCourseTaskTracker.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public TaskService(
        ITaskRepository taskRepository,
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository)
    {
        _taskRepository = taskRepository;
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<IEnumerable<TaskDto>> GetCourseTasksAsync(int courseId, int? userId = null)
    {
        var tasks = await _taskRepository.GetTasksByCourseIdAsync(courseId);

        var result = new List<TaskDto>();

        foreach (var task in tasks)
        {
            string status = "NotStarted";
            DateTime? startedAt = null;
            DateTime? submittedAt = null;
            DateTime? completedAt = null;
            int timeSpent = 0;
            string? subText = null;
            string? subFilePath = null;
            bool isUnlocked = true;

            if (userId.HasValue)
            {
                var isEnrolled = await _enrollmentRepository.IsUserEnrolledAsync(userId.Value, courseId);
                if (!isEnrolled)
                {
                    isUnlocked = false;
                }
                else
                {
                    var progress = await _taskRepository.GetTaskProgressAsync(userId.Value, task.Id);

                    if (progress != null)
                    {
                        status = progress.Status.ToString();
                        startedAt = progress.StartedAt;
                        submittedAt = progress.SubmittedAt;
                        completedAt = progress.CompletedAt;
                        timeSpent = progress.TimeSpentMinutes;
                        subText = progress.SubmissionText;
                        subFilePath = progress.SubmissionFilePath;
                    }

                    if (task.PrerequisiteTaskId.HasValue)
                    {
                        var prereqProgress = await _taskRepository.GetTaskProgressAsync(userId.Value, task.PrerequisiteTaskId.Value);
                        if (prereqProgress == null || prereqProgress.Status != TaskExecutionStatus.Completed)
                        {
                            isUnlocked = false;
                        }
                    }
                }
            }

            result.Add(new TaskDto
            {
                Id = task.Id,
                CourseId = task.CourseId,
                LessonId = task.LessonId,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority.ToString(),
                IsActive = task.IsActive,
                PrerequisiteTaskId = task.PrerequisiteTaskId,
                PrerequisiteTaskTitle = task.PrerequisiteTask?.Title,
                Status = status,
                IsUnlocked = isUnlocked,
                StartedAt = startedAt,
                SubmittedAt = submittedAt,
                CompletedAt = completedAt,
                TimeSpentMinutes = timeSpent,
                SubmissionText = subText,
                SubmissionFilePath = subFilePath
            });
        }

        return result;
    }

    public async Task<TaskDto?> GetTaskByIdAsync(int taskId, int? userId = null)
    {
        var task = await _taskRepository.GetTaskByIdAsync(taskId);
        if (task == null) return null;

        var tasksList = await GetCourseTasksAsync(task.CourseId, userId);
        return tasksList.FirstOrDefault(t => t.Id == taskId);
    }

    public async Task<TaskDto> CreateTaskAsync(CreateTaskDto createDto)
    {
        var courseExists = await _courseRepository.ExistsAsync(createDto.CourseId);
        if (!courseExists)
        {
            throw new InvalidOperationException("Associated course does not exist.");
        }

        var task = new CourseTask
        {
            CourseId = createDto.CourseId,
            LessonId = createDto.LessonId,
            Title = createDto.Title,
            Description = createDto.Description,
            DueDate = createDto.DueDate,
            Priority = createDto.Priority,
            PrerequisiteTaskId = createDto.PrerequisiteTaskId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _taskRepository.AddTaskAsync(task);
        await _taskRepository.SaveChangesAsync();

        return new TaskDto
        {
            Id = task.Id,
            CourseId = task.CourseId,
            LessonId = task.LessonId,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Priority = task.Priority.ToString(),
            IsActive = task.IsActive,
            PrerequisiteTaskId = task.PrerequisiteTaskId,
            Status = "NotStarted",
            IsUnlocked = true
        };
    }

    public async Task<TaskDto?> UpdateTaskAsync(int taskId, UpdateTaskDto updateDto)
    {
        var task = await _taskRepository.GetTaskByIdAsync(taskId);
        if (task == null) return null;

        task.Title = updateDto.Title;
        task.Description = updateDto.Description;
        task.DueDate = updateDto.DueDate;
        task.Priority = updateDto.Priority;
        task.PrerequisiteTaskId = updateDto.PrerequisiteTaskId;
        task.IsActive = updateDto.IsActive;

        await _taskRepository.SaveChangesAsync();
        return await GetTaskByIdAsync(taskId);
    }

    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        var task = await _taskRepository.GetTaskByIdAsync(taskId);
        if (task == null) return false;

        task.IsActive = false;
        await _taskRepository.SaveChangesAsync();
        return true;
    }

    public async Task<TaskDto?> StartTaskAsync(int userId, int taskId)
    {
        var task = await _taskRepository.GetTaskByIdAsync(taskId);
        if (task == null || !task.IsActive)
        {
            throw new InvalidOperationException("Task not found or inactive.");
        }

        // Learner must be enrolled in the course
        var isEnrolled = await _enrollmentRepository.IsUserEnrolledAsync(userId, task.CourseId);
        if (!isEnrolled)
        {
            throw new InvalidOperationException("Learner must be enrolled in the course to start tasks.");
        }

        // Task prerequisite check
        if (task.PrerequisiteTaskId.HasValue)
        {
            var prereqProgress = await _taskRepository.GetTaskProgressAsync(userId, task.PrerequisiteTaskId.Value);
            if (prereqProgress == null || prereqProgress.Status != TaskExecutionStatus.Completed)
            {
                throw new InvalidOperationException("Cannot start task. Prerequisite task must be completed first.");
            }
        }

        var progress = await _taskRepository.GetTaskProgressAsync(userId, taskId);

        if (progress == null)
        {
            progress = new TaskProgress
            {
                UserId = userId,
                TaskId = taskId,
                Status = TaskExecutionStatus.InProgress,
                StartedAt = DateTime.UtcNow
            };
            await _taskRepository.AddTaskProgressAsync(progress);
        }
        else if (progress.Status == TaskExecutionStatus.NotStarted)
        {
            progress.Status = TaskExecutionStatus.InProgress;
            progress.StartedAt ??= DateTime.UtcNow;
        }

        await _taskRepository.SaveChangesAsync();
        return await GetTaskByIdAsync(taskId, userId);
    }

    public async Task<TaskDto?> UpdateTaskProgressAsync(int userId, int taskId, int timeSpentMinutes)
    {
        var progress = await _taskRepository.GetTaskProgressAsync(userId, taskId);

        if (progress == null)
        {
            throw new InvalidOperationException("Task progress not started yet. Call Start Task first.");
        }

        progress.TimeSpentMinutes += timeSpentMinutes;
        await _taskRepository.SaveChangesAsync();

        return await GetTaskByIdAsync(taskId, userId);
    }

    public async Task<TaskDto?> SubmitTaskAsync(int userId, int taskId, SubmitTaskDto submitDto, string? filePath = null)
    {
        var progress = await _taskRepository.GetTaskProgressAsync(userId, taskId);

        if (progress == null)
        {
            throw new InvalidOperationException("Task progress not started. Call Start Task before submitting.");
        }

        progress.Status = TaskExecutionStatus.Submitted;
        progress.SubmittedAt = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(submitDto.SubmissionText))
        {
            progress.SubmissionText = submitDto.SubmissionText;
        }
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            progress.SubmissionFilePath = filePath;
        }
        if (submitDto.TimeSpentMinutes > 0)
        {
            progress.TimeSpentMinutes += submitDto.TimeSpentMinutes;
        }

        await _taskRepository.SaveChangesAsync();
        return await GetTaskByIdAsync(taskId, userId);
    }

    public async Task<TaskDto?> CompleteTaskAsync(int userId, int taskId)
    {
        var progress = await _taskRepository.GetTaskProgressAsync(userId, taskId);

        if (progress == null || progress.Status != TaskExecutionStatus.Submitted)
        {
            throw new InvalidOperationException("Task must be submitted before it can be marked as completed.");
        }

        progress.Status = TaskExecutionStatus.Completed;
        progress.CompletedAt = DateTime.UtcNow;

        await _taskRepository.SaveChangesAsync();
        return await GetTaskByIdAsync(taskId, userId);
    }

    public async Task<IEnumerable<TaskDto>> GetLearnerTaskProgressAsync(int userId)
    {
        var taskProgresses = await _taskRepository.GetUserTaskProgressesAsync(userId);

        return taskProgresses.Select(tp => new TaskDto
        {
            Id = tp.Task.Id,
            CourseId = tp.Task.CourseId,
            LessonId = tp.Task.LessonId,
            Title = tp.Task.Title,
            Description = tp.Task.Description,
            DueDate = tp.Task.DueDate,
            Priority = tp.Task.Priority.ToString(),
            IsActive = tp.Task.IsActive,
            PrerequisiteTaskId = tp.Task.PrerequisiteTaskId,
            PrerequisiteTaskTitle = tp.Task.PrerequisiteTask?.Title,
            Status = tp.Status.ToString(),
            IsUnlocked = true,
            StartedAt = tp.StartedAt,
            SubmittedAt = tp.SubmittedAt,
            CompletedAt = tp.CompletedAt,
            TimeSpentMinutes = tp.TimeSpentMinutes,
            SubmissionText = tp.SubmissionText,
            SubmissionFilePath = tp.SubmissionFilePath
        });
    }
}
