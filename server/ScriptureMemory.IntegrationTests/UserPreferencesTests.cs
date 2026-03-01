using DataAccess.Data;
using DataAccess.DataInterfaces;
using DataAccess.Models;
using DataAccess.Requests;
using Microsoft.Extensions.DependencyInjection;
using ScriptureMemoryLibrary;
using System.Net.Http.Json;
using static ScriptureMemoryLibrary.Enums;

namespace ScriptureMemory.IntegrationTests;

public class UserPreferencesTests : BaseIntegrationTest
{

    public UserPreferencesTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    // Test all preference updates
    [Fact]
    public async Task UserPreferencesTest_UpdateAllPreferences()
    {
        var createRequest = new CreateUserRequest
        {
            Username = $"prefuser{Guid.NewGuid().ToString().Substring(0, 8)}",
            FirstName = "Preference",
            LastName = "Tester",
            Email = $"prefuser{Guid.NewGuid().ToString().Substring(0, 8)}@gmail.com",
            Password = "password1234455",
            BibleVersion = BibleVersion.Kjv
        };

        var createResponse = await Api.PostAsJsonAsync("/users", createRequest);
        createResponse.EnsureSuccessStatusCode();

        var loginResponse = await Api.PostAsJsonAsync("/users/login/username",
            new { Username = createRequest.Username, Password = createRequest.Password });
        loginResponse.EnsureSuccessStatusCode();

        var user = await loginResponse.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(user);
        int userId = user.Id;

        Assert.NotNull(user.Settings);
        Assert.Equal(BibleVersion.Kjv, user.Settings.BibleVersion);
        Assert.Equal(ThemePreference.SystemDefault, user.Settings.ThemePreference);
        Assert.True(user.Settings.PushNotificationsEnabled);
        Assert.True(user.Settings.NotifyMemorizedVerse);
        Assert.True(user.Settings.NotifyPublishedCollection);
        Assert.True(user.Settings.NotifyCollectionSaved);
        Assert.True(user.Settings.NotifyNoteLiked);
        Assert.True(user.Settings.FriendsActivityNotificationsEnabled);
        Assert.True(user.Settings.StreakRemindersEnabled);
        Assert.True(user.Settings.AppBadgesEnabled);
        Assert.True(user.Settings.PracticeTabBadgesEnabled);
        Assert.False(user.Settings.TypeOutReference);


        // Update Collections Sort Preference
        await Api.PutAsJsonAsync("/userpreferences/collectionssort", new {
            UserId = userId,
            SortBy = CollectionsSort.LastPracticed
        });

        // Toggle Subscribed Verse of Day
        await Api.PutAsJsonAsync("/userpreferences/subscribedverseofday", new {
            UserId = userId,
            Enabled = false
        });

        // Disable Push Notifications
        await Api.PutAsJsonAsync("/userpreferences/pushnotifications", new {
            UserId = userId,
            Enabled = false
        });

        // Disable Notify Memorized Verse
        await Api.PutAsJsonAsync("/userpreferences/notifymemorizedverse", new {
            UserId = userId,
            Enabled = false
        });

        // Disable Notify Published Collection
        await Api.PutAsJsonAsync("/userpreferences/notifypublishedcollection", new {
            UserId = userId,
            Enabled = false
        });

        // Disable Notify Collection Saved
        await Api.PutAsJsonAsync("/userpreferences/notifycollectionsaved", new {
            UserId = userId,
            Enabled = false
        });

        // Disable Notify Note Liked
        await Api.PutAsJsonAsync("/userpreferences/notifynoteliked", new {
            UserId = userId,
            Enabled = false
        });

        // Disable Friends Activity Notifications
        await Api.PutAsJsonAsync("/userpreferences/friendsactivitynotifications", new {
            UserId = userId,
            Enabled = false
        });

        // Disable Streak Reminders
        await Api.PutAsJsonAsync("/userpreferences/streakreminders", new {
            UserId = userId,
            Enabled = false
        });

        // Disable App Badges
        await Api.PutAsJsonAsync("/userpreferences/appbadgesenabled", new {
            UserId = userId,
            Enabled = false
        });

        // Disable Practice Tab Badges
        await Api.PutAsJsonAsync("/userpreferences/practicetabbadgesenabled", new {
            UserId = userId,
            Enabled = false
        });

        // Enable Type Out Reference
        await Api.PutAsJsonAsync("/userpreferences/typeoutreference", new {
            UserId = userId,
            Enabled = true
        });

        // Get user and verify all updates persist
        var finalLoginResponse = await Api.PostAsJsonAsync("/users/login/username",
            new { Username = createRequest.Username, Password = createRequest.Password });
        var finalUser = await finalLoginResponse.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(finalUser?.Settings);
        Assert.Equal(CollectionsSort.LastPracticed, finalUser.Settings.CollectionsSort);
        Assert.False(finalUser.Settings.SubscribedVerseOfDay);
        Assert.False(finalUser.Settings.PushNotificationsEnabled);
        Assert.False(finalUser.Settings.NotifyMemorizedVerse);
        Assert.False(finalUser.Settings.NotifyPublishedCollection);
        Assert.False(finalUser.Settings.NotifyCollectionSaved);
        Assert.False(finalUser.Settings.NotifyNoteLiked);
        Assert.False(finalUser.Settings.FriendsActivityNotificationsEnabled);
        Assert.False(finalUser.Settings.StreakRemindersEnabled);
        Assert.False(finalUser.Settings.AppBadgesEnabled);
        Assert.False(finalUser.Settings.PracticeTabBadgesEnabled);
        Assert.True(finalUser.Settings.TypeOutReference);
    }
}
