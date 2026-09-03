using KinaxisCourseTaskTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace KinaxisCourseTaskTracker.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; } = null!;
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<Lesson> Lessons { get; set; } = null!;
    public DbSet<Enrollment> Enrollments { get; set; } = null!;
    public DbSet<LessonProgress> LessonProgresses { get; set; } = null!;
    public DbSet<CourseTask> Tasks { get; set; } = null!;
    public DbSet<Topic> Topics { get; set; } = null!;
    public DbSet<TaskProgress> TaskProgresses { get; set; } = null!;
    public DbSet<MentorAssignment> MentorAssignments { get; set; } = null!;
    public DbSet<Feedback> Feedbacks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Map CourseTask entity to "Tasks" table
        modelBuilder.Entity<CourseTask>().ToTable("Tasks");

        // User indexes & constraints
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

        modelBuilder.Entity<User>()
            .Property(u => u.Status)
            .HasConversion<string>();

        // Enrollment unique constraint: Learner cannot enroll twice in the same course
        modelBuilder.Entity<Enrollment>()
            .HasIndex(e => new { e.UserId, e.CourseId })
            .IsUnique();

        modelBuilder.Entity<Enrollment>()
            .Property(e => e.Status)
            .HasConversion<string>();

        // LessonProgress status enum conversion
        modelBuilder.Entity<LessonProgress>()
            .Property(lp => lp.Status)
            .HasConversion<string>();

        modelBuilder.Entity<LessonProgress>()
            .HasIndex(lp => new { lp.UserId, lp.LessonId })
            .IsUnique();

        // TaskProgress status enum conversion
        modelBuilder.Entity<TaskProgress>()
            .Property(tp => tp.Status)
            .HasConversion<string>();

        modelBuilder.Entity<TaskProgress>()
            .HasIndex(tp => new { tp.UserId, tp.TaskId })
            .IsUnique();

        // TaskPriority enum conversion
        modelBuilder.Entity<CourseTask>()
            .Property(t => t.Priority)
            .HasConversion<string>();

        // Configure foreign key delete behaviors to avoid SQL Server cascade cycle issues
        modelBuilder.Entity<LessonProgress>()
            .HasOne(lp => lp.User)
            .WithMany(u => u.LessonProgresses)
            .HasForeignKey(lp => lp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LessonProgress>()
            .HasOne(lp => lp.Lesson)
            .WithMany(l => l.LessonProgresses)
            .HasForeignKey(lp => lp.LessonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TaskProgress>()
            .HasOne(tp => tp.User)
            .WithMany(u => u.TaskProgresses)
            .HasForeignKey(tp => tp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TaskProgress>()
            .HasOne(tp => tp.Task)
            .WithMany(t => t.TaskProgresses)
            .HasForeignKey(tp => tp.TaskId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MentorAssignment>()
            .HasOne(ma => ma.Mentor)
            .WithMany()
            .HasForeignKey(ma => ma.MentorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MentorAssignment>()
            .HasOne(ma => ma.Learner)
            .WithMany()
            .HasForeignKey(ma => ma.LearnerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Feedback>()
            .HasOne(f => f.Mentor)
            .WithMany()
            .HasForeignKey(f => f.MentorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Feedback>()
            .HasOne(f => f.Learner)
            .WithMany()
            .HasForeignKey(f => f.LearnerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Topic>()
            .HasOne(t => t.Lesson)
            .WithMany(l => l.Topics)
            .HasForeignKey(t => t.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
