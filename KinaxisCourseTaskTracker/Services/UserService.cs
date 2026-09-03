using KinaxisCourseTaskTracker.DTOs.Auth;
using KinaxisCourseTaskTracker.Interfaces;
using KinaxisCourseTaskTracker.Models;
using KinaxisCourseTaskTracker.Repositories.Interfaces;

namespace KinaxisCourseTaskTracker.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(UserRole? roleFilter = null)
    {
        var users = await _userRepository.GetAllUsersAsync();

        if (roleFilter.HasValue)
        {
            users = users.Where(u => u.Role == roleFilter.Value);
        }

        return users.Select(MapToUserDto);
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user == null ? null : MapToUserDto(user);
    }

    public async Task<UserDto?> UpdateUserAsync(int userId, InviteUserDto updateDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        user.Name = updateDto.Name;
        user.Email = updateDto.Email;
        user.Department = updateDto.Department;
        user.Role = updateDto.Role;

        await _userRepository.SaveChangesAsync();
        return MapToUserDto(user);
    }

    public async Task<bool> SetUserStatusAsync(int userId, UserStatus status)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        user.Status = status;

        await _userRepository.SaveChangesAsync();
        return true;
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Department = user.Department,
            Role = user.Role.ToString(),
            Status = user.Status.ToString(),
            EmailVerified = user.EmailVerified,
            CreatedAt = user.CreatedAt
        };
    }
}
