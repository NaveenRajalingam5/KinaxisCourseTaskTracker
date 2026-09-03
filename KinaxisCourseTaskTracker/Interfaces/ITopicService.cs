using KinaxisCourseTaskTracker.DTOs.Topics;

namespace KinaxisCourseTaskTracker.Interfaces;

public interface ITopicService
{
    Task<IEnumerable<TopicDto>> GetTopicsByLessonIdAsync(int lessonId);
    Task<TopicDto?> GetTopicByIdAsync(int topicId);
    Task<TopicDto> CreateTopicAsync(CreateTopicDto createDto);
    Task<TopicDto?> UpdateTopicAsync(int topicId, UpdateTopicDto updateDto);
    Task<bool> DeleteTopicAsync(int topicId);
}
