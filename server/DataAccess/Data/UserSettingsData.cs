using Dapper;
using DataAccess.DataInterfaces;
using DataAccess.Models;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerseAppLibrary;

namespace DataAccess.Data;

public sealed class UserSettingsData : IUserSettingsData
{
    private readonly IConfiguration _config;
    private readonly string connectionString;

    public UserSettingsData(IConfiguration config)
    {
        _config = config;
        connectionString = _config.GetConnectionString("Default")!;
    }

    public async Task CreateUserSettings(UserSettings settings, string username)
    {
        var sql = @"INSERT INTO USER_PREFERENCES
                    (USERNAME, THEME, VERSION, COLLECTIONS_SORT, SUBSCRIBED_VOD,
                     PUSH_NOTIFICATIONS_ENABLED, NOTIFY_MEMORIZED_VERSE, NOTIFY_PUBLISHED_COLLECTION,
                     NOTIFY_COLLECTION_SAVED, NOTIFY_NOTE_LIKED, FRIENDS_ACTIVITY_NOTIFICATIONS_ENABLED,
                     STREAK_REMINDERS_ENABLED, APP_BADGES_ENABLED, PRACTICE_TAB_BADGES_ENABLED, TYPE_OUT_REFERENCE)
                    VALUES
                    (:Username, :Theme, :Version, :CollectionsSort, :SubscribedVod,
                     :PushNotifications, :NotifyMemorizedVerse, :NotifyPublishedCollection,
                     :NotifyCollectionSaved, :NotifyNoteLiked, :FriendsActivityEnabled, 
                     :StreakReminders, :AppBadgesEnabled, :PracticeTabBadgesEnabled, :TypeOutReference)";

        await using var conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(
            sql,
            new
            {
                Username = username,
                Theme = settings.ThemePreference,
                Version = settings.BibleVersion,
                CollectionsSort = settings.CollectionsSort,
                SubscribedVod = Convert.ToInt(settings.SubscribedVerseOfDay),
                PushNotifications = Convert.ToInt(settings.PushNotificationsEnabled),
                NotifyMemorizedVerse = Convert.ToInt(settings.NotifyMemorizedVerse),
                NotifyPublishedCollection = Convert.ToInt(settings.NotifyPublishedCollection),
                NotifyCollectionSaved = Convert.ToInt(settings.NotifyCollectionSaved),
                NotifyNoteLiked = Convert.ToInt(settings.NotifyNoteLiked),
                FriendsActivityEnabled = Convert.ToInt(settings.FriendsActivityNotificationsEnabled),
                StreakReminders = Convert.ToInt(settings.StreakRemindersEnabled),
                AppBadgesEnabled = Convert.ToInt(settings.AppBadgesEnabled),
                PracticeTabBadgesEnabled = Convert.ToInt(settings.PracticeTabBadgesEnabled),
                TypeOutReference = Convert.ToInt(settings.TypeOutReference)
            });
    }

    public async Task<UserSettings> GetUserSettingsFromUsername(string username)
    {
        var sql = @"SELECT THEME, VERSION, COLLECTIONS_SORT, SUBSCRIBED_VOD, PUSH_NOTIFICATIONS_ENABLED,
                    NOTIFY_MEMORIZED_VERSE, NOTIFY_PUBLISHED_COLLECTION, NOTIFY_COLLECTION_SAVED,
                    NOTIFY_NOTE_LIKED, FRIENDS_ACTIVITY_NOTIFICATIONS_ENABLED, STREAK_REMINDERS_ENABLED,
                    APP_BADGES_ENABLED, PRACTICE_TAB_BADGES_ENABLED, TYPE_OUT_REFERENCE
                    FROM USER_PREFERENCES WHERE USERNAME = :username";
        await using var conn = new OracleConnection(connectionString);
        var result = await conn.QuerySingleOrDefaultAsync<UserSettings>(sql, new { username });
        if (result is null)
            throw new Exception($"No user settings found for username: {username}");
        return result;
    }

    public async Task UpdateCollectionsSort(Enums.CollectionsSort sortBy, string username)
    {
        var sql = @"UPDATE USER_PREFERENCES SET COLLECTIONS_SORT = :sortBy WHERE USERNAME = :username";
        await using var conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { sortBy = sortBy, username = username });
    }

    public async Task UpdateSubscribedVerseOfDay(bool subscribed, string username)
    {
        var sql = @"UPDATE USER_PREFERENCES SET SUBSCRIBED_VOD = :subscribed WHERE USERNAME = :username";
        await using var conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { subscribed = subscribed, username = username });
    }

    public async Task UpdatePushNotificationsEnabled(bool enabled, string username)
    {
        var sql = @"UPDATE USER_PREFERENCES SET PUSH_NOTIFICATIONS_ENABLED = :enabled WHERE USERNAME = :username";
        await using var conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { enabled = enabled, username = username });
    }

    public async Task UpdateNotifyMemorizedVerse(bool enabled, string username)
    {
        var sql = @"UPDATE USERS SET NOTIFY_MEMORIZED_VERSE = :enabled WHERE USERNAME = :username";
        await using var conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { enabled = enabled, username = username });
    }

    public async Task UpdateNotifyPublishedCollection(bool enabled, string username)
    {
        var sql = @"UPDATE USER_PREFERENCES SET NOTIFY_PUBLISHED_COLLECTION = :enabled WHERE USERNAME = :username";
        await using var conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { enabled = enabled, username = username });
    }

    public async Task UpdateNotifyCollectionSaved(bool enabled, string username)
    {
        var sql = @"UPDATE USER_PREFERENCES SET NOTIFY_COLLECTION_SAVED = :enabled WHERE USERNAME = :username";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { enabled = enabled, username = username }, commandType: CommandType.Text);
    }

    public async Task UpdateNotifyNoteLiked(bool enabled, string username)
    {
        var sql = @"UPDATE USER_PREFERENCES SET NOTIFY_NOTE_LIKED = :enabled WHERE USERNAME = :username";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { enabled = enabled, username = username }, commandType: CommandType.Text);
    }

    public async Task UpdateFriendsActivityNotifications(bool enabled, string username)
    {
        var sql = @"UPDATE USER_PREFERENCES SET FRIENDS_ACTIVITY_NOTIFICATIONS_ENABLED = :enabled WHERE USERNAME = :username";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { enabled = enabled, username = username }, commandType: CommandType.Text);
    }

    public async Task UpdateStreakReminders(bool enabled, string username)
    {
        var sql = @"UPDATE USER_PREFERENCES SET STREAK_REMINDERS_ENABLED = :enabled WHERE USERNAME = :username";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { enabled = enabled, username = username }, commandType: CommandType.Text);
    }

    public async Task UpdateAppBadgesEnabled(bool enabled, string username)
    {
        var sql = @"UPDATE USER_PREFERENCES SET APP_BADGES_ENABLED = :enabled WHERE USERNAME = :username";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { enabled = enabled, username = username }, commandType: CommandType.Text);
    }

    public async Task UpdatePracticeTabBadgesEnabled(bool enabled, string username)
    {
        var sql = @"UPDATE USER_PREFERENCES SET PRACTICE_TAB_BADGES_ENABLED = :enabled WHERE USERNAME = :username";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { enabled = enabled, username = username }, commandType: CommandType.Text);
    }

    public async Task UpdateTypeOutReference(bool enabled, string username)
    {
        var sql = @"UPDATE USER_PREFERENCES SET TYPE_OUT_REFERENCE = :enabled WHERE USERNAME = :username";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { enabled = enabled, username = username }, commandType: CommandType.Text);
    }
}
