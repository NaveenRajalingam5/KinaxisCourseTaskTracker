using KinaxisCourseTaskTracker.Models;

namespace KinaxisCourseTaskTracker.Repositories.Interfaces;

public interface IMentorRepository
{
    Task<IEnumerable<MentorAssignment>> GetAssignmentsByMentorIdAsync(int mentorId);
    Task<MentorAssignment?> GetAssignmentAsync(int mentorId, int learnerId);
    Task AddAssignmentAsync(MentorAssignment assignment);
    Task AddFeedbackAsync(Feedback feedback);
    Task<IEnumerable<Feedback>> GetLearnerFeedbackAsync(int learnerId);
    Task SaveChangesAsync();
}
