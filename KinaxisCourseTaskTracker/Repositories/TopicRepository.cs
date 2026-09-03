using KinaxisCourseTaskTracker.Data;
using KinaxisCourseTaskTracker.Models;
using KinaxisCourseTaskTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KinaxisCourseTaskTracker.Repositories;

public class TopicRepository : ITopicRepository
{
    private readonly ApplicationDbContext _context;

    public TopicRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Topic>> GetTopicsByLessonIdAsync(int lessonId)
    {
        return await _context.Topics
            .Where(t => t.LessonId == lessonId)
            .OrderBy(t => t.Order)
            .ToListAsync();
    }

    public async Task<Topic?> GetByIdAsync(int id)
    {
        return await _context.Topics.FindAsync(id);
    }

    public async Task AddAsync(Topic topic)
    {
        await _context.Topics.AddAsync(topic);
    }

    public Task UpdateAsync(Topic topic)
    {
        _context.Topics.Update(topic);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Topic topic)
    {
        _context.Topics.Remove(topic);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
