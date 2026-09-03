namespace KinaxisCourseTaskTracker.Models;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public ICollection<CourseTask> Tasks { get; set; } = new List<CourseTask>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
