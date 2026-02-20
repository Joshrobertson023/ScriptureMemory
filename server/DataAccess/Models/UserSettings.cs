using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VerseAppLibrary.Enums;

namespace DataAccess.Models;

public sealed class UserSettings
{
    public ThemePreference ThemePreference { get; set; } = ThemePreference.SystemDefault;
    public BibleVersion BibleVersion { get; set; } = BibleVersion.Kjv;
    public CollectionsSort CollectionsSort { get; set; } = CollectionsSort.Newest;
    public bool SubscribedVerseOfDay { get; set; } = true;                  // User is subscribed to the verse of the day
    public bool PushNotificationsEnabled { get; set; } = true;              // User receives push notifications
    public bool NotifyMemorizedVerse { get; set; } = true;                  // Notify friends when memorizing a passage
    public bool NotifyPublishedCollection { get; set; } = true;             // Notify friends when published a collection
    public bool NotifyCollectionSaved { get; set; } = true;                 // Get notified when your collection is saved
    public bool NotifyNoteLiked { get; set; } = true;                       // Get notified when your note is liked
    public bool FriendsActivityNotificationsEnabled { get; set; } = true;   // Get notified of your friend's activity
    public bool StreakRemindersEnabled { get; set; } = true;                // Get reminders to keep your practice streak
    public bool AppBadgesEnabled { get; set; } = true;                      // Show badges on app icon
    public bool PracticeTabBadgesEnabled { get; set; } = true;              // Show badges on practice tab
    public bool TypeOutReference { get; set; } = false;                     // Type out the reference when practicing
}
