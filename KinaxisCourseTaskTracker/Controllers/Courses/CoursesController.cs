using System.Security.Claims;
using KinaxisCourseTaskTracker.DTOs.Courses;
using KinaxisCourseTaskTracker.DTOs.Enrollment;
using KinaxisCourseTaskTracker.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinaxisCourseTaskTracker.Controllers;

[ApiController]
[Route("api/courses")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly IEnrollmentService _enrollmentService;

    public CoursesController(ICourseService courseService, IEnrollmentService enrollmentService)
    {
        _courseService = courseService;
        _enrollmentService = enrollmentService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Learner,TrainingMentor")]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetAvailableCourses()
    {
        var courses = await _courseService.GetAvailableCoursesAsync();
        return Ok(courses);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Learner,TrainingMentor")]
    public async Task<ActionResult<CourseDetailDto>> GetCourseById(int id)
    {
        int? userId = GetCurrentUserId();
        var course = await _courseService.GetCourseByIdAsync(id, userId);
        if (course == null) return NotFound(new { message = "Course not found." });
        return Ok(course);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CreateCourseDto createDto)
    {
        var course = await _courseService.CreateCourseAsync(createDto);
        return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, course);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CourseDto>> UpdateCourse(int id, [FromBody] UpdateCourseDto updateDto)
    {
        var course = await _courseService.UpdateCourseAsync(id, updateDto);
        if (course == null) return NotFound(new { message = "Course not found." });
        return Ok(course);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateCourse(int id)
    {
        var success = await _courseService.DeactivateCourseAsync(id);
        if (!success) return NotFound(new { message = "Course not found." });
        return NoContent();
    }

    [HttpPost("{courseId}/enroll")]
    [Authorize(Roles = "Learner")]
    public async Task<ActionResult<EnrollmentDto>> Enroll(int courseId)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        try
        {
            var enrollment = await _enrollmentService.EnrollAsync(userId.Value, courseId);
            return Ok(enrollment);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
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
