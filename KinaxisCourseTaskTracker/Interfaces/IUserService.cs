using KinaxisCourseTaskTracker.DTOs.Auth;
using KinaxisCourseTaskTracker.Models;

namespace KinaxisCourseTaskTracker.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync(UserRole? roleFilter = null);
    Task<UserDto?> GetUserByIdAsync(int userId);
    Task<UserDto?> UpdateUserAsync(int userId, InviteUserDto updateDto);
    Task<bool> SetUserStatusAsync(int userId, UserStatus status);
}
