using KinaxisCourseTaskTracker.Models;

namespace KinaxisCourseTaskTracker.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<IEnumerable<User>> GetAllLearnersAsync();
    Task<bool> EmailExistsAsync(string email);
    Task AddUserAsync(User user);
    Task AddVerificationTokenAsync(EmailVerificationToken token);
    Task<EmailVerificationToken?> GetVerificationTokenByHashAsync(string tokenHash);
    Task SaveChangesAsync();
}
