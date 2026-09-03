using KinaxisCourseTaskTracker.DTOs.Auth;
using KinaxisCourseTaskTracker.Helpers;
using KinaxisCourseTaskTracker.Interfaces;
using KinaxisCourseTaskTracker.Models;
using KinaxisCourseTaskTracker.Repositories.Interfaces;

namespace KinaxisCourseTaskTracker.Services;

public class AuthService : IAuthService
{
    private const string AllowedDomain = "@supplychainz.in";
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    private static void ValidateEmailDomain(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Trim().EndsWith(AllowedDomain, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Access restricted: Only email addresses ending with {AllowedDomain} are allowed.");
        }
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        ValidateEmailDomain(loginDto.Email);

        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (!PasswordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (user.Status != UserStatus.Active)
        {
            throw new UnauthorizedAccessException($"Account is not active. Current status: {user.Status}");
        }

        var token = JwtHelper.GenerateToken(user, _configuration);

        return new AuthResponseDto
        {
            Token = token,
            User = MapToUserDto(user)
        };
    }

    public async Task<UserDto> InviteUserAsync(InviteUserDto inviteDto)
    {
        ValidateEmailDomain(inviteDto.Email);

        var existingUser = await _userRepository.EmailExistsAsync(inviteDto.Email);
        if (existingUser)
        {
            throw new InvalidOperationException($"User with email '{inviteDto.Email}' already exists.");
        }

        var user = new User
        {
            Name = inviteDto.Name,
            Email = inviteDto.Email,
            Department = inviteDto.Department,
            Role = inviteDto.Role,
            Status = UserStatus.Invited,
            EmailVerified = false,
            PasswordHash = string.Empty // Set during verification
        };

        await _userRepository.AddUserAsync(user);
        await _userRepository.SaveChangesAsync();

        // Generate 48-hour secure verification token
        var plainToken = TokenHelper.GenerateRandomToken();
        var tokenHash = TokenHelper.HashToken(plainToken);

        var verificationToken = new EmailVerificationToken
        {
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddHours(48),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddVerificationTokenAsync(verificationToken);
        await _userRepository.SaveChangesAsync();

        // Log simulation of sent email link
        Console.WriteLine($"[EMAIL INVITATION SENT] User: {user.Email} | Verification Link: /api/auth/verify-email?token={plainToken}");

        return MapToUserDto(user);
    }

    public async Task<bool> VerifyEmailAsync(VerifyEmailDto verifyDto)
    {
        var tokenHash = TokenHelper.HashToken(verifyDto.Token);

        var tokenEntity = await _userRepository.GetVerificationTokenByHashAsync(tokenHash);

        if (tokenEntity == null)
        {
            throw new InvalidOperationException("Invalid verification token.");
        }

        if (tokenEntity.UsedAt != null)
        {
            throw new InvalidOperationException("Verification token has already been used.");
        }

        if (tokenEntity.ExpiresAt < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Verification token has expired (tokens are valid for 48 hours).");
        }

        // Activate User
        var user = tokenEntity.User;
        user.PasswordHash = PasswordHasher.HashPassword(verifyDto.SetPassword);
        user.Status = UserStatus.Active;
        user.EmailVerified = true;
        user.UpdatedAt = DateTime.UtcNow;

        tokenEntity.UsedAt = DateTime.UtcNow;

        await _userRepository.SaveChangesAsync();
        return true;
    }

    public async Task<UserDto> CreateMentorAsync(CreateMentorDto createMentorDto)
    {
        ValidateEmailDomain(createMentorDto.Email);

        var existingUser = await _userRepository.EmailExistsAsync(createMentorDto.Email);
        if (existingUser)
        {
            throw new InvalidOperationException($"User with email '{createMentorDto.Email}' already exists.");
        }

        var mentor = new User
        {
            Name = createMentorDto.Name,
            Email = createMentorDto.Email,
            Department = createMentorDto.Department,
            Role = UserRole.TrainingMentor,
            Status = UserStatus.Active,
            EmailVerified = true,
            PasswordHash = PasswordHasher.HashPassword(createMentorDto.Password)
        };

        await _userRepository.AddUserAsync(mentor);
        await _userRepository.SaveChangesAsync();

        return MapToUserDto(mentor);
    }

    public async Task<bool> IsEmailRegisteredAsync(string email)
    {
        ValidateEmailDomain(email);
        return await _userRepository.EmailExistsAsync(email);
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
