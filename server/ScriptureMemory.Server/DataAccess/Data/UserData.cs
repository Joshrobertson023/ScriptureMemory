using Dapper;
using DataAccess.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ScriptureMemory.Server.Tools.Enums;
using Npgsql;
using ScriptureMemory.Server.DataAccess.Models;

namespace DataAccess.Data;
public class UserData
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;

    private const string GetQuery =
        """
        select
        u.id as Id,
        u.username as Username,
        u.first_name as FirstName,
        u.last_name as LastName,
        u.email as Email,
        u.profile_picture_url as ProfilePictureUrl,
        u.hashed_password as HashedPassword,
        u.date_registered as DateRegistered,
        u.points as Points,
        u.memorized_count as VersesMemorizedCount,
        u.profile_description as ProfileDescription,
        up.theme as ThemePreference,
        up.bible_version as BibleVersion,
        up.collections_sort as CollectionsSort,
        up.vod_notifications as SubscribedVerseOfDay,
        up.notify_friends_memorized_passage as NotifyFriendsMemorizedPassage,
        up.notify_friends_published_collection as NotifyFriendsPublishedCollection,
        up.notify_collection_saved as NotifyCollectionSaved,
        up.notify_note_liked_comented as NotifyNoteLikedCommented,
        up.friends_activity_notifications as FriendsActivityNotificationsEnabled,
        up.overdue_reminders as OverdueRemindersEnabled,
        up.type_out_reference as TypeOutReference
        """;

    public UserData(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetConnectionString("PostgresConnection")
            ?? throw new InvalidOperationException("Connection string 'PostgresConnection' not found");
    }


    // -------------------------------------------------------
    //  Insert
    // -------------------------------------------------------

    public async Task<int> CreateUser()
    {
        using var conn = new NpgsqlConnection(_connectionString);
        using var transaction = await conn.BeginTransactionAsync();
        int newUserId = await conn.ExecuteScalarAsync<int>(
            """
            insert into users 
            (points, memorized_count, role)
            values
            (@Points, @VersesMemorized, @Role)
            returning id
            """,
            new
            {
                Points = 0,
                VersesMemorized = 0,
                Role = UserRole.User.ToString()
            }, transaction);
        await conn.ExecuteAsync(
            """
            insert into user_preferences
            (user_id)
            values
            (@UserId)
            """, new
            {
                UserId = newUserId
            }, transaction);
        await transaction.CommitAsync();
        return newUserId;
    }


    // -- Get ---------------------------------------------

    public class GetUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string HashedPassword { get; set; } = string.Empty;
        public DateTime? DateRegistered { get; set; }
        public string? ProfileDescription { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public int VersesMemorizedCount { get; set; }
        public int Points { get; set; } = 0;
        public int ThemePreference { get; set; }
        public int BibleVersion { get; set; }
        public int CollectionsSort { get; set; }
        public bool SubscribedVerseOfDay { get; set; } = true;
        public bool NotifyFriendsMemorizedPassage { get; set; } = true;
        public bool NotifyFriendsPublishedCollection { get; set; } = true;
        public bool NotifyCollectionSaved { get; set; } = true;
        public bool NotifyNoteLikedCommented { get; set; } = true;
        public bool FriendsActivityNotificationsEnabled { get; set; } = true;
        public bool OverdueRemindersEnabled { get; set; } = true;
        public bool TypeOutReference { get; set; } = false;

        public string? RefreshTokenHash { get; set; }
        public DateTime LastSeen { get; set; }
        public string? PushNotificationToken { get; set; }
    }

    public async Task<User> GetUserById(int userId)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        var results = await conn.QueryAsync<GetUserDto>(
            $"""
            {GetQuery},
            s.refresh_token_hash as RefreshTokenHash,
            s.last_seen as LastSeen,
            s.push_notification_token as PushNotificationToken
            from users u
            join user_preferences up on u.id = up.user_id
            left join device_sessions s on u.id = s.user_id
            where u.id = :UserId
            """, new
            {
                UserId = userId
            });

        return results
            .GroupBy(r => r.Id)
            .Select(g => new User
            {
                Id = g.Key,
                Username = g.First().Username,
                FirstName = g.First().FirstName,
                LastName = g.First().LastName,
                Email = g.First().Email,
                DateRegistered = g.First().DateRegistered,
                HashedPassword = g.First().HashedPassword,
                Preferences = new UserPreferences
                {
                    ThemePreference = (ThemePreference)g.First().ThemePreference,
                    BibleVersion = (BibleVersion)g.First().BibleVersion,
                    CollectionsSort = (CollectionsSort)g.First().CollectionsSort,
                    SubscribedVerseOfDay = g.First().SubscribedVerseOfDay,
                    NotifyFriendsMemorizedPassage = g.First().NotifyFriendsMemorizedPassage,
                    NotifyFriendsPublishedCollection = g.First().NotifyFriendsPublishedCollection,
                    NotifyCollectionSaved = g.First().NotifyCollectionSaved,
                    NotifyNoteLikedCommented = g.First().NotifyNoteLikedCommented,
                    FriendsActivityNotificationsEnabled = g.First().FriendsActivityNotificationsEnabled,
                    OverdueRemindersEnabled = g.First().OverdueRemindersEnabled,
                    TypeOutReference = g.First().TypeOutReference
                },
                ProfileDescription = g.First().ProfileDescription,
                ProfilePictureUrl = g.First().ProfilePictureUrl,
                VersesMemorizedCount = g.First().VersesMemorizedCount,
                Points = g.First().Points,
                Sessions = g.Where(r => r.RefreshTokenHash != null).Select(r => new Session
                {
                    RefreshTokenHash = r.RefreshTokenHash!,
                    LastSeenAt = r.LastSeen,
                    PushNotificationToken = r.PushNotificationToken
                }).ToList()
            }).FirstOrDefault() ?? throw new Exception("User not found");
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        var results = await conn.QueryAsync<GetUserDto>(
            $"""
            {GetQuery},
            s.refresh_token_hash as RefreshTokenHash,
            s.last_seen as LastSeen,
            s.push_notification_token as PushNotificationToken
            from users u
            join user_preferences up on u.id = up.user_id
            left join device_sessions s on u.id = s.user_id
            where u.username = @Username
            """, new
            {
                Username = username
            });

        return results
            .GroupBy(r => r.Id)
            .Select(g => new User
            {
                Id = g.Key,
                Username = g.First().Username,
                FirstName = g.First().FirstName,
                LastName = g.First().LastName,
                Email = g.First().Email,
                DateRegistered = g.First().DateRegistered,
                HashedPassword = g.First().HashedPassword,
                Preferences = new UserPreferences
                {
                    ThemePreference = (ThemePreference)g.First().ThemePreference,
                    BibleVersion = (BibleVersion)g.First().BibleVersion,
                    CollectionsSort = (CollectionsSort)g.First().CollectionsSort,
                    SubscribedVerseOfDay = g.First().SubscribedVerseOfDay,
                    NotifyFriendsMemorizedPassage = g.First().NotifyFriendsMemorizedPassage,
                    NotifyFriendsPublishedCollection = g.First().NotifyFriendsPublishedCollection,
                    NotifyCollectionSaved = g.First().NotifyCollectionSaved,
                    NotifyNoteLikedCommented = g.First().NotifyNoteLikedCommented,
                    FriendsActivityNotificationsEnabled = g.First().FriendsActivityNotificationsEnabled,
                    OverdueRemindersEnabled = g.First().OverdueRemindersEnabled,
                    TypeOutReference = g.First().TypeOutReference
                },
                ProfileDescription = g.First().ProfileDescription,
                ProfilePictureUrl = g.First().ProfilePictureUrl,
                VersesMemorizedCount = g.First().VersesMemorizedCount,
                Points = g.First().Points,
                Sessions = g.Where(r => r.RefreshTokenHash != null).Select(r => new Session
                {
                    RefreshTokenHash = r.RefreshTokenHash!,
                    LastSeenAt = r.LastSeen,
                    PushNotificationToken = r.PushNotificationToken
                }).ToList()
            }).FirstOrDefault() ?? throw new Exception("User not found");
    }

    public async Task<User?> GetUserByRefreshToken(string refreshTokenHash)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        var results = await conn.QueryAsync<GetUserDto>(
            $"""
            {GetQuery},
            s.refresh_token_hash as RefreshTokenHash,
            s.last_seen as LastSeen,
            s.push_notification_token as PushNotificationToken
            from users u
            join user_preferences up on u.id = up.user_id
            left join device_sessions s on u.id = s.user_id
            join device_sessions ds on u.id = ds.user_id
            where ds.refresh_token_hash = @RefreshTokenHash
            """, new
            {
                RefreshTokenHash = refreshTokenHash
            });

        return results
            .GroupBy(r => r.Id)
            .Select(g => new User
            {
                Id = g.Key,
                Username = g.First().Username,
                FirstName = g.First().FirstName,
                LastName = g.First().LastName,
                Email = g.First().Email,
                DateRegistered = g.First().DateRegistered,
                HashedPassword = g.First().HashedPassword,
                Preferences = new UserPreferences
                {
                    ThemePreference = (ThemePreference)g.First().ThemePreference,
                    BibleVersion = (BibleVersion)g.First().BibleVersion,
                    CollectionsSort = (CollectionsSort)g.First().CollectionsSort,
                    SubscribedVerseOfDay = g.First().SubscribedVerseOfDay,
                    NotifyFriendsMemorizedPassage = g.First().NotifyFriendsMemorizedPassage,
                    NotifyFriendsPublishedCollection = g.First().NotifyFriendsPublishedCollection,
                    NotifyCollectionSaved = g.First().NotifyCollectionSaved,
                    NotifyNoteLikedCommented = g.First().NotifyNoteLikedCommented,
                    FriendsActivityNotificationsEnabled = g.First().FriendsActivityNotificationsEnabled,
                    OverdueRemindersEnabled = g.First().OverdueRemindersEnabled,
                    TypeOutReference = g.First().TypeOutReference
                },
                ProfileDescription = g.First().ProfileDescription,
                ProfilePictureUrl = g.First().ProfilePictureUrl,
                VersesMemorizedCount = g.First().VersesMemorizedCount,
                Points = g.First().Points,
                Sessions = g.Where(r => r.RefreshTokenHash != null).Select(r => new Session
                {
                    RefreshTokenHash = r.RefreshTokenHash!,
                    LastSeenAt = r.LastSeen,
                    PushNotificationToken = r.PushNotificationToken
                }).ToList()
            }).FirstOrDefault() ?? throw new Exception("User not found");
    }

    public async Task<string?> GetPasswordHash(string username)
    {
        var sql = $@"select hashed_password from users where username = @Username";

        using var conn = new NpgsqlConnection(_connectionString);
        var results = await conn.QueryAsync<string>(sql, new { Username = username });
        return results.FirstOrDefault();
    }

    public async Task<bool> CheckUsernameExists(string username)
    {
        var sql = @"SELECT COUNT(1) FROM USERS WHERE USERNAME = :Username";

        using var conn = new NpgsqlConnection(_connectionString);
        var count = await conn.ExecuteScalarAsync<int>(sql, new { Username = username });
        return count > 0;
    }


    // -------------------------------------------------------
    //  Increment
    // -------------------------------------------------------

    public async Task IncrementVersesMemorized(int userId)
    {
        var sql = @"UPDATE USERS SET VERSES_MEMORIZED = NVL(VERSES_MEMORIZED, 0) + 1 WHERE ID = :Id";

        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { Id = userId }, commandType: CommandType.Text);
    }

    public async Task AddPoints(int userId, int points)
    {
        var sql = @"UPDATE USERS SET POINTS = NVL(POINTS, 0) + :points WHERE ID = :userId";

        using var conn = new NpgsqlConnection(_connectionString);
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

        using var conn = new NpgsqlConnection(_connectionString);
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

        using var conn = new NpgsqlConnection(_connectionString);
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

        using var conn = new NpgsqlConnection(_connectionString);
        var results = await conn.QueryAsync<PasswordResetToken>(sql, new { userId, token }, commandType: CommandType.Text);
        return results.FirstOrDefault();
    }

    public async Task DeletePasswordResetToken(int userId)
    {
        var sql = @"DELETE FROM PASSWORD_RESETS WHERE USER_ID = :userId";

        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { userId }, commandType: CommandType.Text);
    }


    // -------------------------------------------------------
    //  Update
    // -------------------------------------------------------

    public async Task UpdatePassword(int userId, string hashedPassword)
    {
        var sql = @"UPDATE USERS SET HASHEDPASSWORD = :hashedPassword WHERE ID = :userId";

        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { hashedPassword, userId }, commandType: CommandType.Text);
    }

    public async Task UpdateUsername(int userId, string newUsername)
    {
        var sql = @"UPDATE USERS SET USERNAME = :NewUsername WHERE ID = :UserId";

        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { NewUsername = newUsername, UserId = userId }, commandType: CommandType.Text);
    }

    public async Task UpdateEmail(int userId, string newEmail)
    {
        var sql = @"UPDATE USERS SET EMAIL = :NewEmail WHERE ID = :UserId";

        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { NewEmail = newEmail, UserId = userId }, commandType: CommandType.Text);
    }

    public async Task UpdateLastSeen(int userId)
    {
        var sql = @"UPDATE USERS SET LAST_SEEN = :CurrentDate WHERE ID = :UserId";

        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { CurrentDate = DateTime.UtcNow, UserId = userId });
    }

    public async Task UpdateDescription(int userId, string description)
    {
        var sql = @"UPDATE USERS SET PROFILE_DESCRIPTION = :description WHERE ID = :UserId";

        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { description = description, UserId = userId }, commandType: CommandType.Text);
    }

    public async Task UpdateName(int userId, string firstName, string lastName)
    {
        var sql = @"UPDATE USERS SET FIRST_NAME = :firstName, LAST_NAME = :lastName WHERE ID = :UserId";

        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { firstName = firstName, lastName = lastName, UserId = userId }, commandType: CommandType.Text);
    }


    // -------------------------------------------------------
    //  Leaderboard
    // -------------------------------------------------------

    public async Task<List<User>> GetLeaderboard(int page, int pageSize)
    {
        var offset = (page - 1) * pageSize;
        var sql = @"SELECT USERNAME, FIRST_NAME as FirstName, LAST_NAME as LastName, EMAIL, AUTH_TOKEN as AuthToken,
                           USER_STATUS as Status, HASHED_PASSWORD as HashedPassword,
                           DATE_REGISTERED as DateRegistered, LAST_SEEN as LastSeen, PROFILE_DESCRIPTION as ProfileDescription,
                           VERSES_MEMORIZED AS VersesMemorizedCount, POINTS, 
                           PROFILE_PICTURE_URL AS ProfilePictureUrl,
                           ROW_NUMBER() OVER (ORDER BY NVL(POINTS, 0) DESC, USERNAME ASC) as rn
                    FROM USERS
                    WHERE rn > :Offset AND rn <= :Offset + :PageSize
                    ORDER BY rn";

        using var conn = new NpgsqlConnection(_connectionString);
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

        using var conn = new NpgsqlConnection(_connectionString);
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
                           USER_STATUS as Status, HASHED_PASSWORD as HashedPassword,
                           DATE_REGISTERED as DateRegistered, LAST_SEEN as LastSeen, PROFILE_DESCRIPTION as ProfileDescription,
                           VERSES_MEMORIZED AS VersesMemorizedCount, POINTS, 
                           PROFILE_PICTURE_URL AS ProfilePictureUrl
                    FROM USERS
                    WHERE UPPER(USERNAME) LIKE UPPER(:Query) 
                       OR UPPER(FIRST_NAME) LIKE UPPER(:Query) 
                       OR UPPER(LAST_NAME) LIKE UPPER(:Query)
                       OR (UPPER(FIRST_NAME) LIKE UPPER(:FirstName) AND UPPER(LAST_NAME) LIKE UPPER(:LastName))
                    ORDER BY USERNAME";

        using var conn = new NpgsqlConnection(_connectionString);
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
