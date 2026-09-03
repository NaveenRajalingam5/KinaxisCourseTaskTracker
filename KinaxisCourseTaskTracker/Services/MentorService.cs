using KinaxisCourseTaskTracker.DTOs.Mentor;
using KinaxisCourseTaskTracker.Interfaces;
using KinaxisCourseTaskTracker.Models;
using KinaxisCourseTaskTracker.Repositories.Interfaces;

namespace KinaxisCourseTaskTracker.Services;

public class MentorService : IMentorService
{
    private readonly IMentorRepository _mentorRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ITaskRepository _taskRepository;

    public MentorService(
        IMentorRepository mentorRepository,
        IUserRepository userRepository,
        IEnrollmentRepository enrollmentRepository,
        ITaskRepository taskRepository)
    {
        _mentorRepository = mentorRepository;
        _userRepository = userRepository;
        _enrollmentRepository = enrollmentRepository;
        _taskRepository = taskRepository;
    }

    public async Task<IEnumerable<LearnerSummaryDto>> GetAllLearnersAsync()
    {
        var learners = await _userRepository.GetAllLearnersAsync();

        var result = new List<LearnerSummaryDto>();

        foreach (var learner in learners)
        {
            var enrollments = (await _enrollmentRepository.GetUserEnrollmentsAsync(learner.Id)).ToList();

            int activeCourses = enrollments.Count(e => e.Status == EnrollmentStatus.Enrolled);
            int completedCourses = enrollments.Count(e => e.Status == EnrollmentStatus.Completed);

            int totalLessons = 0;
            int completedLessons = 0;

            foreach (var e in enrollments)
            {
                var lessonIds = e.Course.Lessons.Select(l => l.Id).ToList();
                totalLessons += lessonIds.Count;
                completedLessons += await _enrollmentRepository.GetCompletedLessonsCountAsync(learner.Id, lessonIds);
            }

            double overallProgress = totalLessons > 0 ? Math.Round((double)completedLessons / totalLessons * 100, 2) : 0;

            var taskProgresses = (await _taskRepository.GetUserTaskProgressesAsync(learner.Id)).ToList();

            int completedTasks = taskProgresses.Count(tp => tp.Status == TaskExecutionStatus.Completed);
            int lessonMinutes = await _enrollmentRepository.GetTotalLessonTimeMinutesAsync(learner.Id);
            int taskMinutes = await _taskRepository.GetTotalTaskTimeMinutesAsync(learner.Id);

            result.Add(new LearnerSummaryDto
            {
                LearnerId = learner.Id,
                Name = learner.Name,
                Email = learner.Email,
                Department = learner.Department,
                Status = learner.Status.ToString(),
                ActiveCoursesCount = activeCourses,
                CompletedCoursesCount = completedCourses,
                OverallCourseProgressPercentage = overallProgress,
                TotalTasksAssigned = taskProgresses.Count,
                CompletedTasksCount = completedTasks,
                TotalLearningTimeMinutes = lessonMinutes + taskMinutes
            });
        }

        return result;
    }

    public async Task<LearnerOverviewDto?> GetLearnerOverviewAsync(int learnerId)
    {
        var learner = await _userRepository.GetByIdAsync(learnerId);

        if (learner == null || learner.Role != UserRole.Learner) return null;

        var enrollments = (await _enrollmentRepository.GetUserEnrollmentsAsync(learnerId)).ToList();
        var taskProgresses = (await _taskRepository.GetUserTaskProgressesAsync(learnerId)).ToList();

        int lessonMinutes = await _enrollmentRepository.GetTotalLessonTimeMinutesAsync(learnerId);
        int taskMinutes = await _taskRepository.GetTotalTaskTimeMinutesAsync(learnerId);

        int pendingTasks = taskProgresses.Count(tp => tp.Status != TaskExecutionStatus.Completed);
        int overdueTasks = taskProgresses.Count(tp => tp.Task.DueDate.HasValue && tp.Task.DueDate.Value < DateTime.UtcNow && tp.Status != TaskExecutionStatus.Completed);

        return new LearnerOverviewDto
        {
            LearnerId = learner.Id,
            Name = learner.Name,
            Email = learner.Email,
            Department = learner.Department,
            Status = learner.Status.ToString(),
            CreatedAt = learner.CreatedAt,
            TotalCoursesEnrolled = enrollments.Count,
            ActiveCoursesCount = enrollments.Count(e => e.Status == EnrollmentStatus.Enrolled),
            CompletedCoursesCount = enrollments.Count(e => e.Status == EnrollmentStatus.Completed),
            PendingTasksCount = pendingTasks,
            OverdueTasksCount = overdueTasks,
            TotalLearningTimeMinutes = lessonMinutes + taskMinutes
        };
    }

    public async Task<IEnumerable<LearnerCourseProgressDto>> GetLearnerCoursesAsync(int learnerId)
    {
        var enrollments = await _enrollmentRepository.GetUserEnrollmentsAsync(learnerId);

        var result = new List<LearnerCourseProgressDto>();

        foreach (var e in enrollments)
        {
            var lessonIds = e.Course.Lessons.Select(l => l.Id).ToList();
            int completedLessons = await _enrollmentRepository.GetCompletedLessonsCountAsync(learnerId, lessonIds);

            double progress = lessonIds.Count > 0 ? Math.Round((double)completedLessons / lessonIds.Count * 100, 2) : 0;

            result.Add(new LearnerCourseProgressDto
            {
                CourseId = e.CourseId,
                CourseTitle = e.Course.Title,
                Category = e.Course.Category,
                Level = e.Course.Level,
                Author1 = e.Course.Author1,
                Author2 = e.Course.Author2,
                EnrolledAt = e.EnrolledAt,
                Status = e.Status.ToString(),
                CompletedAt = e.CompletedAt,
                TotalLessons = lessonIds.Count,
                CompletedLessons = completedLessons,
                ProgressPercentage = progress
            });
        }

        return result;
    }

    public async Task<IEnumerable<LearnerTaskProgressDto>> GetLearnerTasksAsync(int learnerId)
    {
        var taskProgresses = await _taskRepository.GetUserTaskProgressesAsync(learnerId);

        return taskProgresses.Select(tp => new LearnerTaskProgressDto
        {
            TaskId = tp.TaskId,
            TaskTitle = tp.Task.Title,
            CourseId = tp.Task.CourseId,
            CourseTitle = tp.Task.Course != null ? tp.Task.Course.Title : string.Empty,
            Priority = tp.Task.Priority.ToString(),
            DueDate = tp.Task.DueDate,
            Status = tp.Status.ToString(),
            IsOverdue = tp.Task.DueDate.HasValue && tp.Task.DueDate.Value < DateTime.UtcNow && tp.Status != TaskExecutionStatus.Completed,
            SubmittedAt = tp.SubmittedAt,
            CompletedAt = tp.CompletedAt,
            TimeSpentMinutes = tp.TimeSpentMinutes,
            SubmissionText = tp.SubmissionText
        });
    }

    public async Task<LearnerLearningTimeDto?> GetLearnerLearningTimeAsync(int learnerId)
    {
        var learner = await _userRepository.GetByIdAsync(learnerId);
        if (learner == null) return null;

        int lessonMinutes = await _enrollmentRepository.GetTotalLessonTimeMinutesAsync(learnerId);
        int taskMinutes = await _taskRepository.GetTotalTaskTimeMinutesAsync(learnerId);

        return new LearnerLearningTimeDto
        {
            LearnerId = learner.Id,
            LearnerName = learner.Name,
            LessonTimeMinutes = lessonMinutes,
            TaskTimeMinutes = taskMinutes,
            TotalTimeMinutes = lessonMinutes + taskMinutes
        };
    }

    public async Task<LearnerInsightsDto?> GetLearnerInsightsAsync(int learnerId)
    {
        var learner = await _userRepository.GetByIdAsync(learnerId);
        if (learner == null) return null;

        var enrollments = (await _enrollmentRepository.GetUserEnrollmentsAsync(learnerId)).ToList();
        var taskProgresses = (await _taskRepository.GetUserTaskProgressesAsync(learnerId)).ToList();

        int inProgressCourses = enrollments.Count(e => e.Status == EnrollmentStatus.Enrolled);
        int completedCourses = enrollments.Count(e => e.Status == EnrollmentStatus.Completed);
        int pendingTasks = taskProgresses.Count(tp => tp.Status != TaskExecutionStatus.Completed);
        int overdueTasks = taskProgresses.Count(tp => tp.Task.DueDate.HasValue && tp.Task.DueDate.Value < DateTime.UtcNow && tp.Status != TaskExecutionStatus.Completed);
        int delayedTasks = taskProgresses.Count(tp => tp.Status == TaskExecutionStatus.InProgress && tp.TimeSpentMinutes > 120);

        int lessonMinutes = await _enrollmentRepository.GetTotalLessonTimeMinutesAsync(learnerId);
        int taskMinutes = await _taskRepository.GetTotalTaskTimeMinutesAsync(learnerId);

        double completionPercentage = enrollments.Count > 0 ? Math.Round((double)completedCourses / enrollments.Count * 100, 2) : 0;

        var feedbackList = await _mentorRepository.GetLearnerFeedbackAsync(learnerId);
        var recentFeedback = feedbackList.Take(5).Select(f => new FeedbackDto
        {
            Id = f.Id,
            MentorId = f.MentorId,
            MentorName = f.Mentor != null ? f.Mentor.Name : "Mentor",
            LearnerId = f.LearnerId,
            LearnerName = learner.Name,
            CourseId = f.CourseId,
            CourseTitle = f.Course != null ? f.Course.Title : null,
            TaskId = f.TaskId,
            TaskTitle = f.Task != null ? f.Task.Title : null,
            Comments = f.Comments,
            CreatedAt = f.CreatedAt
        }).ToList();

        return new LearnerInsightsDto
        {
            LearnerId = learner.Id,
            LearnerName = learner.Name,
            InProgressCourses = inProgressCourses,
            CompletedCourses = completedCourses,
            PendingTasks = pendingTasks,
            OverdueTasks = overdueTasks,
            DelayedTasks = delayedTasks,
            TotalLearningTimeMinutes = lessonMinutes + taskMinutes,
            CompletionPercentage = completionPercentage,
            RecentFeedback = recentFeedback
        };
    }

    public async Task<FeedbackDto> ProvideFeedbackAsync(int mentorId, ProvideFeedbackDto feedbackDto)
    {
        var mentor = await _userRepository.GetByIdAsync(mentorId);
        var learner = await _userRepository.GetByIdAsync(feedbackDto.LearnerId);

        if (learner == null)
        {
            throw new InvalidOperationException("Target learner not found.");
        }

        var feedback = new Feedback
        {
            MentorId = mentorId,
            LearnerId = feedbackDto.LearnerId,
            CourseId = feedbackDto.CourseId,
            TaskId = feedbackDto.TaskId,
            Comments = feedbackDto.Comments,
            CreatedAt = DateTime.UtcNow
        };

        await _mentorRepository.AddFeedbackAsync(feedback);
        await _mentorRepository.SaveChangesAsync();

        return new FeedbackDto
        {
            Id = feedback.Id,
            MentorId = mentorId,
            MentorName = mentor?.Name ?? "Mentor",
            LearnerId = learner.Id,
            LearnerName = learner.Name,
            CourseId = feedback.CourseId,
            TaskId = feedback.TaskId,
            Comments = feedback.Comments,
            CreatedAt = feedback.CreatedAt
        };
    }

    public async Task<bool> AssignMentorToLearnerAsync(AssignMentorDto assignDto)
    {
        var mentor = await _userRepository.GetByIdAsync(assignDto.MentorId);
        if (mentor == null || mentor.Role != UserRole.TrainingMentor)
        {
            throw new InvalidOperationException("Invalid Training Mentor user ID.");
        }

        var learner = await _userRepository.GetByIdAsync(assignDto.LearnerId);
        if (learner == null || learner.Role != UserRole.Learner)
        {
            throw new InvalidOperationException("Invalid Learner user ID.");
        }

        var assignment = new MentorAssignment
        {
            MentorId = assignDto.MentorId,
            LearnerId = assignDto.LearnerId,
            AssignedAt = DateTime.UtcNow
        };

        await _mentorRepository.AddAssignmentAsync(assignment);
        await _mentorRepository.SaveChangesAsync();
        return true;
    }
}
