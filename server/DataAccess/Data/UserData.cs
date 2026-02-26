using Dapper;
using DataAccess.DataInterfaces;
using DataAccess.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ScriptureMemoryLibrary.Enums;

namespace DataAccess.Data;
public class UserData : IUserData
{
    private readonly IConfiguration _config;
    private readonly string connectionString;

    public UserData(IConfiguration config)
    {
        _config = config;
        connectionString = _config.GetConnectionString("Default")!;
    }


    // -------------------------------------------------------
    //  Insert
    // -------------------------------------------------------

    public async Task CreateUser(User user)
    {
        var sql = @"INSERT INTO USERS
                    (USERNAME, FIRST_NAME, LAST_NAME, EMAIL, AUTH_TOKEN, STATUS, HASHED_PASSWORD, DATE_REGISTERED, 
                     LAST_SEEN, PROFILE_DESCRIPTION, POINTS, VERSES_MEMORIZED, PROFILE_PICTURE_URL)
                    VALUES
                    (:Username, :FirstName, :LastName, :Email, :AuthToken, :Status, :HashedPassword, :DateRegistered,
                     :LastSeen, :Description, :Points, :VersesMemorized, :ProfilePictureUrl)";

        await using var conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(
            sql,
            new
            {
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                AuthToken = user.AuthToken,
                Status = user.Status,
                HashedPassword = user.HashedPassword,
                DateRegistered = user.DateRegistered,
                LastSeen = user.LastSeen,
                Description = user.ProfileDescription,
                Points = user.Points,
                VersesMemorized = user.VersesMemorizedCount,
                ProfilePictureUrl = user.ProfilePictureUrl
            });
    }


    // -------------------------------------------------------
    //  Get
    // -------------------------------------------------------

    public async Task<User?> GetUserFromUsername(string username)
    {
        var sql = @"SELECT ID, USERNAME, FIRST_NAME as FirstName, LAST_NAME as LastName, EMAIL, AUTH_TOKEN as AuthToken, ""STATUS"",
                    HASHED_PASSWORD as HashedPassword, LAST_SEEN as LastSeen, PROFILE_DESCRIPTION AS ProfileDescription, 
                    POINTS, VERSES_MEMORIZED AS VersesMemorizedCount, 
                    PROFILE_PICTURE_URL AS ProfilePictureUrl, DATE_REGISTERED as DateRegistered
                    FROM USERS WHERE USERNAME = :Username";

        await using var conn = new OracleConnection(connectionString);
        await conn.OpenAsync();

        var results = await conn.QueryAsync<User>(sql, new { Username = username });
        return results.FirstOrDefault();
    }

    public async Task<User?> GetUserFromToken(string token)
    {
        var sql = @"SELECT ID, USERNAME, FIRST_NAME as FirstName, LAST_NAME as LastName, EMAIL, AUTH_TOKEN as AuthToken, ""STATUS"",
                    HASHED_PASSWORD as HashedPassword, LAST_SEEN as LastSeen, PROFILE_DESCRIPTION AS ProfileDescription, 
                    POINTS, VERSES_MEMORIZED AS VersesMemorizedCount, 
                    PROFILE_PICTURE_URL AS ProfilePictureUrl, DATE_REGISTERED as DateRegistered
                    FROM USERS WHERE AUTH_TOKEN = :Token";
        await using var conn = new OracleConnection(connectionString);
        await conn.OpenAsync();

        var results = await conn.QueryAsync<User>(sql, new { Token = token });
        return results.FirstOrDefault();
    }

    public async Task<string?> GetTokenFromUsername(string username)
    {
        var sql = @"SELECT AUTH_TOKEN as AuthToken FROM USERS WHERE USERNAME = :Username";

        await using var conn = new OracleConnection(connectionString);
        await conn.OpenAsync();

        var results = await conn.QueryAsync<string>(sql, new { Username = username });
        return results.FirstOrDefault();
    }


    public async Task<List<User>> GetUsersFromEmail(string email)
    {
        var sql = @"SELECT ID, USERNAME, FIRST_NAME as FirstName, LAST_NAME as LastName, EMAIL, AUTH_TOKEN as AuthToken, ""STATUS"",
                    HASHED_PASSWORD as HashedPassword, LAST_SEEN as LastSeen, PROFILE_DESCRIPTION AS ProfileDescription, 
                    POINTS, VERSES_MEMORIZED AS VersesMemorizedCount, 
                    PROFILE_PICTURE_URL AS ProfilePictureUrl, DATE_REGISTERED as DateRegistered
                    FROM USERS WHERE UPPER(EMAIL) = UPPER(:Email)";

        await using var conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<User>(sql, new { Email = email });
        return results.ToList();
    }

    public async Task<string?> GetPasswordHash(string username)
    {
        var sql = $@"SELECT HASHED_PASSWORD as HashedPassword FROM USERS WHERE USERNAME = :Username";
        await using var conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<string>(sql, new { Username = username });
        return results.FirstOrDefault();
    }

    public async Task<List<string>> GetUsernamesByProfile(string firstName, string lastName, string email)
    {
        var sql = @"SELECT USERNAME 
                    FROM USERS 
                    WHERE UPPER(FIRST_NAME) = UPPER(:firstName)
                      AND UPPER(LAST_NAME) = UPPER(:lastName)
                      AND UPPER(EMAIL) = UPPER(:email)";
        using IDbConnection conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<string>(sql, new
        {
            firstName,
            lastName,
            email
        });

        return results.ToList();
    }

    public async Task<List<User>> GetUsers(int count = 100)
    {
        var sql = @"SELECT ID, USERNAME, FIRST_NAME as FirstName, LAST_NAME as LastName, EMAIL, AUTH_TOKEN as AuthToken, ""STATUS"",
                    HASHED_PASSWORD as HashedPassword, LAST_SEEN as LastSeen, PROFILE_DESCRIPTION AS ProfileDescription, 
                    POINTS, VERSES_MEMORIZED AS VersesMemorizedCount, 
                    PROFILE_PICTURE_URL AS ProfilePictureUrl, DATE_REGISTERED as DateRegistered
                    FROM USERS WHERE ROWNUM <= :Count";

        await using var conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<User>(sql, new { Count = count });
        return results.ToList();
    }

    public async Task<int> GetUserIdFromUsername(string username)
    {
        var sql = @"SELECT ID FROM USERS WHERE USERNAME = :Username";

        await using var conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<int>(sql, new { Username = username });

        return results.First();
    }

    public async Task<User> GetUserFromUserId(int userId)
    {
        var sql = @"SELECT ID, USERNAME, FIRST_NAME as FirstName, LAST_NAME as LastName, EMAIL, AUTH_TOKEN as AuthToken, ""STATUS"",
                    HASHED_PASSWORD as HashedPassword, LAST_SEEN as LastSeen, PROFILE_DESCRIPTION AS ProfileDescription, 
                    POINTS, VERSES_MEMORIZED AS VersesMemorizedCount, 
                    PROFILE_PICTURE_URL AS ProfilePictureUrl, DATE_REGISTERED as DateRegistered
                    FROM USERS WHERE ID = :UserId";

        await using var conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<User>(sql, new { UserId = userId });

        return results.First();
    }

    public async Task<bool> CheckUsernameExists(string username)
    {
        var sql = @"SELECT COUNT(1) FROM USERS WHERE USERNAME = :Username";
        await using var conn = new OracleConnection(connectionString);
        var count = await conn.ExecuteScalarAsync<int>(sql, new { Username = username });
        return count > 0;
    }

    public async Task<string> GetUsernameFromId(int userId)
    {
        var sql = @"SELECT USERNAME FROM USERS WHERE ID = :UserId";
        await using var conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<string>(sql, new { UserId = userId });
        return results.FirstOrDefault() ?? "";
    }


    // -------------------------------------------------------
    //  Increment
    // -------------------------------------------------------

    public async Task IncrementVersesMemorized(int userId)
    {
        var sql = @"UPDATE USERS SET VERSES_MEMORIZED = NVL(VERSES_MEMORIZED, 0) + 1 WHERE ID = :Id";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { Id = userId }, commandType: CommandType.Text);
    }

    public async Task AddPoints(int userId, int points)
    {
        var sql = @"UPDATE USERS SET POINTS = NVL(POINTS, 0) + :points WHERE ID = :userId";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { points, userId }, commandType: CommandType.Text);
    }


    // -------------------------------------------------------
    //  Password Reset
    // -------------------------------------------------------

    public async Task<PasswordRecoveryInfo?> GetPasswordRecoveryInfo(string username, string email)
    {
        var sql = @"SELECT USERNAME, EMAIL, HASHED_PASSWORD as HashedPassword
                    FROM USERS 
                    WHERE USERNAME = :username AND UPPER(EMAIL) = UPPER(:email)";
        using IDbConnection conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<PasswordRecoveryInfo>(sql, new
        {
            username,
            email
        }, commandType: CommandType.Text);
        return results.FirstOrDefault();
    }

    public async Task UpsertPasswordResetToken(int userId, string token)
    {
        const string updateSql = @"UPDATE PASSWORD_RESETS 
                                   SET TOKEN = :token, SENT = SYSDATE 
                                   WHERE USER_ID = :userId";
        const string insertSql = @"INSERT INTO PASSWORD_RESETS (USER_ID, TOKEN, SENT) 
                                   VALUES (:userId, :token, SYSDATE)";

        using IDbConnection conn = new OracleConnection(connectionString);
        var affected = await conn.ExecuteAsync(updateSql, new { token, userId }, commandType: CommandType.Text);
        if (affected == 0)
        {
            await conn.ExecuteAsync(insertSql, new { userId, token }, commandType: CommandType.Text);
        }
    }

    public async Task<PasswordResetToken?> GetPasswordResetToken(int userId, string token)
    {
        var sql = @"SELECT USER_ID as UserId, TOKEN, SENT 
                    FROM PASSWORD_RESETS 
                    WHERE USER_ID = :userId AND TOKEN = :token";
        using IDbConnection conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<PasswordResetToken>(sql, new { userId, token }, commandType: CommandType.Text);
        return results.FirstOrDefault();
    }

    public async Task DeletePasswordResetToken(int userId)
    {
        var sql = @"DELETE FROM PASSWORD_RESETS WHERE USER_ID = :userId";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { userId }, commandType: CommandType.Text);
    }


    // -------------------------------------------------------
    //  Update
    // -------------------------------------------------------

    public async Task UpdatePassword(int userId, string hashedPassword)
    {
        var sql = @"UPDATE USERS SET HASHEDPASSWORD = :hashedPassword WHERE ID = :userId";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { hashedPassword, userId }, commandType: CommandType.Text);
    }

    public async Task UpdateUsername(int userId, string newUsername)
    {
        var sql = @"UPDATE USERS SET USERNAME = :NewUsername WHERE ID = :UserId";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { NewUsername = newUsername, UserId = userId }, commandType: CommandType.Text);
    }

    public async Task UpdateEmail(int userId, string newEmail)
    {
        var sql = @"UPDATE USERS SET EMAIL = :NewEmail WHERE ID = :UserId";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { NewEmail = newEmail, UserId = userId }, commandType: CommandType.Text);
    }

    public async Task UpdateLastSeen(int userId)
    {
        var sql = @"UPDATE USERS SET LAST_SEEN = :CurrentDate WHERE ID = :UserId";
        await using var conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { CurrentDate = DateTime.UtcNow, UserId = userId });
    }

    public async Task UpdateDescription(int userId, string description)
    {
        var sql = @"UPDATE USERS SET PROFILE_DESCRIPTION = :description WHERE ID = :UserId";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { description = description, UserId = userId }, commandType: CommandType.Text);
    }

    public async Task UpdateName(int userId, string firstName, string lastName)
    {
        var sql = @"UPDATE USERS SET FIRST_NAME = :firstName, LAST_NAME = :lastName WHERE ID = :UserId";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { firstName = firstName, lastName = lastName, UserId = userId }, commandType: CommandType.Text);
    }


    // -------------------------------------------------------
    //  Leaderboard
    // -------------------------------------------------------

    public async Task<List<User>> GetLeaderboard(int page, int pageSize)
    {
        var offset = (page - 1) * pageSize;
        var sql = @"SELECT USERNAME, FIRST_NAME as FirstName, LAST_NAME as LastName, EMAIL, AUTH_TOKEN as AuthToken,
                           ""STATUS"" AS Status, HASHED_PASSWORD as HashedPassword,
                           DATE_REGISTERED as DateRegistered, LAST_SEEN as LastSeen, PROFILE_DESCRIPTION as ProfileDescription,
                           VERSES_MEMORIZED AS VersesMemorizedCount, POINTS, 
                           PROFILE_PICTURE_URL AS ProfilePictureUrl,
                           ROW_NUMBER() OVER (ORDER BY NVL(POINTS, 0) DESC, USERNAME ASC) as rn
                    FROM USERS
                    WHERE rn > :Offset AND rn <= :Offset + :PageSize
                    ORDER BY rn";

        await using var conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<User>(sql, new { Offset = offset, PageSize = pageSize });
        return results.ToList();
    }

    public async Task<int> GetUserRank(int userId)
    {
        var sql = @"SELECT rn FROM (
                        SELECT ID,
                               ROW_NUMBER() OVER (ORDER BY NVL(POINTS, 0) DESC, USERNAME ASC) as rn
                        FROM USERS
                    )
                    WHERE ID = :UserId";
        await using var conn = new OracleConnection(connectionString);
        var rank = await conn.QueryFirstOrDefaultAsync<int?>(sql, new { UserId = userId });
        return rank ?? 0;
    }


    // -------------------------------------------------------
    //  Search
    // -------------------------------------------------------

    public async Task<List<User>> SearchUsers(string query)
    {
        var fullName = query.Trim().Split(' ', 2);

        var sql = @"SELECT USERNAME, FIRST_NAME as FirstName, LAST_NAME as LastName, EMAIL, AUTH_TOKEN as AuthToken,
                           ""STATUS"" AS Status, HASHED_PASSWORD as HashedPassword,
                           DATE_REGISTERED as DateRegistered, LAST_SEEN as LastSeen, PROFILE_DESCRIPTION as ProfileDescription,
                           VERSES_MEMORIZED AS VersesMemorizedCount, POINTS, 
                           PROFILE_PICTURE_URL AS ProfilePictureUrl
                    FROM USERS
                    WHERE UPPER(USERNAME) LIKE UPPER(:Query) 
                       OR UPPER(FIRST_NAME) LIKE UPPER(:Query) 
                       OR UPPER(LAST_NAME) LIKE UPPER(:Query)
                       OR (UPPER(FIRST_NAME) LIKE UPPER(:FirstName) AND UPPER(LAST_NAME) LIKE UPPER(:LastName))
                    ORDER BY USERNAME";

        await using var conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<User>(
            sql, 
            new 
            { 
                Query = $"%{query}%", 
                FirstName = fullName[0], 
                LastName = fullName.Length > 1 ? fullName[1] : "" 
            });

        return results.ToList();
    }
}
