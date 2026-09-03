using KinaxisCourseTaskTracker.DTOs.Mentor;

namespace KinaxisCourseTaskTracker.Interfaces;

public interface IMentorService
{
    Task<IEnumerable<LearnerSummaryDto>> GetAllLearnersAsync();
    Task<LearnerOverviewDto?> GetLearnerOverviewAsync(int learnerId);
    Task<IEnumerable<LearnerCourseProgressDto>> GetLearnerCoursesAsync(int learnerId);
    Task<IEnumerable<LearnerTaskProgressDto>> GetLearnerTasksAsync(int learnerId);
    Task<LearnerLearningTimeDto?> GetLearnerLearningTimeAsync(int learnerId);
    Task<LearnerInsightsDto?> GetLearnerInsightsAsync(int learnerId);
    Task<FeedbackDto> ProvideFeedbackAsync(int mentorId, ProvideFeedbackDto feedbackDto);
    Task<bool> AssignMentorToLearnerAsync(AssignMentorDto assignDto);
}
