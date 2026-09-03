using KinaxisCourseTaskTracker.DTOs.Auth;
using KinaxisCourseTaskTracker.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinaxisCourseTaskTracker.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var result = await _authService.LoginAsync(loginDto);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("register/invite")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> InviteUser([FromBody] InviteUserDto inviteDto)
    {
        try
        {
            var user = await _authService.InviteUserAsync(inviteDto);
            return CreatedAtAction(nameof(InviteUser), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("verify-email")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto verifyDto)
    {
        try
        {
            await _authService.VerifyEmailAsync(verifyDto);
            return Ok(new { message = "Email verified successfully. Account is now active." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("create-mentor")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> CreateMentor([FromBody] CreateMentorDto createMentorDto)
    {
        try
        {
            var mentor = await _authService.CreateMentorAsync(createMentorDto);
            return Ok(mentor);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("check-email")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckEmail([FromQuery] string email)
    {
        try
        {
            bool isRegistered = await _authService.IsEmailRegisteredAsync(email);
            return Ok(new { email, isRegistered, message = isRegistered ? "Email already exists in the database." : "Email is available." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
