using Azure;
using DataAccess.Data;
using DataAccess.Models;
using DataAccess.Requests;
using Microsoft.AspNetCore.Identity.Data;
using ScriptureMemoryLibrary;
using System.Net.Http.Json;
using static ScriptureMemoryLibrary.Enums;
using System;

namespace ScriptureMemory.IntegrationTests;

public class UserTests : BaseIntegrationTest
{
    public UserTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateUser_ShouldCreateUserAccount()
    {
        var request = new CreateUserRequest
        {
            Username = $"testuser{Guid.NewGuid().ToString().Substring(0, 8)}",
            FirstName = "Test",
            LastName = "User",
            Email = $"testuser@gmail.com",
            Password = "password1234455",
            BibleVersion = BibleVersion.Kjv
        };

        var result = await client.PostAsJsonAsync("/users", request);

        if (!result.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"{await result.Content.ReadAsStringAsync()}");
        }


        // Get the new user and assert
        var loginResponse = await client.PostAsJsonAsync("/users/username", new { Username = request.Username, Password = request.Password });

        if (!loginResponse.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"{await loginResponse.Content.ReadAsStringAsync()}");
        }

        var loggedInUser = await loginResponse.Content.ReadFromJsonAsync<User>();

        Assert.NotNull(loggedInUser);
        Assert.Equal(request.Username, loggedInUser.Username);
        Assert.Equal(request.FirstName, loggedInUser.FirstName);
        Assert.Equal(request.LastName, loggedInUser.LastName);
        Assert.Equal(request.Email, loggedInUser.Email);
        Assert.NotNull(loggedInUser.HashedPassword);
        Assert.NotNull(loggedInUser.DateRegistered);
        Assert.NotNull(loggedInUser.AuthToken);
        Assert.NotNull(loggedInUser.Settings);
        Assert.Equal(request.BibleVersion, loggedInUser.Settings.BibleVersion);
        Assert.True(loggedInUser.Settings.PushNotificationsEnabled);


        // Get the created log and assert
        PagedLogs<ActivityLog> logResponse = await activityLogContext.GetByUser(request.Username);
        ActivityLog loginLog = logResponse.Items.First();
        ActivityLog registerLog = logResponse.Items.Last();

        Assert.NotNull(logResponse);
        Assert.Equal(request.Username, registerLog.Username);
        Assert.Equal(ActionType.Register, registerLog.ActionType);
        Assert.Equal(EntityType.User, registerLog.EntityType);
        Assert.Equal("Created account", registerLog.ContextDescription);


        // Get the welcome notification and assert
        List<Notification> notifications = await notificationContext.GetUserNotifications(request.Username);
        Notification latestNotification = notifications.First();

        Assert.NotNull(notifications);
        Assert.Single(notifications);
        Assert.Equal(request.Username, latestNotification.Receiver);
        Assert.Equal(Data.welcomeNotificationBody, latestNotification.Message);
        Assert.Equal(Data.notifificationSystemName, latestNotification.Sender);
        Assert.Equal(NotificationType.Welcome, latestNotification.NotificationType);
    }
}