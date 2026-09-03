using System.Security.Claims;
using KinaxisCourseTaskTracker.DTOs.Enrollment;
using KinaxisCourseTaskTracker.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinaxisCourseTaskTracker.Controllers;

[ApiController]
[Route("api/learners/me/courses")]
[Authorize(Roles = "Learner")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetMyEnrolledCourses()
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        var enrollments = await _enrollmentService.GetLearnerEnrollmentsAsync(userId.Value);
        return Ok(enrollments);
    }

    [HttpGet("{courseId}")]
    public async Task<ActionResult<EnrollmentDto>> GetEnrollmentDetails(int courseId)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        var enrollment = await _enrollmentService.GetEnrollmentDetailsAsync(userId.Value, courseId);
        if (enrollment == null) return NotFound(new { message = "Enrollment not found for this course." });
        return Ok(enrollment);
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
