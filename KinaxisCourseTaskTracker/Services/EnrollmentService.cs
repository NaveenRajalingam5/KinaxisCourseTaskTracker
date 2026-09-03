using KinaxisCourseTaskTracker.DTOs.Enrollment;
using KinaxisCourseTaskTracker.Interfaces;
using KinaxisCourseTaskTracker.Models;
using KinaxisCourseTaskTracker.Repositories.Interfaces;

namespace KinaxisCourseTaskTracker.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;

    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        ICourseRepository courseRepository,
        IUserRepository userRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
        _userRepository = userRepository;
    }

    public async Task<EnrollmentDto> EnrollAsync(int userId, int courseId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || user.Status != UserStatus.Active)
        {
            throw new InvalidOperationException("Only Active users are allowed to enroll in courses.");
        }

        var course = await _courseRepository.GetByIdWithLessonsAndTasksAsync(courseId);

        if (course == null || !course.IsActive)
        {
            throw new InvalidOperationException("The requested course is invalid or inactive.");
        }

        var existingEnrollment = await _enrollmentRepository.GetEnrollmentAsync(userId, courseId);

        if (existingEnrollment != null)
        {
            throw new InvalidOperationException("Learner is already enrolled in this course.");
        }

        var enrollment = new Enrollment
        {
            UserId = userId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow,
            Status = EnrollmentStatus.Enrolled
        };

        await _enrollmentRepository.AddEnrollmentAsync(enrollment);
        await _enrollmentRepository.SaveChangesAsync();

        return new EnrollmentDto
        {
            Id = enrollment.Id,
            UserId = enrollment.UserId,
            CourseId = enrollment.CourseId,
            CourseTitle = course.Title,
            CourseDescription = course.Description,
            EnrolledAt = enrollment.EnrolledAt,
            Status = enrollment.Status.ToString(),
            CompletedAt = enrollment.CompletedAt,
            ProgressPercentage = 0,
            CompletedLessons = 0,
            TotalLessons = course.Lessons.Count
        };
    }

    public async Task<IEnumerable<EnrollmentDto>> GetLearnerEnrollmentsAsync(int userId)
    {
        var enrollments = await _enrollmentRepository.GetUserEnrollmentsAsync(userId);

        var result = new List<EnrollmentDto>();

        foreach (var e in enrollments)
        {
            var totalLessons = e.Course.Lessons.Count;
            var lessonIds = e.Course.Lessons.Select(l => l.Id).ToList();

            var completedLessons = await _enrollmentRepository.GetCompletedLessonsCountAsync(userId, lessonIds);

            double progress = totalLessons > 0 ? Math.Round((double)completedLessons / totalLessons * 100, 2) : 0;

            result.Add(new EnrollmentDto
            {
                Id = e.Id,
                UserId = e.UserId,
                CourseId = e.CourseId,
                CourseTitle = e.Course.Title,
                CourseDescription = e.Course.Description,
                EnrolledAt = e.EnrolledAt,
                Status = e.Status.ToString(),
                CompletedAt = e.CompletedAt,
                ProgressPercentage = progress,
                CompletedLessons = completedLessons,
                TotalLessons = totalLessons
            });
        }

        return result;
    }

    public async Task<EnrollmentDto?> GetEnrollmentDetailsAsync(int userId, int courseId)
    {
        var enrollment = await _enrollmentRepository.GetEnrollmentAsync(userId, courseId);

        if (enrollment == null) return null;

        var totalLessons = enrollment.Course.Lessons.Count;
        var lessonIds = enrollment.Course.Lessons.Select(l => l.Id).ToList();

        var completedLessons = await _enrollmentRepository.GetCompletedLessonsCountAsync(userId, lessonIds);

        double progress = totalLessons > 0 ? Math.Round((double)completedLessons / totalLessons * 100, 2) : 0;

        return new EnrollmentDto
        {
            Id = enrollment.Id,
            UserId = enrollment.UserId,
            CourseId = enrollment.CourseId,
            CourseTitle = enrollment.Course.Title,
            CourseDescription = enrollment.Course.Description,
            EnrolledAt = enrollment.EnrolledAt,
            Status = enrollment.Status.ToString(),
            CompletedAt = enrollment.CompletedAt,
            ProgressPercentage = progress,
            CompletedLessons = completedLessons,
            TotalLessons = totalLessons
        };
    }
}
