namespace KinaxisCourseTaskTracker.Models;

public enum LessonStatus
{
    NotStarted,
    InProgress,
    Completed
}

public class LessonProgress
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;
    public LessonStatus Status { get; set; } = LessonStatus.NotStarted;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TimeSpentMinutes { get; set; } = 0;
}
