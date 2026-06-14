namespace DataAccess.Data;

public interface IUserData
{
    Task<User> CreateUser();

    // Task<User> GetUserById(int userId);
    //
    // Task<User> GetUserByDeviceId(string deviceId);
    //
    // Task<User> GetUserByUsername(string username);
    //
    // Task<User> GetUserByRefreshToken(string refreshTokenHash);
    //
    // Task<List<string>> GetUsernamesByEmail(string email);
    //
    // Task<string> GetPasswordHash(string username);
    //
    // Task<bool> CheckUsernameExists(string username);
    //
    // // Increments
    //
    // Task IncrementVersesMemorized(int userId);
    //
    // Task AddPoints(int userId, int points);
    //
    // // Password Resets
    //
    // Task<PasswordRecoveryInfo?> GetPasswordRecoveryInfo(string username, string email);
    //
    // Task UpsertPasswordResetToken(int userId, string token);
    //
    // Task<PasswordResetToken?> GetPasswordResetToken(int userId, string token);
}