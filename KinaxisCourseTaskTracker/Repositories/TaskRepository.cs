using KinaxisCourseTaskTracker.Data;
using KinaxisCourseTaskTracker.Models;
using KinaxisCourseTaskTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KinaxisCourseTaskTracker.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;

    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CourseTask>> GetTasksByCourseIdAsync(int courseId)
    {
        return await _context.Tasks
            .Where(t => t.CourseId == courseId && t.IsActive)
            .Include(t => t.PrerequisiteTask)
            .ToListAsync();
    }

    public async Task<CourseTask?> GetTaskByIdAsync(int id)
    {
        return await _context.Tasks
            .Include(t => t.PrerequisiteTask)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<TaskProgress?> GetTaskProgressAsync(int userId, int taskId)
    {
        return await _context.TaskProgresses
            .FirstOrDefaultAsync(tp => tp.UserId == userId && tp.TaskId == taskId);
    }

    public async Task<IEnumerable<TaskProgress>> GetUserTaskProgressesAsync(int userId)
    {
        return await _context.TaskProgresses
            .Where(tp => tp.UserId == userId)
            .Include(tp => tp.Task)
            .ToListAsync();
    }

    public async Task AddTaskAsync(CourseTask task)
    {
        await _context.Tasks.AddAsync(task);
    }

    public async Task AddTaskProgressAsync(TaskProgress progress)
    {
        await _context.TaskProgresses.AddAsync(progress);
    }

    public Task DeleteTaskAsync(CourseTask task)
    {
        _context.Tasks.Remove(task);
        return Task.CompletedTask;
    }

    public async Task<int> GetCompletedTasksCountAsync(int userId, IEnumerable<int> taskIds)
    {
        return await _context.TaskProgresses
            .CountAsync(tp => tp.UserId == userId &&
                              taskIds.Contains(tp.TaskId) &&
                              tp.Status == TaskExecutionStatus.Completed);
    }

    public async Task<int> GetTotalTaskTimeMinutesAsync(int userId)
    {
        return await _context.TaskProgresses
            .Where(tp => tp.UserId == userId)
            .SumAsync(tp => tp.TimeSpentMinutes);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
