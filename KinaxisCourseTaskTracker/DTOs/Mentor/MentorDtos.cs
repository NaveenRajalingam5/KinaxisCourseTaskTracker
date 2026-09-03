using System.ComponentModel.DataAnnotations;

namespace KinaxisCourseTaskTracker.DTOs.Mentor;

public class LearnerSummaryDto
{
    public int LearnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int ActiveCoursesCount { get; set; }
    public int CompletedCoursesCount { get; set; }
    public double OverallCourseProgressPercentage { get; set; }
    public int TotalTasksAssigned { get; set; }
    public int CompletedTasksCount { get; set; }
    public int TotalLearningTimeMinutes { get; set; }
}

public class LearnerOverviewDto
{
    public int LearnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int TotalCoursesEnrolled { get; set; }
    public int ActiveCoursesCount { get; set; }
    public int CompletedCoursesCount { get; set; }
    public int PendingTasksCount { get; set; }
    public int OverdueTasksCount { get; set; }
    public int TotalLearningTimeMinutes { get; set; }
}

public class LearnerCourseProgressDto
{
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string? Author1 { get; set; } = string.Empty;
    public string? Author2 { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? CompletedAt { get; set; }
    public int TotalLessons { get; set; }
    public int CompletedLessons { get; set; }
    public double ProgressPercentage { get; set; }
}

public class LearnerTaskProgressDto
{
    public int TaskId { get; set; }
    public string TaskTitle { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsOverdue { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TimeSpentMinutes { get; set; }
    public string? SubmissionText { get; set; }
}

public class LearnerLearningTimeDto
{
    public int LearnerId { get; set; }
    public string LearnerName { get; set; } = string.Empty;
    public int LessonTimeMinutes { get; set; }
    public int TaskTimeMinutes { get; set; }
    public int TotalTimeMinutes { get; set; }
}

public class LearnerInsightsDto
{
    public int LearnerId { get; set; }
    public string LearnerName { get; set; } = string.Empty;
    public int InProgressCourses { get; set; }
    public int CompletedCourses { get; set; }
    public int PendingTasks { get; set; }
    public int OverdueTasks { get; set; }
    public int DelayedTasks { get; set; }
    public int TotalLearningTimeMinutes { get; set; }
    public double CompletionPercentage { get; set; }
    public List<FeedbackDto> RecentFeedback { get; set; } = new();
}

public class ProvideFeedbackDto
{
    [Required]
    public int LearnerId { get; set; }

    public int? CourseId { get; set; }

    public int? TaskId { get; set; }

    [Required, MinLength(3)]
    public string Comments { get; set; } = string.Empty;
}

public class FeedbackDto
{
    public int Id { get; set; }
    public int MentorId { get; set; }
    public string MentorName { get; set; } = string.Empty;
    public int LearnerId { get; set; }
    public string LearnerName { get; set; } = string.Empty;
    public int? CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public int? TaskId { get; set; }
    public string? TaskTitle { get; set; }
    public string Comments { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class AssignMentorDto
{
    [Required]
    public int MentorId { get; set; }

    [Required]
    public int LearnerId { get; set; }
}
