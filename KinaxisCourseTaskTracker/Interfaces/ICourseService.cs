using KinaxisCourseTaskTracker.DTOs.Courses;
using KinaxisCourseTaskTracker.DTOs.Enrollment;

namespace KinaxisCourseTaskTracker.Interfaces;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetAvailableCoursesAsync();
    Task<CourseDetailDto?> GetCourseByIdAsync(int courseId, int? userId = null);
    Task<CourseDto> CreateCourseAsync(CreateCourseDto createDto);
    Task<CourseDto?> UpdateCourseAsync(int courseId, UpdateCourseDto updateDto);
    Task<bool> DeactivateCourseAsync(int courseId);
}
