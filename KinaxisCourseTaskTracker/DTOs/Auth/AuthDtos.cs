using System.ComponentModel.DataAnnotations;
using KinaxisCourseTaskTracker.Models;

namespace KinaxisCourseTaskTracker.DTOs.Auth;

public class LoginDto
{
    [Required, EmailAddress]
    [RegularExpression(@"^[^@\s]+@supplychainz\.in$", ErrorMessage = "Only email addresses ending with @supplychainz.in are allowed.")]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class InviteUserDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress]
    [RegularExpression(@"^[^@\s]+@supplychainz\.in$", ErrorMessage = "Only email addresses ending with @supplychainz.in are allowed.")]
    public string Email { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Learner;
}

public class VerifyEmailDto
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string SetPassword { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool EmailVerified { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateMentorDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress]
    [RegularExpression(@"^[^@\s]+@supplychainz\.in$", ErrorMessage = "Only email addresses ending with @supplychainz.in are allowed.")]
    public string Email { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
}
