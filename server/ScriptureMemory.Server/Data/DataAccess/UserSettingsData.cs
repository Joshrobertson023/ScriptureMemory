using Dapper;
using DataAccess.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptureMemory.Server.Tools;
using Npgsql;

namespace DataAccess.Data;

public sealed class UserSettingsData
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;

    public UserSettingsData(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetConnectionString("PostgresConnection")
            ?? throw new InvalidOperationException("Connection string 'PostgresConnection' not found");
    }

    public async Task UpdateThemePreference(Enums.ThemePreference preference, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET THEME = :preference WHERE USER_ID = :userId";
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { preference = preference, userId = userId });
    }

    public async Task UpdateBibleVersion(Enums.BibleVersion version, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET BIBLE_VERSION = :version WHERE USER_ID = :userId";
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { version = version, userId = userId });
    }

    public async Task UpdateCollectionsSort(Enums.CollectionsSort sortBy, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET COLLECTIONS_SORT = :sortBy WHERE USER_ID = :userId";
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { sortBy = sortBy, userId = userId });
    }

    public async Task UpdateSubscribedVerseOfDay(bool subscribed, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET SUBSCRIBED_VOD = :subscribed WHERE USER_ID = :userId";
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { subscribed = Convert.ToInt(subscribed), userId = userId });
    }

    public async Task UpdatePushNotificationsEnabled(bool enabled, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET PUSH_NOTIFICATIONS_ENABLED = :enabled WHERE USER_ID = :userId";
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { enabled = Convert.ToInt(enabled), userId = userId });
    }

    public async Task UpdateNotifyMemorizedVerse(bool enabled, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET NOTIFY_MEMORIZED_VERSE = :enabled WHERE USER_ID = :userId";
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { enabled = Convert.ToInt(enabled), userId = userId });
    }

    public async Task UpdateNotifyPublishedCollection(bool enabled, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET NOTIFY_PUBLISHED_COLLECTION = :enabled WHERE USER_ID = :userId";
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { enabled = Convert.ToInt(enabled), userId = userId });
    }

    public async Task UpdateNotifyCollectionSaved(bool enabled, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET NOTIFY_COLLECTION_SAVED = :enabled WHERE USER_ID = :userId";
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { enabled = Convert.ToInt(enabled), userId = userId });
    }

    public async Task UpdateNotifyNoteLiked(bool enabled, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET NOTIFY_NOTE_LIKED = :enabled WHERE USER_ID = :userId";
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { enabled = Convert.ToInt(enabled), userId = userId });
    }

    public async Task UpdateFriendsActivityNotifications(bool enabled, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET FRIENDS_ACTIVITY_NOTIFICATIONS_ENABLED = :enabled WHERE USER_ID = :userId";
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { enabled = Convert.ToInt(enabled), userId = userId });
    }

    public async Task UpdateStreakReminders(bool enabled, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET STREAK_REMINDERS_ENABLED = :enabled WHERE USER_ID = :userId";
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { enabled = Convert.ToInt(enabled), userId = userId });
    }

    public async Task UpdateAppBadgesEnabled(bool enabled, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET APP_BADGES_ENABLED = :enabled WHERE USER_ID = :userId";
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { enabled = Convert.ToInt(enabled), userId = userId });
    }

    public async Task UpdatePracticeTabBadgesEnabled(bool enabled, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET PRACTICE_TAB_BADGES_ENABLED = :enabled WHERE USER_ID = :userId";
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { enabled = Convert.ToInt(enabled), userId = userId });
    }

    public async Task UpdateTypeOutReference(bool enabled, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET TYPE_OUT_REFERENCE = :enabled WHERE USER_ID = :userId";
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { enabled = Convert.ToInt(enabled), userId = userId });
    }
}
