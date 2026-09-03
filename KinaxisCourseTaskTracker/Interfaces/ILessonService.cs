using KinaxisCourseTaskTracker.DTOs.Lessons;

namespace KinaxisCourseTaskTracker.Interfaces;

public interface ILessonService
{
    Task<IEnumerable<LessonDto>> GetCourseLessonsAsync(int courseId, int? userId = null);
    Task<LessonDto?> GetLessonByIdAsync(int lessonId, int? userId = null);
    Task<LessonDto> CreateLessonAsync(CreateLessonDto createDto);
    Task<LessonDto?> UpdateLessonAsync(int lessonId, UpdateLessonDto updateDto);
    Task<bool> DeleteLessonAsync(int lessonId);
    Task<LessonDto?> StartLessonAsync(int userId, int lessonId);
    Task<LessonDto?> UpdateLessonProgressAsync(int userId, int lessonId, int timeSpentMinutes);
    Task<LessonDto?> CompleteLessonAsync(int userId, int lessonId);
}
