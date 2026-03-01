using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptureMemoryLibrary;

namespace DataAccess.DataInterfaces;

public interface IUserSettingsData
{
    Task UpdateThemePreference(Enums.ThemePreference preference, int userId);
    Task UpdateBibleVersion(Enums.BibleVersion version, int userId);
    Task CreateUserSettings(UserSettings settings, int userId);
    Task<UserSettings> GetUserSettingsFromUserId(int userId);
    Task UpdateCollectionsSort(Enums.CollectionsSort sortBy, int userId);
    Task UpdateSubscribedVerseOfDay(bool subscribed, int userId);
    Task UpdatePushNotificationsEnabled(bool enabled, int userId);
    Task UpdateNotifyMemorizedVerse(bool enabled, int userId);
    Task UpdateNotifyPublishedCollection(bool enabled, int userId);
    Task UpdateNotifyCollectionSaved(bool enabled, int userId);
    Task UpdateNotifyNoteLiked(bool enabled, int userId);
    Task UpdateFriendsActivityNotifications(bool enabled, int userId);
    Task UpdateStreakReminders(bool enabled, int userId);
    Task UpdateAppBadgesEnabled(bool enabled, int userId);
    Task UpdatePracticeTabBadgesEnabled(bool enabled, int userId);
    Task UpdateTypeOutReference(bool enabled, int userId);
}
