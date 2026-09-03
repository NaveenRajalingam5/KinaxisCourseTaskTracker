namespace KinaxisCourseTaskTracker.Models;

public enum EnrollmentStatus
{
    Enrolled,
    Completed,
    Paused,
    Cancelled
}

public class Enrollment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Enrolled;
    public DateTime? CompletedAt { get; set; }
}
