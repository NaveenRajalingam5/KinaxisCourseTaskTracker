namespace KinaxisCourseTaskTracker.Models;

public class Feedback
{
    public int Id { get; set; }
    public int MentorId { get; set; }
    public User Mentor { get; set; } = null!;
    public int LearnerId { get; set; }
    public User Learner { get; set; } = null!;
    public int? CourseId { get; set; }
    public Course? Course { get; set; }
    public int? TaskId { get; set; }
    public CourseTask? Task { get; set; }
    public string Comments { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
