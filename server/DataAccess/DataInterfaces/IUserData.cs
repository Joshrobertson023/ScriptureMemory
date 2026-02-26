using DataAccess.Models;
using static ScriptureMemoryLibrary.Enums;

namespace DataAccess.DataInterfaces;
public interface IUserData
{
    Task CreateUser(User user);
    Task<User?> GetUserFromUsername(string username);
    Task<User?> GetUserFromToken(string token);
    Task<string?> GetTokenFromUsername(string username);
    Task UpdateLastSeen(int userId);
    Task<List<User>> GetUsersFromEmail(string email);
    Task<string?> GetPasswordHash(string username);
    Task<List<User>> GetUsers(int count = 100);
    Task<bool> CheckUsernameExists(string username);
    Task<int> GetUserIdFromUsername(string username);
    Task<User> GetUserFromUserId(int userId);
    Task<string> GetUsernameFromId(int userId);
    Task IncrementVersesMemorized(int userId);
    Task AddPoints(int userId, int points);
    Task UpdateDescription(int userId, string description);
    Task<List<string>> GetUsernamesByProfile(string firstName, string lastName, string email);
    Task<PasswordRecoveryInfo?> GetPasswordRecoveryInfo(string username, string email);
    Task UpsertPasswordResetToken(int userId, string token);
    Task<PasswordResetToken?> GetPasswordResetToken(int userId, string token);
    Task DeletePasswordResetToken(int userId);
    Task UpdatePassword(int userId, string hashedPassword);
    Task UpdateUsername(int userId, string newUsername);
    Task<List<User>> GetLeaderboard(int page, int pageSize);
    Task<int> GetUserRank(int userId);
    Task<List<User>> SearchUsers(string query);
    Task UpdateEmail(int userId, string newEmail);
    Task UpdateName(int userId, string firstName, string lastName);
}
