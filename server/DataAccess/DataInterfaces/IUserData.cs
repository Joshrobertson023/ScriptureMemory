using DataAccess.Models;
using static VerseAppLibrary.Enums;

namespace DataAccess.DataInterfaces;
public interface IUserData
{
    Task CreateUser(User user);
    Task<User?> GetUserFromUsername(string username);
    Task<User?> GetUserFromToken(string token);
    Task<string?> GetTokenFromUsername(string username);
    Task UpdateLastSeen(string username);
    Task<List<User>> GetUsersFromEmail(string email);
    Task<string?> GetPasswordHash(string username);
    Task<List<User>> GetUsers(int count = 100);
    Task IncrementVersesMemorized(string username);
    Task AddPoints(string username, int points);
    Task UpdateDescription(string description, string username);
    Task<List<string>> GetUsernamesByProfile(string firstName, string lastName, string email);
    Task<PasswordRecoveryInfo?> GetPasswordRecoveryInfo(string username, string email);
    Task UpsertPasswordResetToken(string username, string token);
    Task<PasswordResetToken?> GetPasswordResetToken(string username, string token);
    Task DeletePasswordResetToken(string username);
    Task UpdatePassword(string username, string hashedPassword);
    Task UpdateUsername(string username, string newUsername);
    Task<List<User>> GetLeaderboard(int page, int pageSize);
    Task<int> GetUserRank(string username);
    Task<List<User>> SearchUsers(string query);
    Task UpdateEmail(string username, string newEmail);
    Task UpdateName(string username, string firstName, string lastName);
}