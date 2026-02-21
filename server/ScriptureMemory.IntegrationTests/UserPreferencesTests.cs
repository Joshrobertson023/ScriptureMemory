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
    private IUserSettingsData settingsData;

    public UserPreferencesTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        settingsData = factory.Services.CreateScope().ServiceProvider.GetRequiredService<IUserSettingsData>();
    }

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

        var createResponse = await api.PostAsJsonAsync("/users", createRequest);
        createResponse.EnsureSuccessStatusCode();

        var loginResponse = await api.PostAsJsonAsync("/users/login/username",
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
        await settingsData.UpdateCollectionsSort(CollectionsSort.LastPracticed, userId);

        var loginAfterCollectionsSort = await api.PostAsJsonAsync("/users/login/username",
            new { Username = createRequest.Username, Password = createRequest.Password });
        var userAfterCollectionsSort = await loginAfterCollectionsSort.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(userAfterCollectionsSort?.Settings);
        Assert.Equal(CollectionsSort.LastPracticed, userAfterCollectionsSort.Settings.CollectionsSort);


        // Toggle Subscribed Verse of Day
        await settingsData.UpdateSubscribedVerseOfDay(false, userId);

        var loginAfterVOD = await api.PostAsJsonAsync("/users/login/username",
            new { Username = createRequest.Username, Password = createRequest.Password });
        var userAfterVOD = await loginAfterVOD.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(userAfterVOD?.Settings);
        Assert.False(userAfterVOD.Settings.SubscribedVerseOfDay);


        // Disable Push Notifications
        await settingsData.UpdatePushNotificationsEnabled(false, userId);

        var loginAfterPushDisable = await api.PostAsJsonAsync("/users/login/username",
            new { Username = createRequest.Username, Password = createRequest.Password });
        var userAfterPushDisable = await loginAfterPushDisable.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(userAfterPushDisable?.Settings);
        Assert.False(userAfterPushDisable.Settings.PushNotificationsEnabled);


        // Disable Notify Memorized Verse
        await settingsData.UpdateNotifyMemorizedVerse(false, userId);

        var loginAfterNoMemorizedNotify = await api.PostAsJsonAsync("/users/login/username",
            new { Username = createRequest.Username, Password = createRequest.Password });
        var userAfterNoMemorizedNotify = await loginAfterNoMemorizedNotify.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(userAfterNoMemorizedNotify?.Settings);
        Assert.False(userAfterNoMemorizedNotify.Settings.NotifyMemorizedVerse);


        // Disable Notify Published Collection
        await settingsData.UpdateNotifyPublishedCollection(false, userId);

        var loginAfterNoPublishedNotify = await api.PostAsJsonAsync("/users/login/username",
            new { Username = createRequest.Username, Password = createRequest.Password });
        var userAfterNoPublishedNotify = await loginAfterNoPublishedNotify.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(userAfterNoPublishedNotify?.Settings);
        Assert.False(userAfterNoPublishedNotify.Settings.NotifyPublishedCollection);


        // Disable Notify Collection Saved
        await settingsData.UpdateNotifyCollectionSaved(false, userId);

        var loginAfterNoCollectionSavedNotify = await api.PostAsJsonAsync("/users/login/username",
            new { Username = createRequest.Username, Password = createRequest.Password });
        var userAfterNoCollectionSavedNotify = await loginAfterNoCollectionSavedNotify.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(userAfterNoCollectionSavedNotify?.Settings);
        Assert.False(userAfterNoCollectionSavedNotify.Settings.NotifyCollectionSaved);


        // Disable Notify Note Liked
        await settingsData.UpdateNotifyNoteLiked(false, userId);

        var loginAfterNoNoteLikedNotify = await api.PostAsJsonAsync("/users/login/username",
            new { Username = createRequest.Username, Password = createRequest.Password });
        var userAfterNoNoteLikedNotify = await loginAfterNoNoteLikedNotify.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(userAfterNoNoteLikedNotify?.Settings);
        Assert.False(userAfterNoNoteLikedNotify.Settings.NotifyNoteLiked);


        // Disable Friends Activity Notifications
        await settingsData.UpdateFriendsActivityNotifications(false, userId);

        var loginAfterNoFriendsActivityNotify = await api.PostAsJsonAsync("/users/login/username",
            new { Username = createRequest.Username, Password = createRequest.Password });
        var userAfterNoFriendsActivityNotify = await loginAfterNoFriendsActivityNotify.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(userAfterNoFriendsActivityNotify?.Settings);
        Assert.False(userAfterNoFriendsActivityNotify.Settings.FriendsActivityNotificationsEnabled);


        // Disable Streak Reminders
        await settingsData.UpdateStreakReminders(false, userId);

        var loginAfterNoStreakReminders = await api.PostAsJsonAsync("/users/login/username",
            new { Username = createRequest.Username, Password = createRequest.Password });
        var userAfterNoStreakReminders = await loginAfterNoStreakReminders.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(userAfterNoStreakReminders?.Settings);
        Assert.False(userAfterNoStreakReminders.Settings.StreakRemindersEnabled);


        // Disable App Badges
        await settingsData.UpdateAppBadgesEnabled(false, userId);

        var loginAfterNoAppBadges = await api.PostAsJsonAsync("/users/login/username",
            new { Username = createRequest.Username, Password = createRequest.Password });
        var userAfterNoAppBadges = await loginAfterNoAppBadges.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(userAfterNoAppBadges?.Settings);
        Assert.False(userAfterNoAppBadges.Settings.AppBadgesEnabled);


        // Disable Practice Tab Badges
        await settingsData.UpdatePracticeTabBadgesEnabled(false, userId);

        var loginAfterNoPracticeTabBadges = await api.PostAsJsonAsync("/users/login/username",
            new { Username = createRequest.Username, Password = createRequest.Password });
        var userAfterNoPracticeTabBadges = await loginAfterNoPracticeTabBadges.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(userAfterNoPracticeTabBadges?.Settings);
        Assert.False(userAfterNoPracticeTabBadges.Settings.PracticeTabBadgesEnabled);


        // Enable Type Out Reference
        await settingsData.UpdateTypeOutReference(true, userId);

        var loginAfterTypeOutRef = await api.PostAsJsonAsync("/users/login/username",
            new { Username = createRequest.Username, Password = createRequest.Password });
        var userAfterTypeOutRef = await loginAfterTypeOutRef.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(userAfterTypeOutRef?.Settings);
        Assert.True(userAfterTypeOutRef.Settings.TypeOutReference);


        // Get user and verify all updates persist
        var finalLoginResponse = await api.PostAsJsonAsync("/users/login/username",
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
