using System.Security.Claims;
using KinaxisCourseTaskTracker.DTOs.Lessons;
using KinaxisCourseTaskTracker.DTOs.Tasks;
using KinaxisCourseTaskTracker.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinaxisCourseTaskTracker.Controllers;

[ApiController]
[Route("api/progress")]
[Authorize(Roles = "Learner")]
public class ProgressController : ControllerBase
{
    private readonly ILessonService _lessonService;
    private readonly ITaskService _taskService;

    public ProgressController(ILessonService lessonService, ITaskService taskService)
    {
        _lessonService = lessonService;
        _taskService = taskService;
    }

    // --- LESSON PROGRESS ENDPOINTS ---

    [HttpPost("lessons/{lessonId}/start")]
    public async Task<ActionResult<LessonDto>> StartLesson(int lessonId)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        try
        {
            var result = await _lessonService.StartLessonAsync(userId.Value, lessonId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("lessons/{lessonId}/time")]
    public async Task<ActionResult<LessonDto>> UpdateLessonTime(int lessonId, [FromBody] UpdateLessonProgressDto dto)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        try
        {
            var result = await _lessonService.UpdateLessonProgressAsync(userId.Value, lessonId, dto.TimeSpentMinutes);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("lessons/{lessonId}/complete")]
    public async Task<ActionResult<LessonDto>> CompleteLesson(int lessonId)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        var result = await _lessonService.CompleteLessonAsync(userId.Value, lessonId);
        if (result == null) return NotFound(new { message = "Lesson not found." });
        return Ok(result);
    }

    // --- TASK PROGRESS ENDPOINTS ---

    [HttpPost("tasks/{taskId}/start")]
    public async Task<ActionResult<TaskDto>> StartTask(int taskId)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        try
        {
            var result = await _taskService.StartTaskAsync(userId.Value, taskId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("tasks/{taskId}/time")]
    public async Task<ActionResult<TaskDto>> UpdateTaskTime(int taskId, [FromBody] UpdateTaskProgressDto dto)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        try
        {
            var result = await _taskService.UpdateTaskProgressAsync(userId.Value, taskId, dto.TimeSpentMinutes);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("tasks/{taskId}/submit")]
    public async Task<ActionResult<TaskDto>> SubmitTask(int taskId, [FromForm] SubmitTaskDto submitDto, IFormFile? file)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        string? filePath = null;

        if (file != null && file.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            filePath = Path.Combine("Uploads", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }

        try
        {
            var result = await _taskService.SubmitTaskAsync(userId.Value, taskId, submitDto, filePath);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("tasks/{taskId}/complete")]
    public async Task<ActionResult<TaskDto>> CompleteTask(int taskId)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        try
        {
            var result = await _taskService.CompleteTaskAsync(userId.Value, taskId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("tasks/me")]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetMyTaskProgress()
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        var result = await _taskService.GetLearnerTaskProgressAsync(userId.Value);
        return Ok(result);
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
