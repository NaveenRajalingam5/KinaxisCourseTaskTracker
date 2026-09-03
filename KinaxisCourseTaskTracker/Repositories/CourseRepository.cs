using KinaxisCourseTaskTracker.Data;
using KinaxisCourseTaskTracker.Models;
using KinaxisCourseTaskTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KinaxisCourseTaskTracker.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly ApplicationDbContext _context;

    public CourseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Course>> GetActiveCoursesAsync()
    {
        return await _context.Courses
            .Where(c => c.IsActive)
            .Include(c => c.Lessons)
            .Include(c => c.Tasks)
            .ToListAsync();
    }

    public async Task<Course?> GetByIdWithLessonsAndTasksAsync(int id)
    {
        return await _context.Courses
            .Include(c => c.Lessons)
            .Include(c => c.Tasks)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Course?> GetByIdAsync(int id)
    {
        return await _context.Courses.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Courses.AnyAsync(c => c.Id == id);
    }

    public async Task AddAsync(Course course)
    {
        await _context.Courses.AddAsync(course);
    }

    public Task UpdateAsync(Course course)
    {
        _context.Courses.Update(course);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
