namespace KinaxisCourseTaskTracker.Models;

public enum TaskExecutionStatus
{
    NotStarted,
    InProgress,
    Submitted,
    Completed
}

public class TaskProgress
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public CourseTask Task { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public TaskExecutionStatus Status { get; set; } = TaskExecutionStatus.NotStarted;
    public DateTime? StartedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TimeSpentMinutes { get; set; } = 0;
    public string? SubmissionText { get; set; }
    public string? SubmissionFilePath { get; set; }
}
