using System.Security.Claims;
using KinaxisCourseTaskTracker.DTOs.Lessons;
using KinaxisCourseTaskTracker.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinaxisCourseTaskTracker.Controllers;

[ApiController]
[Authorize]
public class LessonsController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonsController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    [HttpGet("api/courses/{courseId}/lessons")]
    [Authorize(Roles = "Admin,Learner,TrainingMentor")]
    public async Task<ActionResult<IEnumerable<LessonDto>>> GetCourseLessons(int courseId)
    {
        int? userId = GetCurrentUserId();
        var lessons = await _lessonService.GetCourseLessonsAsync(courseId, userId);
        return Ok(lessons);
    }

    [HttpGet("api/lessons/{id}")]
    [Authorize(Roles = "Admin,Learner,TrainingMentor")]
    public async Task<ActionResult<LessonDto>> GetLessonById(int id)
    {
        int? userId = GetCurrentUserId();
        var lesson = await _lessonService.GetLessonByIdAsync(id, userId);
        if (lesson == null) return NotFound(new { message = "Lesson not found." });
        return Ok(lesson);
    }

    [HttpPost("api/lessons")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<LessonDto>> CreateLesson([FromBody] CreateLessonDto createDto)
    {
        try
        {
            var lesson = await _lessonService.CreateLessonAsync(createDto);
            return CreatedAtAction(nameof(GetLessonById), new { id = lesson.Id }, lesson);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("api/lessons/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<LessonDto>> UpdateLesson(int id, [FromBody] UpdateLessonDto updateDto)
    {
        var lesson = await _lessonService.UpdateLessonAsync(id, updateDto);
        if (lesson == null) return NotFound(new { message = "Lesson not found." });
        return Ok(lesson);
    }

    [HttpDelete("api/lessons/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteLesson(int id)
    {
        var success = await _lessonService.DeleteLessonAsync(id);
        if (!success) return NotFound(new { message = "Lesson not found." });
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
