using KinaxisCourseTaskTracker.DTOs.Auth;
using KinaxisCourseTaskTracker.Interfaces;
using KinaxisCourseTaskTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinaxisCourseTaskTracker.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] UserRole? role)
    {
        var users = await _userService.GetAllUsersAsync(role);
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null) return NotFound(new { message = "User not found." });
        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] InviteUserDto updateDto)
    {
        var user = await _userService.UpdateUserAsync(id, updateDto);
        if (user == null) return NotFound(new { message = "User not found." });
        return Ok(user);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> SetStatus(int id, [FromQuery] UserStatus status)
    {
        var success = await _userService.SetUserStatusAsync(id, status);
        if (!success) return NotFound(new { message = "User not found." });
        return Ok(new { message = $"User status updated to {status}" });
    }
}
