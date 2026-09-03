using KinaxisCourseTaskTracker.DTOs.Auth;

namespace KinaxisCourseTaskTracker.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<UserDto> InviteUserAsync(InviteUserDto inviteDto);
    Task<bool> VerifyEmailAsync(VerifyEmailDto verifyDto);
    Task<UserDto> CreateMentorAsync(CreateMentorDto createMentorDto);
    Task<bool> IsEmailRegisteredAsync(string email);
}
