namespace KinaxisCourseTaskTracker.Models;

public class MentorAssignment
{
    public int Id { get; set; }
    public int MentorId { get; set; }
    public User Mentor { get; set; } = null!;
    public int LearnerId { get; set; }
    public User Learner { get; set; } = null!;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
