namespace KinaxisCourseTaskTracker.Models;

public enum UserRole
{
    Admin,
    Learner,
    TrainingMentor
}

public enum UserStatus
{
    Invited,
    Active,
    Deactivated
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Learner;
    public UserStatus Status { get; set; } = UserStatus.Invited;
    public bool EmailVerified { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<EmailVerificationToken> VerificationTokens { get; set; } = new List<EmailVerificationToken>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
    public ICollection<TaskProgress> TaskProgresses { get; set; } = new List<TaskProgress>();
}
