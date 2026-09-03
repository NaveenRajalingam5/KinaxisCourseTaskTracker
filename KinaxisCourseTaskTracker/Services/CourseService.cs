using KinaxisCourseTaskTracker.DTOs.Courses;
using KinaxisCourseTaskTracker.DTOs.Enrollment;
using KinaxisCourseTaskTracker.Interfaces;
using KinaxisCourseTaskTracker.Models;
using KinaxisCourseTaskTracker.Repositories.Interfaces;

namespace KinaxisCourseTaskTracker.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ITaskRepository _taskRepository;

    public CourseService(
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository,
        ITaskRepository taskRepository)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _taskRepository = taskRepository;
    }

    public async Task<IEnumerable<CourseDto>> GetAvailableCoursesAsync()
    {
        var courses = await _courseRepository.GetActiveCoursesAsync();

        return courses.Select(c => new CourseDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            DurationMinutes = c.DurationMinutes,
            IsActive = c.IsActive,
            LessonCount = c.Lessons.Count,
            TaskCount = c.Tasks.Count,
            CreatedAt = c.CreatedAt
        });
    }

    public async Task<CourseDetailDto?> GetCourseByIdAsync(int courseId, int? userId = null)
    {
        var course = await _courseRepository.GetByIdWithLessonsAndTasksAsync(courseId);
        if (course == null) return null;

        bool isEnrolled = false;
        string enrollmentStatus = "NotEnrolled";
        double progressPercentage = 0;
        int completedLessons = 0;
        int completedTasks = 0;

        if (userId.HasValue)
        {
            var enrollment = await _enrollmentRepository.GetEnrollmentAsync(userId.Value, courseId);

            if (enrollment != null)
            {
                isEnrolled = true;
                enrollmentStatus = enrollment.Status.ToString();

                var lessonIds = course.Lessons.Select(l => l.Id).ToList();
                completedLessons = await _enrollmentRepository.GetCompletedLessonsCountAsync(userId.Value, lessonIds);

                var taskIds = course.Tasks.Select(t => t.Id).ToList();
                completedTasks = await _taskRepository.GetCompletedTasksCountAsync(userId.Value, taskIds);

                if (course.Lessons.Count > 0)
                {
                    progressPercentage = Math.Round((double)completedLessons / course.Lessons.Count * 100, 2);
                }
            }
        }

        return new CourseDetailDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            DurationMinutes = course.DurationMinutes,
            IsEnrolled = isEnrolled,
            EnrollmentStatus = enrollmentStatus,
            ProgressPercentage = progressPercentage,
            CompletedLessonsCount = completedLessons,
            TotalLessonsCount = course.Lessons.Count,
            CompletedTasksCount = completedTasks,
            TotalTasksCount = course.Tasks.Count
        };
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto createDto)
    {
        var course = new Course
        {
            Title = createDto.Title,
            Description = createDto.Description,
            DurationMinutes = createDto.DurationMinutes,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _courseRepository.AddAsync(course);
        await _courseRepository.SaveChangesAsync();

        return new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            DurationMinutes = course.DurationMinutes,
            IsActive = course.IsActive,
            LessonCount = 0,
            TaskCount = 0,
            CreatedAt = course.CreatedAt
        };
    }

    public async Task<CourseDto?> UpdateCourseAsync(int courseId, UpdateCourseDto updateDto)
    {
        var course = await _courseRepository.GetByIdWithLessonsAndTasksAsync(courseId);
        if (course == null) return null;

        course.Title = updateDto.Title;
        course.Description = updateDto.Description;
        course.DurationMinutes = updateDto.DurationMinutes;
        course.IsActive = updateDto.IsActive;

        await _courseRepository.UpdateAsync(course);
        await _courseRepository.SaveChangesAsync();

        return new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            DurationMinutes = course.DurationMinutes,
            IsActive = course.IsActive,
            LessonCount = course.Lessons.Count,
            TaskCount = course.Tasks.Count,
            CreatedAt = course.CreatedAt
        };
    }

    public async Task<bool> DeactivateCourseAsync(int courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null) return false;

        course.IsActive = false;

        await _courseRepository.UpdateAsync(course);
        await _courseRepository.SaveChangesAsync();
        return true;
    }
}
