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
    Task CreateUserSettings(UserSettings settings, string username);
    Task<UserSettings> GetUserSettingsFromUsername(string username);
    Task UpdateCollectionsSort(Enums.CollectionsSort sortBy, string username);
    Task UpdateSubscribedVerseOfDay(bool subscribed, string username);
    Task UpdatePushNotificationsEnabled(bool enabled, string username);
    Task UpdateNotifyMemorizedVerse(bool enabled, string username);
    Task UpdateNotifyPublishedCollection(bool enabled, string username);
    Task UpdateNotifyCollectionSaved(bool enabled, string username);
    Task UpdateNotifyNoteLiked(bool enabled, string username);
    Task UpdateFriendsActivityNotifications(bool enabled, string username);
    Task UpdateStreakReminders(bool enabled, string username);
    Task UpdateAppBadgesEnabled(bool enabled, string username);
    Task UpdatePracticeTabBadgesEnabled(bool enabled, string username);
    Task UpdateTypeOutReference(bool enabled, string username);
}
