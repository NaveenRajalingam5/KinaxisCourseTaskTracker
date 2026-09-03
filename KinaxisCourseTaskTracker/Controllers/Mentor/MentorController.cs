using System.Security.Claims;
using KinaxisCourseTaskTracker.DTOs.Mentor;
using KinaxisCourseTaskTracker.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinaxisCourseTaskTracker.Controllers;

[ApiController]
[Route("api/mentor")]
[Authorize(Roles = "TrainingMentor,Admin")]
public class MentorController : ControllerBase
{
    private readonly IMentorService _mentorService;

    public MentorController(IMentorService mentorService)
    {
        _mentorService = mentorService;
    }

    [HttpGet("learners")]
    public async Task<ActionResult<IEnumerable<LearnerSummaryDto>>> GetAllLearners()
    {
        var learners = await _mentorService.GetAllLearnersAsync();
        return Ok(learners);
    }

    [HttpGet("learners/{learnerId}")]
    public async Task<ActionResult<LearnerOverviewDto>> GetLearnerOverview(int learnerId)
    {
        var overview = await _mentorService.GetLearnerOverviewAsync(learnerId);
        if (overview == null) return NotFound(new { message = "Learner not found." });
        return Ok(overview);
    }

    [HttpGet("learners/{learnerId}/courses")]
    public async Task<ActionResult<IEnumerable<LearnerCourseProgressDto>>> GetLearnerCourses(int learnerId)
    {
        var courses = await _mentorService.GetLearnerCoursesAsync(learnerId);
        return Ok(courses);
    }

    [HttpGet("learners/{learnerId}/tasks")]
    public async Task<ActionResult<IEnumerable<LearnerTaskProgressDto>>> GetLearnerTasks(int learnerId)
    {
        var tasks = await _mentorService.GetLearnerTasksAsync(learnerId);
        return Ok(tasks);
    }

    [HttpGet("learners/{learnerId}/learning-time")]
    public async Task<ActionResult<LearnerLearningTimeDto>> GetLearnerLearningTime(int learnerId)
    {
        var time = await _mentorService.GetLearnerLearningTimeAsync(learnerId);
        if (time == null) return NotFound(new { message = "Learner not found." });
        return Ok(time);
    }

    [HttpGet("learners/{learnerId}/insights")]
    public async Task<ActionResult<LearnerInsightsDto>> GetLearnerInsights(int learnerId)
    {
        var insights = await _mentorService.GetLearnerInsightsAsync(learnerId);
        if (insights == null) return NotFound(new { message = "Learner not found." });
        return Ok(insights);
    }

    [HttpPost("feedback")]
    public async Task<ActionResult<FeedbackDto>> ProvideFeedback([FromBody] ProvideFeedbackDto feedbackDto)
    {
        var mentorId = GetCurrentUserId();
        if (!mentorId.HasValue) return Unauthorized();

        try
        {
            var result = await _mentorService.ProvideFeedbackAsync(mentorId.Value, feedbackDto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("assign")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignMentor([FromBody] AssignMentorDto assignDto)
    {
        try
        {
            await _mentorService.AssignMentorToLearnerAsync(assignDto);
            return Ok(new { message = "Mentor assigned to learner successfully." });
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
