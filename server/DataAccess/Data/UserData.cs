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
                    (USERNAME, FIRSTNAME, LASTNAME, EMAIL, AUTHTOKEN, STATUS, HASHEDPASSWORD, DATEREGISTERED, 
                     LASTSEEN, DESCRIPTION, POINTS, VERSES_MEMORIZED, PROFILE_PICTURE_URL)
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
        var sql = @"SELECT USERNAME, FIRSTNAME, LASTNAME, EMAIL, AUTHTOKEN, ""STATUS"",
                    HASHEDPASSWORD, LASTSEEN, ""DESCRIPTION"" AS ProfileDescription, 
                    POINTS, VERSES_MEMORIZED AS VersesMemorizedCount, 
                    PROFILE_PICTURE_URL AS ProfilePictureUrl, DATEREGISTERED
                    FROM USERS WHERE USERNAME = :Username";

        await using var conn = new OracleConnection(connectionString);
        await conn.OpenAsync();

        var results = await conn.QueryAsync<User>(sql, new { Username = username });
        return results.FirstOrDefault();
    }

    public async Task<User?> GetUserFromToken(string token)
    {
        var sql = @"SELECT USERNAME, FIRSTNAME, LASTNAME, EMAIL, AUTHTOKEN, ""STATUS"",
                    HASHEDPASSWORD, LASTSEEN, ""DESCRIPTION"" AS ProfileDescription, 
                    POINTS, VERSES_MEMORIZED AS VersesMemorizedCount, 
                    PROFILE_PICTURE_URL AS ProfilePictureUrl, DATEREGISTERED
                    FROM USERS WHERE AUTHTOKEN = :Token";

        await using var conn = new OracleConnection(connectionString);
        await conn.OpenAsync();

        var results = await conn.QueryAsync<User>(sql, new { Token = token });
        return results.FirstOrDefault();
    }

    public async Task<string?> GetTokenFromUsername(string username)
    {
        var sql = @"SELECT AUTHTOKEN FROM USERS WHERE USERNAME = :Username";

        await using var conn = new OracleConnection(connectionString);
        await conn.OpenAsync();

        var results = await conn.QueryAsync<string>(sql, new { Username = username });
        return results.FirstOrDefault();
    }


    public async Task<List<User>> GetUsersFromEmail(string email)
    {
        var sql = @"SELECT USERNAME, FIRSTNAME, LASTNAME, EMAIL, AUTHTOKEN, ""STATUS"",
                    HASHEDPASSWORD, LASTSEEN, ""DESCRIPTION"" AS ProfileDescription, 
                    POINTS, VERSES_MEMORIZED AS VersesMemorizedCount, 
                    PROFILE_PICTURE_URL AS ProfilePictureUrl, DATEREGISTERED
                    FROM USERS WHERE UPPER(EMAIL) = UPPER(:Email)";

        await using var conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<User>(sql, new { Email = email });
        return results.ToList();
    }

    public async Task<string?> GetPasswordHash(string username)
    {
        var sql = $@"SELECT HASHEDPASSWORD FROM USERS WHERE USERNAME = :Username";
        await using var conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<string>(sql, new { Username = username });
        return results.FirstOrDefault();
    }

    public async Task<List<string>> GetUsernamesByProfile(string firstName, string lastName, string email)
    {
        var sql = @"SELECT USERNAME 
                    FROM USERS 
                    WHERE UPPER(FIRSTNAME) = UPPER(:firstName)
                      AND UPPER(LASTNAME) = UPPER(:lastName)
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
        var sql = @"SELECT USERNAME, FIRSTNAME, LASTNAME, EMAIL, AUTHTOKEN, ""STATUS"",
                    HASHEDPASSWORD, LASTSEEN, ""DESCRIPTION"" AS ProfileDescription, 
                    POINTS, VERSES_MEMORIZED AS VersesMemorizedCount, 
                    PROFILE_PICTURE_URL AS ProfilePictureUrl, DATEREGISTERED
                    FROM USERS WHERE ROWNUM <= :Count";

        await using var conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<User>(sql, new { Count = count });
        return results.ToList();
    }


    // -------------------------------------------------------
    //  Increment
    // -------------------------------------------------------

    public async Task IncrementVersesMemorized(string username)
    {
        var sql = @"UPDATE USERS SET VERSES_MEMORIZED = NVL(VERSES_MEMORIZED, 0) + 1 WHERE USERNAME = :username";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { username = username }, commandType: CommandType.Text);
    }

    public async Task AddPoints(string username, int points)
    {
        var sql = @"UPDATE USERS SET POINTS = NVL(POINTS, 0) + :points WHERE USERNAME = :username";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { points, username }, commandType: CommandType.Text);
    }


    // -------------------------------------------------------
    //  Password Reset
    // -------------------------------------------------------

    public async Task<PasswordRecoveryInfo?> GetPasswordRecoveryInfo(string username, string email)
    {
        var sql = @"SELECT USERNAME, EMAIL, HASHEDPASSWORD 
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

    public async Task UpsertPasswordResetToken(string username, string token)
    {
        const string updateSql = @"UPDATE PASSWORD_RESETS 
                                   SET TOKEN = :token, SENT = SYSDATE 
                                   WHERE USERNAME = :username";
        const string insertSql = @"INSERT INTO PASSWORD_RESETS (USERNAME, TOKEN, SENT) 
                                   VALUES (:username, :token, SYSDATE)";

        using IDbConnection conn = new OracleConnection(connectionString);
        var affected = await conn.ExecuteAsync(updateSql, new { token, username }, commandType: CommandType.Text);
        if (affected == 0)
        {
            await conn.ExecuteAsync(insertSql, new { username, token }, commandType: CommandType.Text);
        }
    }

    public async Task<PasswordResetToken?> GetPasswordResetToken(string username, string token)
    {
        var sql = @"SELECT USERNAME, TOKEN, SENT 
                    FROM PASSWORD_RESETS 
                    WHERE USERNAME = :username AND TOKEN = :token";
        using IDbConnection conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<PasswordResetToken>(sql, new { username, token }, commandType: CommandType.Text);
        return results.FirstOrDefault();
    }

    public async Task DeletePasswordResetToken(string username)
    {
        var sql = @"DELETE FROM PASSWORD_RESETS WHERE USERNAME = :username";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { username }, commandType: CommandType.Text);
    }


    // -------------------------------------------------------
    //  Update
    // -------------------------------------------------------

    public async Task UpdatePassword(string username, string hashedPassword)
    {
        var sql = @"UPDATE USERS SET HASHEDPASSWORD = :hashedPassword WHERE USERNAME = :username";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { hashedPassword, username }, commandType: CommandType.Text);
    }

    public async Task UpdateUsername(string username, string newUsername)
    {
        var sql = @"UPDATE USERS SET USERNAME = :NewUsername WHERE USERNAME = :Username";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { NewUsername = newUsername, Username = username }, commandType: CommandType.Text);
    }

    public async Task UpdateEmail(string username, string newEmail)
    {
        var sql = @"UPDATE USERS SET EMAIL = :NewEmail WHERE USERNAME = :Username";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { NewEmail = newEmail, Username = username }, commandType: CommandType.Text);
    }

    public async Task UpdateName(string username, string newEmail)
    {
        var sql = @"UPDATE USERS SET EMAIL = :NewEmail WHERE USERNAME = :Username";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { NewEmail = newEmail, Username = username }, commandType: CommandType.Text);
    }

    public async Task UpdateLastSeen(string username)
    {
        var sql = @"UPDATE USERS SET LASTSEEN = :CurrentDate WHERE USERNAME = :Username";
        await using var conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { CurrentDate = DateTime.UtcNow, Username = username });
    }

    public async Task UpdateDescription(string description, string username)
    {
        var sql = @"UPDATE USERS SET ""DESCRIPTION"" = :description WHERE USERNAME = :username";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { description = description, username = username }, commandType: CommandType.Text);
    }

    public async Task UpdateName(string username, string firstName, string lastName)
    {
        var sql = @"UPDATE USERS SET FIRSTNAME = :firstName, LASTNAME = :lastName WHERE USERNAME = :Username";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { firstName = firstName, lastName = lastName, Username = username }, commandType: CommandType.Text);
    }


    // -------------------------------------------------------
    //  Leaderboard
    // -------------------------------------------------------

    public async Task<List<User>> GetLeaderboard(int page, int pageSize)
    {
        var offset = (page - 1) * pageSize;
        var sql = @"SELECT USERNAME, FIRSTNAME, LASTNAME, EMAIL, AUTHTOKEN,
                           ""STATUS"" AS Status, HASHEDPASSWORD,
                           DATEREGISTERED, LASTSEEN, ""DESCRIPTION"",
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

    public async Task<int> GetUserRank(string username)
    {
        var sql = @"SELECT rn FROM (
                        SELECT USERNAME,
                               ROW_NUMBER() OVER (ORDER BY NVL(POINTS, 0) DESC, USERNAME ASC) as rn
                        FROM USERS
                    )
                    WHERE USERNAME = :Username";

        await using var conn = new OracleConnection(connectionString);
        var rank = await conn.QueryFirstOrDefaultAsync<int?>(sql, new { Username = username });
        return rank ?? 0;
    }


    // -------------------------------------------------------
    //  Search
    // -------------------------------------------------------

    public async Task<List<User>> SearchUsers(string query)
    {
        var fullName = query.Trim().Split(' ', 2);

        var sql = @"SELECT USERNAME, FIRSTNAME, LASTNAME, EMAIL, AUTHTOKEN,
                           ""STATUS"" AS Status, HASHEDPASSWORD,
                           DATEREGISTERED, LASTSEEN, ""DESCRIPTION"",
                           VERSES_MEMORIZED AS VersesMemorizedCount, POINTS, 
                           PROFILE_PICTURE_URL AS ProfilePictureUrl
                    FROM USERS
                    WHERE UPPER(USERNAME) LIKE UPPER(:Query) 
                       OR UPPER(FIRSTNAME) LIKE UPPER(:Query) 
                       OR UPPER(LASTNAME) LIKE UPPER(:Query)
                       OR (UPPER(FIRSTNAME) LIKE UPPER(:FirstName) AND UPPER(LASTNAME) LIKE UPPER(:LastName))
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
