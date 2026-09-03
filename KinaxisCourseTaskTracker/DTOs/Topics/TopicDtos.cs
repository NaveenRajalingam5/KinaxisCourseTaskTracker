using System.ComponentModel.DataAnnotations;

namespace KinaxisCourseTaskTracker.DTOs.Topics;

public class TopicDto
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public int DurationMinutes { get; set; }
}

public class CreateTopicDto
{
    [Required]
    public int LessonId { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Order { get; set; }

    public int DurationMinutes { get; set; }
}

public class UpdateTopicDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Order { get; set; }

    public int DurationMinutes { get; set; }
}
