using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ScriptureMemory.Server.Tools.Enums;

namespace DataAccess.Models;

public sealed class UserPreferences
{
    public ThemePreference ThemePreference { get; set; } = ThemePreference.SystemDefault;
    public BibleVersion BibleVersion { get; set; } = BibleVersion.Kjv;
    public CollectionsSort CollectionsSort { get; set; } = CollectionsSort.Newest;
    public bool SubscribedVerseOfDay { get; set; } = true;             // User receives push notifications
    public bool NotifyFriendsMemorizedPassage { get; set; } = true;                  // Notify friends when memorizing a passage
    public bool NotifyFriendsPublishedCollection { get; set; } = true;             // Notify friends when published a collection
    public bool NotifyCollectionSaved { get; set; } = true;                 // Get notified when your collection is saved
    public bool NotifyNoteLikedCommented { get; set; } = true;                       // Get notified when your note is liked
    public bool FriendsActivityNotificationsEnabled { get; set; } = true;   // Get notified of your friend's activity
    public bool OverdueRemindersEnabled { get; set; } = true;                     // Get reminders for overdue tasks
    public bool TypeOutReference { get; set; } = false;                     // Type out reference in practice

    public User UserNavigation { get; set; } = null!;
}
