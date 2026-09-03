using System.Security.Claims;
using KinaxisCourseTaskTracker.DTOs.Tasks;
using KinaxisCourseTaskTracker.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinaxisCourseTaskTracker.Controllers;

[ApiController]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet("api/courses/{courseId}/tasks")]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetCourseTasks(int courseId)
    {
        int? userId = GetCurrentUserId();
        var tasks = await _taskService.GetCourseTasksAsync(courseId, userId);
        return Ok(tasks);
    }

    [HttpGet("api/tasks/{id}")]
    public async Task<ActionResult<TaskDto>> GetTaskById(int id)
    {
        int? userId = GetCurrentUserId();
        var task = await _taskService.GetTaskByIdAsync(id, userId);
        if (task == null) return NotFound(new { message = "Task not found." });
        return Ok(task);
    }

    [HttpPost("api/tasks")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto createDto)
    {
        try
        {
            var task = await _taskService.CreateTaskAsync(createDto);
            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("api/tasks/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TaskDto>> UpdateTask(int id, [FromBody] UpdateTaskDto updateDto)
    {
        var task = await _taskService.UpdateTaskAsync(id, updateDto);
        if (task == null) return NotFound(new { message = "Task not found." });
        return Ok(task);
    }

    [HttpDelete("api/tasks/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var success = await _taskService.DeleteTaskAsync(id);
        if (!success) return NotFound(new { message = "Task not found." });
        return NoContent();
    }

    private int? GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim != null && int.TryParse(claim.Value, out int id))
        {
            return id;
        }
        return null;
    }
}
