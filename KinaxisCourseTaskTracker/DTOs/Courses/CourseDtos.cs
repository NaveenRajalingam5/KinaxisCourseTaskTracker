using System.ComponentModel.DataAnnotations;

namespace KinaxisCourseTaskTracker.DTOs.Courses;

public class CourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string? Author1 { get; set; } = string.Empty;
    public string? Author2 { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int LessonCount { get; set; }
    public int TaskCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCourseDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Level { get; set; } = string.Empty;

    public int DurationMinutes { get; set; }

    public string? Author1 { get; set; } = string.Empty;

    public string? Author2 { get; set; } = string.Empty;
}

public class UpdateCourseDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Level { get; set; } = string.Empty;

    public int DurationMinutes { get; set; }

    public string? Author1 { get; set; } = string.Empty;

    public string? Author2 { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
