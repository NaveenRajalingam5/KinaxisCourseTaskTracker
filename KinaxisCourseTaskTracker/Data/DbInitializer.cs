using KinaxisCourseTaskTracker.Helpers;
using KinaxisCourseTaskTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace KinaxisCourseTaskTracker.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Users.AnyAsync())
        {
            return; // DB has been seeded already
        }

        // 1. Seed Admin
        var admin = new User
        {
            Name = "System Admin",
            Email = "admin@supplychainz.in",
            PasswordHash = PasswordHasher.HashPassword("AdminPass123!"),
            Department = "IT Operations",
            Role = UserRole.Admin,
            Status = UserStatus.Active,
            EmailVerified = true,
            CreatedAt = DateTime.UtcNow
        };
        context.Users.Add(admin);

        // 2. Seed Training Mentor
        var mentor = new User
        {
            Name = "Sarah Mentor",
            Email = "mentor@supplychainz.in",
            PasswordHash = PasswordHasher.HashPassword("MentorPass123!"),
            Department = "Global Training",
            Role = UserRole.TrainingMentor,
            Status = UserStatus.Active,
            EmailVerified = true,
            CreatedAt = DateTime.UtcNow
        };
        context.Users.Add(mentor);

        // 3. Seed Learners
        var learner1 = new User
        {
            Name = "John Learner",
            Email = "john.learner@supplychainz.in",
            PasswordHash = PasswordHasher.HashPassword("LearnerPass123!"),
            Department = "Supply Chain Engineering",
            Role = UserRole.Learner,
            Status = UserStatus.Active,
            EmailVerified = true,
            CreatedAt = DateTime.UtcNow
        };

        var learner2 = new User
        {
            Name = "Jane Learner",
            Email = "jane.learner@supplychainz.in",
            PasswordHash = PasswordHasher.HashPassword("LearnerPass123!"),
            Department = "Logistics Operations",
            Role = UserRole.Learner,
            Status = UserStatus.Active,
            EmailVerified = true,
            CreatedAt = DateTime.UtcNow
        };
        context.Users.AddRange(learner1, learner2);

        await context.SaveChangesAsync();

        // 4. Seed Course, Lessons, Topics, and Tasks
        var course1 = new Course
        {
            Title = "Kinaxis RapidResponse Fundamentals",
            Description = "Master core concepts of Kinaxis RapidResponse, concurrent planning, and supply chain scenario modeling.",
            Category = "Supply Chain Management",
            Level = "Beginner",
            DurationMinutes = 180,
            Author1 = "Kinaxis Lead Architect",
            Author2 = "Supply Chain Specialist",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        context.Courses.Add(course1);
        await context.SaveChangesAsync();

        var lesson1 = new Lesson
        {
            CourseId = course1.Id,
            Title = "Lesson 1: Introduction to RapidResponse & Concurrent Planning",
            Description = "Learn the foundational architecture of Kinaxis RapidResponse and multi-site planning.",
            Order = 1
        };

        context.Lessons.Add(lesson1);
        await context.SaveChangesAsync();

        var topic1 = new Topic
        {
            LessonId = lesson1.Id,
            Title = "Topic 1.1: Concurrent Planning Overview",
            Description = "Introduction to real-time synchronization between supply and demand nodes.",
            Order = 1,
            DurationMinutes = 20
        };

        var topic2 = new Topic
        {
            LessonId = lesson1.Id,
            Title = "Topic 1.2: Architecture and User Interface",
            Description = "Navigating workbooks, data tables, and user access levels in RapidResponse.",
            Order = 2,
            DurationMinutes = 25
        };

        context.Topics.AddRange(topic1, topic2);

        var lesson2 = new Lesson
        {
            CourseId = course1.Id,
            Title = "Lesson 2: Supply Chain Data Modeling & Table Operations",
            Description = "Understand supply chain data tables, demand filtering, and part relationships.",
            Order = 2
        };
        context.Lessons.Add(lesson2);
        await context.SaveChangesAsync();

        var topic3 = new Topic
        {
            LessonId = lesson2.Id,
            Title = "Topic 2.1: Part Master & Bill of Materials",
            Description = "Exploring parts, suppliers, lead times, and parent-child part hierarchies.",
            Order = 1,
            DurationMinutes = 30
        };

        var topic4 = new Topic
        {
            LessonId = lesson2.Id,
            Title = "Topic 2.2: Demand and Forecast Tables",
            Description = "Understanding sales orders, historical shipments, and statistical forecast series.",
            Order = 2,
            DurationMinutes = 35
        };

        context.Topics.AddRange(topic3, topic4);

        var task1 = new CourseTask
        {
            CourseId = course1.Id,
            LessonId = lesson1.Id,
            Title = "Task 1: Build a Supply Scenario Simulation",
            Description = "Create a what-if scenario in RapidResponse to simulate a material shortage impact.",
            DueDate = DateTime.UtcNow.AddDays(7),
            Priority = TaskPriority.High,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        context.Tasks.Add(task1);

        await context.SaveChangesAsync();

        Console.WriteLine("[DATABASE SEEDED SUCCESSFULLY] Seeded Admin, Mentor, 2 Learners, Sample Course with Authors, Lessons, Topics, and Tasks.");
    }
}
