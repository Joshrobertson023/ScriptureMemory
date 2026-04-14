using DataAccess.Data;
using DataAccess.Models;
using DataAccess.Requests;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using static ScriptureMemory.Server.Tools.Enums;

namespace ScriptureMemory.IntegrationTests;

public class UserPreferencesTests : BaseIntegrationTest
{

    public UserPreferencesTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    // Test all preference updates
    //[Fact]
    //public async Task UserPreferencesTest_UpdateAllPreferences()
    //{
    //    var createRequest = new CreateUserRequest
    //    {
    //        Username = $"prefuser{Guid.NewGuid().ToString().Substring(0, 8)}",
    //        FirstName = "Preference",
    //        LastName = "Tester",
    //        Email = $"prefuser{Guid.NewGuid().ToString().Substring(0, 8)}@gmail.com",
    //        Password = "password1234455",
    //        BibleVersion = BibleVersion.Kjv
    //    };

    //    var createResponse = await Api.PostAsJsonAsync("/users", createRequest);
    //    createResponse.EnsureSuccessStatusCode();

    //    var loginResponse = await Api.PostAsJsonAsync("/users/login/username",
    //        new { Username = createRequest.Username, Password = createRequest.Password });
    //    loginResponse.EnsureSuccessStatusCode();

    //    var user = await loginResponse.Content.ReadFromJsonAsync<User>();
    //    Assert.NotNull(user);
    //    int userId = user.Id;

    //    Assert.NotNull(user.Preferences);
    //    Assert.Equal(BibleVersion.Kjv, user.Preferences.BibleVersion);
    //    Assert.Equal(ThemePreference.SystemDefault, user.Preferences.ThemePreference);
    //    Assert.True(user.Preferences.PushNotificationsEnabled);
    //    Assert.True(user.Preferences.NotifyMemorizedVerse);
    //    Assert.True(user.Preferences.NotifyPublishedCollection);
    //    Assert.True(user.Preferences.NotifyCollectionSaved);
    //    Assert.True(user.Preferences.NotifyNoteLiked);
    //    Assert.True(user.Preferences.FriendsActivityNotificationsEnabled);
    //    Assert.True(user.Preferences.StreakRemindersEnabled);
    //    Assert.True(user.Preferences.AppBadgesEnabled);
    //    Assert.True(user.Preferences.PracticeTabBadgesEnabled);
    //    Assert.False(user.Preferences.TypeOutReference);


    //    // Update Collections Sort Preference
    //    await Api.PutAsJsonAsync("/userpreferences/collectionssort", new {
    //        UserId = userId,
    //        SortBy = CollectionsSort.LastPracticed
    //    });

    //    // Toggle Subscribed Verse of Day
    //    await Api.PutAsJsonAsync("/userpreferences/subscribedverseofday", new {
    //        UserId = userId,
    //        Enabled = false
    //    });

    //    // Disable Push Notifications
    //    await Api.PutAsJsonAsync("/userpreferences/pushnotifications", new {
    //        UserId = userId,
    //        Enabled = false
    //    });

    //    // Disable Notify Memorized Verse
    //    await Api.PutAsJsonAsync("/userpreferences/notifymemorizedverse", new {
    //        UserId = userId,
    //        Enabled = false
    //    });

    //    // Disable Notify Published Collection
    //    await Api.PutAsJsonAsync("/userpreferences/notifypublishedcollection", new {
    //        UserId = userId,
    //        Enabled = false
    //    });

    //    // Disable Notify Collection Saved
    //    await Api.PutAsJsonAsync("/userpreferences/notifycollectionsaved", new {
    //        UserId = userId,
    //        Enabled = false
    //    });

    //    // Disable Notify Note Liked
    //    await Api.PutAsJsonAsync("/userpreferences/notifynoteliked", new {
    //        UserId = userId,
    //        Enabled = false
    //    });

    //    // Disable Friends Activity Notifications
    //    await Api.PutAsJsonAsync("/userpreferences/friendsactivitynotifications", new {
    //        UserId = userId,
    //        Enabled = false
    //    });

    //    // Disable Streak Reminders
    //    await Api.PutAsJsonAsync("/userpreferences/streakreminders", new {
    //        UserId = userId,
    //        Enabled = false
    //    });

    //    // Disable App Badges
    //    await Api.PutAsJsonAsync("/userpreferences/appbadgesenabled", new {
    //        UserId = userId,
    //        Enabled = false
    //    });

    //    // Disable Practice Tab Badges
    //    await Api.PutAsJsonAsync("/userpreferences/practicetabbadgesenabled", new {
    //        UserId = userId,
    //        Enabled = false
    //    });

    //    // Enable Type Out Reference
    //    await Api.PutAsJsonAsync("/userpreferences/typeoutreference", new {
    //        UserId = userId,
    //        Enabled = true
    //    });

    //    // Get user and verify all updates persist
    //    var finalLoginResponse = await Api.PostAsJsonAsync("/users/login/username",
    //        new { Username = createRequest.Username, Password = createRequest.Password });
    //    var finalUser = await finalLoginResponse.Content.ReadFromJsonAsync<User>();
    //    Assert.NotNull(finalUser?.Preferences);
    //    Assert.Equal(CollectionsSort.LastPracticed, finalUser.Preferences.CollectionsSort);
    //    Assert.False(finalUser.Preferences.SubscribedVerseOfDay);
    //    Assert.False(finalUser.Preferences.PushNotificationsEnabled);
    //    Assert.False(finalUser.Preferences.NotifyMemorizedVerse);
    //    Assert.False(finalUser.Preferences.NotifyPublishedCollection);
    //    Assert.False(finalUser.Preferences.NotifyCollectionSaved);
    //    Assert.False(finalUser.Preferences.NotifyNoteLiked);
    //    Assert.False(finalUser.Preferences.FriendsActivityNotificationsEnabled);
    //    Assert.False(finalUser.Preferences.StreakRemindersEnabled);
    //    Assert.False(finalUser.Preferences.AppBadgesEnabled);
    //    Assert.False(finalUser.Preferences.PracticeTabBadgesEnabled);
    //    Assert.True(finalUser.Preferences.TypeOutReference);
    //}
}
