using KinaxisCourseTaskTracker.DTOs.Lessons;
using KinaxisCourseTaskTracker.DTOs.Topics;
using KinaxisCourseTaskTracker.Interfaces;
using KinaxisCourseTaskTracker.Models;
using KinaxisCourseTaskTracker.Repositories.Interfaces;

namespace KinaxisCourseTaskTracker.Services;

public class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUserRepository _userRepository;

    public LessonService(
        ILessonRepository lessonRepository,
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository,
        IUserRepository userRepository)
    {
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<LessonDto>> GetCourseLessonsAsync(int courseId, int? userId = null)
    {
        var lessons = await _lessonRepository.GetLessonsByCourseIdAsync(courseId);

        var result = new List<LessonDto>();

        var currentUser = userId.HasValue ? await _userRepository.GetByIdAsync(userId.Value) : null;
        bool isStaff = currentUser != null && (currentUser.Role == UserRole.Admin || currentUser.Role == UserRole.TrainingMentor);

        foreach (var lesson in lessons)
        {
            string status = "NotStarted";
            int timeSpent = 0;
            bool isUnlocked = true;

            if (userId.HasValue)
            {
                if (isStaff)
                {
                    isUnlocked = true;
                }
                else
                {
                    // Verify enrollment for learners
                    var isEnrolled = await _enrollmentRepository.IsUserEnrolledAsync(userId.Value, courseId);
                    if (!isEnrolled)
                    {
                        isUnlocked = false;
                    }
                    else
                    {
                        var progress = await _enrollmentRepository.GetLessonProgressAsync(userId.Value, lesson.Id);

                        if (progress != null)
                        {
                            status = progress.Status.ToString();
                            timeSpent = progress.TimeSpentMinutes;
                        }
                    }
                }
            }

            result.Add(new LessonDto
            {
                Id = lesson.Id,
                CourseId = lesson.CourseId,
                Title = lesson.Title,
                Description = lesson.Description,
                Order = lesson.Order,
                Status = status,
                IsUnlocked = isUnlocked,
                TimeSpentMinutes = timeSpent,
                Topics = lesson.Topics.OrderBy(t => t.Order).Select(t => new TopicDto
                {
                    Id = t.Id,
                    LessonId = t.LessonId,
                    Title = t.Title,
                    Description = t.Description,
                    Order = t.Order
                }).ToList()
            });
        }

        return result;
    }

    public async Task<LessonDto?> GetLessonByIdAsync(int lessonId, int? userId = null)
    {
        var lesson = await _lessonRepository.GetByIdWithTopicsAsync(lessonId);
        if (lesson == null) return null;

        var lessonsList = await GetCourseLessonsAsync(lesson.CourseId, userId);
        return lessonsList.FirstOrDefault(l => l.Id == lessonId);
    }

    public async Task<LessonDto> CreateLessonAsync(CreateLessonDto createDto)
    {
        var courseExists = await _courseRepository.ExistsAsync(createDto.CourseId);
        if (!courseExists)
        {
            throw new InvalidOperationException("Associated course does not exist.");
        }

        var lesson = new Lesson
        {
            CourseId = createDto.CourseId,
            Title = createDto.Title,
            Description = createDto.Description,
            Order = createDto.Order
        };

        await _lessonRepository.AddAsync(lesson);
        await _lessonRepository.SaveChangesAsync();

        return new LessonDto
        {
            Id = lesson.Id,
            CourseId = lesson.CourseId,
            Title = lesson.Title,
            Description = lesson.Description,
            Order = lesson.Order,
            Status = "NotStarted",
            IsUnlocked = true,
            Topics = new List<TopicDto>()
        };
    }

    public async Task<LessonDto?> UpdateLessonAsync(int lessonId, UpdateLessonDto updateDto)
    {
        var lesson = await _lessonRepository.GetByIdAsync(lessonId);
        if (lesson == null) return null;

        lesson.Title = updateDto.Title;
        lesson.Description = updateDto.Description;
        lesson.Order = updateDto.Order;

        await _lessonRepository.UpdateAsync(lesson);
        await _lessonRepository.SaveChangesAsync();

        return await GetLessonByIdAsync(lessonId);
    }

    public async Task<bool> DeleteLessonAsync(int lessonId)
    {
        var lesson = await _lessonRepository.GetByIdAsync(lessonId);
        if (lesson == null) return false;

        await _lessonRepository.DeleteAsync(lesson);
        await _lessonRepository.SaveChangesAsync();
        return true;
    }

    public async Task<LessonDto?> StartLessonAsync(int userId, int lessonId)
    {
        var lesson = await _lessonRepository.GetByIdAsync(lessonId);
        if (lesson == null)
        {
            throw new InvalidOperationException("Lesson not found.");
        }

        // Enrollment requirement check
        var isEnrolled = await _enrollmentRepository.IsUserEnrolledAsync(userId, lesson.CourseId);
        if (!isEnrolled)
        {
            throw new InvalidOperationException("Learner must be enrolled in the course to start lessons.");
        }

        var progress = await _enrollmentRepository.GetLessonProgressAsync(userId, lessonId);

        if (progress == null)
        {
            progress = new LessonProgress
            {
                UserId = userId,
                LessonId = lessonId,
                Status = LessonStatus.InProgress
            };
            await _enrollmentRepository.AddLessonProgressAsync(progress);
        }
        else if (progress.Status == LessonStatus.NotStarted)
        {
            progress.Status = LessonStatus.InProgress;
        }

        await _enrollmentRepository.SaveChangesAsync();
        return await GetLessonByIdAsync(lessonId, userId);
    }

    public async Task<LessonDto?> UpdateLessonProgressAsync(int userId, int lessonId, int timeSpentMinutes)
    {
        var progress = await _enrollmentRepository.GetLessonProgressAsync(userId, lessonId);

        if (progress == null)
        {
            throw new InvalidOperationException("Lesson progress not started yet. Call Start Lesson first.");
        }

        progress.TimeSpentMinutes += timeSpentMinutes;
        await _enrollmentRepository.SaveChangesAsync();

        return await GetLessonByIdAsync(lessonId, userId);
    }

    public async Task<LessonDto?> CompleteLessonAsync(int userId, int lessonId)
    {
        var lesson = await _lessonRepository.GetByIdAsync(lessonId);
        if (lesson == null) return null;

        var progress = await _enrollmentRepository.GetLessonProgressAsync(userId, lessonId);

        if (progress == null)
        {
            progress = new LessonProgress
            {
                UserId = userId,
                LessonId = lessonId
            };
            await _enrollmentRepository.AddLessonProgressAsync(progress);
        }

        progress.Status = LessonStatus.Completed;

        await _enrollmentRepository.SaveChangesAsync();

        // Check if all lessons in course are completed to complete the Course Enrollment
        var allCourseLessonIds = await _lessonRepository.GetLessonIdsByCourseIdAsync(lesson.CourseId);

        var completedCount = await _enrollmentRepository.GetCompletedLessonsCountAsync(userId, allCourseLessonIds);

        if (allCourseLessonIds.Count > 0 && completedCount == allCourseLessonIds.Count)
        {
            var enrollment = await _enrollmentRepository.GetEnrollmentAsync(userId, lesson.CourseId);

            if (enrollment != null && enrollment.Status != EnrollmentStatus.Completed)
            {
                enrollment.Status = EnrollmentStatus.Completed;
                enrollment.CompletedAt = DateTime.UtcNow;
                await _enrollmentRepository.SaveChangesAsync();
            }
        }

        return await GetLessonByIdAsync(lessonId, userId);
    }
}
