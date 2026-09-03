namespace KinaxisCourseTaskTracker.Models;

public enum TaskPriority
{
    Low,
    Medium,
    High
}

public class CourseTask
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public int? LessonId { get; set; }
    public Lesson? Lesson { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Prerequisite task (optional)
    public int? PrerequisiteTaskId { get; set; }
    public CourseTask? PrerequisiteTask { get; set; }

    public ICollection<TaskProgress> TaskProgresses { get; set; } = new List<TaskProgress>();
}
