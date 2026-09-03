using KinaxisCourseTaskTracker.DTOs.Topics;
using KinaxisCourseTaskTracker.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinaxisCourseTaskTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TopicsController : ControllerBase
{
    private readonly ITopicService _topicService;

    public TopicsController(ITopicService topicService)
    {
        _topicService = topicService;
    }

    [HttpGet("lesson/{lessonId}")]
    [Authorize(Roles = "Admin,Learner,TrainingMentor")]
    public async Task<IActionResult> GetTopicsByLessonId(int lessonId)
    {
        var topics = await _topicService.GetTopicsByLessonIdAsync(lessonId);
        return Ok(topics);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Learner,TrainingMentor")]
    public async Task<IActionResult> GetTopicById(int id)
    {
        var topic = await _topicService.GetTopicByIdAsync(id);
        if (topic == null)
        {
            return NotFound(new { message = $"Topic with ID {id} not found." });
        }
        return Ok(topic);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateTopic([FromBody] CreateTopicDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var created = await _topicService.CreateTopicAsync(createDto);
            return CreatedAtAction(nameof(GetTopicById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateTopic(int id, [FromBody] UpdateTopicDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updated = await _topicService.UpdateTopicAsync(id, updateDto);
        if (updated == null)
        {
            return NotFound(new { message = $"Topic with ID {id} not found." });
        }

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTopic(int id)
    {
        var deleted = await _topicService.DeleteTopicAsync(id);
        if (!deleted)
        {
            return NotFound(new { message = $"Topic with ID {id} not found." });
        }

        return NoContent();
    }
}
