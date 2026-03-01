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
using ScriptureMemoryLibrary;

namespace DataAccess.Data;

public sealed class UserSettingsData : IUserSettingsData
{
    public async Task UpdateThemePreference(Enums.ThemePreference preference, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET THEME = :preference WHERE USER_ID = :userId";
        await conn.ExecuteAsync(sql, new { preference = preference, userId = userId });
    }

    public async Task UpdateBibleVersion(Enums.BibleVersion version, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET BIBLE_VERSION = :version WHERE USER_ID = :userId";
        await conn.ExecuteAsync(sql, new { version = version, userId = userId });
    }

    private readonly IDbConnection conn;

    public UserSettingsData(IDbConnection connection)
    {
        conn = connection;
    }

    public async Task CreateUserSettings(UserSettings settings, int userId)
    {
        var sql = @"INSERT INTO USER_PREFERENCES
                    (USER_ID, THEME, BIBLE_VERSION, COLLECTIONS_SORT, SUBSCRIBED_VOD,
                     PUSH_NOTIFICATIONS_ENABLED, NOTIFY_MEMORIZED_VERSE, NOTIFY_PUBLISHED_COLLECTION,
                     NOTIFY_COLLECTION_SAVED, NOTIFY_NOTE_LIKED, FRIENDS_ACTIVITY_NOTIFICATIONS_ENABLED,
                     STREAK_REMINDERS_ENABLED, APP_BADGES_ENABLED, PRACTICE_TAB_BADGES_ENABLED, TYPE_OUT_REFERENCE)
                    VALUES
                    (:UserId, :Theme, :Version, :CollectionsSort, :SubscribedVod,
                     :PushNotifications, :NotifyMemorizedVerse, :NotifyPublishedCollection,
                     :NotifyCollectionSaved, :NotifyNoteLiked, :FriendsActivityEnabled, 
                     :StreakReminders, :AppBadgesEnabled, :PracticeTabBadgesEnabled, :TypeOutReference)";

        await conn.ExecuteAsync(
            sql,
            new
            {
                UserId = userId,
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

    public async Task<UserSettings> GetUserSettingsFromUserId(int userId)
    {
        var sql = @"SELECT THEME as ThemePreference, 
                           BIBLE_VERSION as BibleVersion, 
                           COLLECTIONS_SORT as CollectionsSort, 
                           SUBSCRIBED_VOD as SubscribedVerseOfDay, 
                           PUSH_NOTIFICATIONS_ENABLED as PushNotificationsEnabled,
                           NOTIFY_MEMORIZED_VERSE as NotifyMemorizedVerse, 
                           NOTIFY_PUBLISHED_COLLECTION as NotifyPublishedCollection, 
                           NOTIFY_COLLECTION_SAVED as NotifyCollectionSaved,
                           NOTIFY_NOTE_LIKED as NotifyNoteLiked, 
                           FRIENDS_ACTIVITY_NOTIFICATIONS_ENABLED as FriendsActivityNotificationsEnabled, 
                           STREAK_REMINDERS_ENABLED as StreakRemindersEnabled,
                           APP_BADGES_ENABLED as AppBadgesEnabled, 
                           PRACTICE_TAB_BADGES_ENABLED as PracticeTabBadgesEnabled, 
                           TYPE_OUT_REFERENCE as TypeOutReference
                    FROM USER_PREFERENCES WHERE USER_ID = :userId";
        var result = await conn.QuerySingleOrDefaultAsync<UserSettings>(sql, new { userId });
        if (result is null)
            throw new Exception($"No user settings found for user id: {userId}");
        return result;
    }

    public async Task UpdateCollectionsSort(Enums.CollectionsSort sortBy, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET COLLECTIONS_SORT = :sortBy WHERE USER_ID = :userId";
        await conn.ExecuteAsync(sql, new { sortBy = sortBy, userId = userId });
    }

    public async Task UpdateSubscribedVerseOfDay(bool subscribed, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET SUBSCRIBED_VOD = :subscribed WHERE USER_ID = :userId";
        await conn.ExecuteAsync(sql, new { subscribed = Convert.ToInt(subscribed), userId = userId });
    }

    public async Task UpdatePushNotificationsEnabled(bool enabled, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET PUSH_NOTIFICATIONS_ENABLED = :enabled WHERE USER_ID = :userId";
        await conn.ExecuteAsync(sql, new { enabled = Convert.ToInt(enabled), userId = userId });
    }

    public async Task UpdateNotifyMemorizedVerse(bool enabled, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET NOTIFY_MEMORIZED_VERSE = :enabled WHERE USER_ID = :userId";
        await conn.ExecuteAsync(sql, new { enabled = Convert.ToInt(enabled), userId = userId });
    }

    public async Task UpdateNotifyPublishedCollection(bool enabled, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET NOTIFY_PUBLISHED_COLLECTION = :enabled WHERE USER_ID = :userId";
        await conn.ExecuteAsync(sql, new { enabled = Convert.ToInt(enabled), userId = userId });
    }

    public async Task UpdateNotifyCollectionSaved(bool enabled, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET NOTIFY_COLLECTION_SAVED = :enabled WHERE USER_ID = :userId";
        await conn.ExecuteAsync(sql, new { enabled = Convert.ToInt(enabled), userId = userId });
    }

    public async Task UpdateNotifyNoteLiked(bool enabled, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET NOTIFY_NOTE_LIKED = :enabled WHERE USER_ID = :userId";
        await conn.ExecuteAsync(sql, new { enabled = Convert.ToInt(enabled), userId = userId });
    }

    public async Task UpdateFriendsActivityNotifications(bool enabled, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET FRIENDS_ACTIVITY_NOTIFICATIONS_ENABLED = :enabled WHERE USER_ID = :userId";
        await conn.ExecuteAsync(sql, new { enabled = Convert.ToInt(enabled), userId = userId });
    }

    public async Task UpdateStreakReminders(bool enabled, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET STREAK_REMINDERS_ENABLED = :enabled WHERE USER_ID = :userId";
        await conn.ExecuteAsync(sql, new { enabled = Convert.ToInt(enabled), userId = userId });
    }

    public async Task UpdateAppBadgesEnabled(bool enabled, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET APP_BADGES_ENABLED = :enabled WHERE USER_ID = :userId";
        await conn.ExecuteAsync(sql, new { enabled = Convert.ToInt(enabled), userId = userId });
    }

    public async Task UpdatePracticeTabBadgesEnabled(bool enabled, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET PRACTICE_TAB_BADGES_ENABLED = :enabled WHERE USER_ID = :userId";
        await conn.ExecuteAsync(sql, new { enabled = Convert.ToInt(enabled), userId = userId });
    }

    public async Task UpdateTypeOutReference(bool enabled, int userId)
    {
        var sql = @"UPDATE USER_PREFERENCES SET TYPE_OUT_REFERENCE = :enabled WHERE USER_ID = :userId";
        await conn.ExecuteAsync(sql, new { enabled = Convert.ToInt(enabled), userId = userId });
    }
}
