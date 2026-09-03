using KinaxisCourseTaskTracker.DTOs.Topics;
using KinaxisCourseTaskTracker.Interfaces;
using KinaxisCourseTaskTracker.Models;
using KinaxisCourseTaskTracker.Repositories.Interfaces;

namespace KinaxisCourseTaskTracker.Services;

public class TopicService : ITopicService
{
    private readonly ITopicRepository _topicRepository;
    private readonly ILessonRepository _lessonRepository;

    public TopicService(ITopicRepository topicRepository, ILessonRepository lessonRepository)
    {
        _topicRepository = topicRepository;
        _lessonRepository = lessonRepository;
    }

    public async Task<IEnumerable<TopicDto>> GetTopicsByLessonIdAsync(int lessonId)
    {
        var topics = await _topicRepository.GetTopicsByLessonIdAsync(lessonId);
        return topics.Select(MapToDto);
    }

    public async Task<TopicDto?> GetTopicByIdAsync(int topicId)
    {
        var topic = await _topicRepository.GetByIdAsync(topicId);
        return topic == null ? null : MapToDto(topic);
    }

    public async Task<TopicDto> CreateTopicAsync(CreateTopicDto createDto)
    {
        var lesson = await _lessonRepository.GetByIdAsync(createDto.LessonId);
        if (lesson == null)
        {
            throw new InvalidOperationException("Associated lesson does not exist.");
        }

        var topic = new Topic
        {
            LessonId = createDto.LessonId,
            Title = createDto.Title,
            Description = createDto.Description,
            Order = createDto.Order,
            DurationMinutes = createDto.DurationMinutes,
            CreatedAt = DateTime.UtcNow
        };

        await _topicRepository.AddAsync(topic);
        await _topicRepository.SaveChangesAsync();

        return MapToDto(topic);
    }

    public async Task<TopicDto?> UpdateTopicAsync(int topicId, UpdateTopicDto updateDto)
    {
        var topic = await _topicRepository.GetByIdAsync(topicId);
        if (topic == null) return null;

        topic.Title = updateDto.Title;
        topic.Description = updateDto.Description;
        topic.Order = updateDto.Order;
        topic.DurationMinutes = updateDto.DurationMinutes;

        await _topicRepository.UpdateAsync(topic);
        await _topicRepository.SaveChangesAsync();
        return MapToDto(topic);
    }

    public async Task<bool> DeleteTopicAsync(int topicId)
    {
        var topic = await _topicRepository.GetByIdAsync(topicId);
        if (topic == null) return false;

        await _topicRepository.DeleteAsync(topic);
        await _topicRepository.SaveChangesAsync();
        return true;
    }

    private static TopicDto MapToDto(Topic topic)
    {
        return new TopicDto
        {
            Id = topic.Id,
            LessonId = topic.LessonId,
            Title = topic.Title,
            Description = topic.Description,
            Order = topic.Order,
            DurationMinutes = topic.DurationMinutes,
            CreatedAt = topic.CreatedAt
        };
    }
}
