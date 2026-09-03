using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KinaxisCourseTaskTracker.Models;
using Microsoft.IdentityModel.Tokens;

namespace KinaxisCourseTaskTracker.Helpers;

public static class JwtHelper
{
    public static string GenerateToken(User user, IConfiguration configuration)
    {
        var key = configuration["Jwt:Key"] 
            ?? configuration["JWT:KEY"] 
            ?? "KinaxisCourseTaskTracker_SuperSecretKey_2026_MinLength32Bytes!";
        var issuer = configuration["Jwt:Issuer"] ?? "KinaxisCourseTaskTrackerAPI";
        var audience = configuration["Jwt:Audience"] ?? "KinaxisCourseTaskTrackerClient";
        var expirationMinutes = double.Parse(configuration["Jwt:ExpirationMinutes"] ?? "1440");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("sub", user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("email", user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("role", user.Role.ToString()),
            new Claim("Department", user.Department)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
