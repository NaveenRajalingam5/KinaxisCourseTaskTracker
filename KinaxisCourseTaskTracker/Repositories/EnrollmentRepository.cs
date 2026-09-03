using KinaxisCourseTaskTracker.Data;
using KinaxisCourseTaskTracker.Models;
using KinaxisCourseTaskTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KinaxisCourseTaskTracker.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly ApplicationDbContext _context;

    public EnrollmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Enrollment?> GetEnrollmentAsync(int userId, int courseId)
    {
        return await _context.Enrollments
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);
    }

    public async Task<IEnumerable<Enrollment>> GetUserEnrollmentsAsync(int userId)
    {
        return await _context.Enrollments
            .Where(e => e.UserId == userId)
            .Include(e => e.Course)
                .ThenInclude(c => c.Lessons)
            .ToListAsync();
    }

    public async Task<bool> IsUserEnrolledAsync(int userId, int courseId)
    {
        return await _context.Enrollments
            .AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
    }

    public async Task AddEnrollmentAsync(Enrollment enrollment)
    {
        await _context.Enrollments.AddAsync(enrollment);
    }

    public async Task<LessonProgress?> GetLessonProgressAsync(int userId, int lessonId)
    {
        return await _context.LessonProgresses
            .FirstOrDefaultAsync(lp => lp.UserId == userId && lp.LessonId == lessonId);
    }

    public async Task AddLessonProgressAsync(LessonProgress progress)
    {
        await _context.LessonProgresses.AddAsync(progress);
    }

    public async Task<int> GetCompletedLessonsCountAsync(int userId, IEnumerable<int> lessonIds)
    {
        return await _context.LessonProgresses
            .CountAsync(lp => lp.UserId == userId &&
                              lessonIds.Contains(lp.LessonId) &&
                              lp.Status == LessonStatus.Completed);
    }

    public async Task<int> GetTotalLessonTimeMinutesAsync(int userId)
    {
        return await _context.LessonProgresses
            .Where(lp => lp.UserId == userId)
            .SumAsync(lp => lp.TimeSpentMinutes);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
