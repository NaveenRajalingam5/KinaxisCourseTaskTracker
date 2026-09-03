namespace KinaxisCourseTaskTracker.DTOs.Enrollment;

public class EnrollmentDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string CourseDescription { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string? Author1 { get; set; } = string.Empty;
    public string? Author2 { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
    public string Status { get; set; } = "Enrolled";
    public DateTime? CompletedAt { get; set; }
    public double ProgressPercentage { get; set; }
    public int CompletedLessons { get; set; }
    public int TotalLessons { get; set; }
}

public class CourseDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string? Author1 { get; set; } = string.Empty;
    public string? Author2 { get; set; } = string.Empty;
    public bool IsEnrolled { get; set; }
    public string EnrollmentStatus { get; set; } = "NotEnrolled";
    public double ProgressPercentage { get; set; }
    public int CompletedLessonsCount { get; set; }
    public int TotalLessonsCount { get; set; }
    public int CompletedTasksCount { get; set; }
    public int TotalTasksCount { get; set; }
}
