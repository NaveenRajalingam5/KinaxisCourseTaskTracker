using KinaxisCourseTaskTracker.Data;
using KinaxisCourseTaskTracker.Models;
using KinaxisCourseTaskTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KinaxisCourseTaskTracker.Repositories;

public class MentorRepository : IMentorRepository
{
    private readonly ApplicationDbContext _context;

    public MentorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MentorAssignment>> GetAssignmentsByMentorIdAsync(int mentorId)
    {
        return await _context.MentorAssignments
            .Where(ma => ma.MentorId == mentorId)
            .Include(ma => ma.Learner)
            .ToListAsync();
    }

    public async Task<MentorAssignment?> GetAssignmentAsync(int mentorId, int learnerId)
    {
        return await _context.MentorAssignments
            .FirstOrDefaultAsync(ma => ma.MentorId == mentorId && ma.LearnerId == learnerId);
    }

    public async Task AddAssignmentAsync(MentorAssignment assignment)
    {
        await _context.MentorAssignments.AddAsync(assignment);
    }

    public async Task AddFeedbackAsync(Feedback feedback)
    {
        await _context.Feedbacks.AddAsync(feedback);
    }

    public async Task<IEnumerable<Feedback>> GetLearnerFeedbackAsync(int learnerId)
    {
        return await _context.Feedbacks
            .Where(f => f.LearnerId == learnerId)
            .Include(f => f.Mentor)
            .Include(f => f.Course)
            .Include(f => f.Task)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
