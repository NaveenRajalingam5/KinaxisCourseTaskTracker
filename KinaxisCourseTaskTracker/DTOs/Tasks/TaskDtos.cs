using System.ComponentModel.DataAnnotations;
using KinaxisCourseTaskTracker.Models;

namespace KinaxisCourseTaskTracker.DTOs.Tasks;

public class TaskDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public int? LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public string Priority { get; set; } = "Medium";
    public bool IsActive { get; set; }
    public int? PrerequisiteTaskId { get; set; }
    public string? PrerequisiteTaskTitle { get; set; }
    public string Status { get; set; } = "NotStarted";
    public bool IsUnlocked { get; set; } = true;
    public DateTime? SubmittedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TimeSpentMinutes { get; set; }
    public string? SubmissionText { get; set; }
    public string? SubmissionFilePath { get; set; }
}

public class CreateTaskDto
{
    [Required]
    public int CourseId { get; set; }

    public int? LessonId { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public int? PrerequisiteTaskId { get; set; }
}

public class UpdateTaskDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public int? PrerequisiteTaskId { get; set; }

    public bool IsActive { get; set; } = true;
}

public class SubmitTaskDto
{
    public string? SubmissionText { get; set; }
    public int TimeSpentMinutes { get; set; }
}

public class UpdateTaskProgressDto
{
    public int TimeSpentMinutes { get; set; }
}
