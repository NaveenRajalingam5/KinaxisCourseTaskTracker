using KinaxisCourseTaskTracker.Data;
using KinaxisCourseTaskTracker.Models;
using KinaxisCourseTaskTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KinaxisCourseTaskTracker.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly ApplicationDbContext _context;

    public LessonRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Lesson>> GetLessonsByCourseIdAsync(int courseId)
    {
        return await _context.Lessons
            .Where(l => l.CourseId == courseId)
            .Include(l => l.Topics)
            .OrderBy(l => l.Order)
            .ToListAsync();
    }

    public async Task<Lesson?> GetByIdWithTopicsAsync(int id)
    {
        return await _context.Lessons
            .Include(l => l.Topics)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<Lesson?> GetByIdAsync(int id)
    {
        return await _context.Lessons.FindAsync(id);
    }

    public async Task<List<int>> GetLessonIdsByCourseIdAsync(int courseId)
    {
        return await _context.Lessons
            .Where(l => l.CourseId == courseId)
            .Select(l => l.Id)
            .ToListAsync();
    }

    public async Task AddAsync(Lesson lesson)
    {
        await _context.Lessons.AddAsync(lesson);
    }

    public Task UpdateAsync(Lesson lesson)
    {
        _context.Lessons.Update(lesson);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Lesson lesson)
    {
        _context.Lessons.Remove(lesson);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
