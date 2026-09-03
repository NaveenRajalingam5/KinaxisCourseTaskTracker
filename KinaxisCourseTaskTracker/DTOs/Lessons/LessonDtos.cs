using System.ComponentModel.DataAnnotations;
using KinaxisCourseTaskTracker.DTOs.Topics;

namespace KinaxisCourseTaskTracker.DTOs.Lessons;

public class LessonDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public string Status { get; set; } = "NotStarted";
    public bool IsUnlocked { get; set; } = true;
    public int TimeSpentMinutes { get; set; }
    public List<TopicDto> Topics { get; set; } = new();
}

public class CreateLessonDto
{
    [Required]
    public int CourseId { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Order { get; set; }
}

public class UpdateLessonDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Order { get; set; }
}

public class UpdateLessonProgressDto
{
    public int TimeSpentMinutes { get; set; }
}
