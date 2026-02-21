using Azure;
using DataAccess.Data;
using DataAccess.Models;
using DataAccess.Requests;
using Microsoft.AspNetCore.Identity.Data;
using ScriptureMemoryLibrary;
using System.Net.Http.Json;
using static ScriptureMemoryLibrary.Enums;
using System;
using DataAccess.Requests.UpdateRequests;

namespace ScriptureMemory.IntegrationTests;

public class UserTests : BaseIntegrationTest
{
    public UserTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task UserTest_CreateLoginUpdateUser()
    {

        // -- Account Creation -------------------------------


        var request = new CreateUserRequest
        {
            Username = $"testuser{Guid.NewGuid().ToString().Substring(0, 8)}",
            FirstName = "Test",
            LastName = "User",
            Email = $"testuser@gmail.com",
            Password = "password1234455",
            BibleVersion = BibleVersion.Kjv
        };

        var result = await api.PostAsJsonAsync("/users", request);

        if (!result.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"{await result.Content.ReadAsStringAsync()}");
        }


        // Verify login in with username/password and with token
        var loginResponse = await api.PostAsJsonAsync("/users/login/username", new { Username = request.Username, Password = request.Password });

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

        var tokenLoginResponse = await api.PostAsJsonAsync("/users/login/token", new { loggedInUser.AuthToken });

        if (!tokenLoginResponse.IsSuccessStatusCode)
            throw new HttpRequestException($"{await loginResponse.Content.ReadAsStringAsync()}");

        var tokenUser = await tokenLoginResponse.Content.ReadFromJsonAsync<User>();

        Assert.NotNull(tokenUser);
        Assert.Equal(tokenUser.Username, loggedInUser.Username);
        Assert.Equal(tokenUser.AuthToken, loggedInUser.AuthToken);
        Assert.Equal(tokenUser.Settings.ThemePreference, loggedInUser.Settings.ThemePreference);


        // Verify register log is created
        PagedLogs<ActivityLog> logResponse = await activityLogContext.GetByUser(request.Username);
        ActivityLog loginLog = logResponse.Items.First();
        ActivityLog registerLog = logResponse.Items.Last();

        Assert.NotNull(logResponse);
        Assert.Equal(request.Username, registerLog.Username);
        Assert.Equal(ActionType.Register, registerLog.ActionType);
        Assert.Equal(EntityType.User, registerLog.EntityType);
        Assert.Equal("Created account", registerLog.ContextDescription);


        // Verify welcome notification is created
        List<Notification> notifications = await notificationContext.GetUserNotifications(request.Username);
        Notification latestNotification = notifications.First();

        Assert.NotNull(notifications);
        Assert.Single(notifications);
        Assert.Equal(request.Username, latestNotification.Receiver);
        Assert.Equal(Data.welcomeNotificationBody, latestNotification.Message);
        Assert.Equal(Data.notifificationSystemName, latestNotification.Sender);
        Assert.Equal(NotificationType.Welcome, latestNotification.NotificationType);


        // -- Updates -------------------------------


        // Test update username
        var oldUsername = request.Username;
        var updateUsernameRequest = new UpdateUsernameRequest
        {
            OldUsername = request.Username,
            NewUsername = $"testuser{Guid.NewGuid().ToString().Substring(0, 8)}"
        };
        var updateUsernameResponse = await api.PutAsJsonAsync("/users/username", updateUsernameRequest);
        var response = await api.PostAsJsonAsync("/users/login/username", new { request.Username, request.Password });
        response.EnsureSuccessStatusCode();
        loggedInUser = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(loggedInUser);
        Assert.NotEqual(oldUsername, loggedInUser.Username);

        logResponse = await activityLogContext.GetByUser(loggedInUser.Username);
        Assert.NotEmpty(logResponse.Items);
        var latestLog = logResponse.Items.First();
        Assert.Equal(ActionType.Update, latestLog.ActionType);
        Assert.Equal("User updated username", latestLog.ContextDescription);


        // Test update description
        var updateDescriptionRequest = new UpdateDescriptionRequest
        {
            Description = "This is my profile description",
            Username = loggedInUser.Username
        };
        var updateDescriptionResponse = await api.PutAsJsonAsync("/users/username", updateDescriptionRequest);
        response = await api.PostAsJsonAsync("/users/login/username", new { request.Username, request.Password });
        response.EnsureSuccessStatusCode();
        loggedInUser = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(loggedInUser);
        Assert.Equal(updateDescriptionRequest.Description, loggedInUser.ProfileDescription);

        logResponse = await activityLogContext.GetByUser(loggedInUser.Username);
        Assert.NotEmpty(logResponse.Items);
        latestLog = logResponse.Items.First();
        Assert.Equal(ActionType.Update, latestLog.ActionType);
        Assert.Equal("User updated profile description", latestLog.ContextDescription);


        // Test increment verses memorized count
        var incrementVersesMemorizedResponse = await api.PutAsJsonAsync("/users/incrementVersesMemorized", request.Username);
        incrementVersesMemorizedResponse.EnsureSuccessStatusCode();

        User beforeIncrement = loggedInUser;
        response = await api.PostAsJsonAsync("users/login/username", new { request.Username, request.Password });
        response.EnsureSuccessStatusCode();
        loggedInUser = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(loggedInUser);
        Assert.Equal(beforeIncrement.VersesMemorizedCount + 1, loggedInUser.VersesMemorizedCount);
        Assert.Equal(beforeIncrement.Username, loggedInUser.Username);


        // Test update email
        var oldEmail = loggedInUser.Username;
        var updateEmailRequest = new UpdateEmailRequest
        {
            Username = loggedInUser.Username,
            NewEmail = "newemail@gmail.com"
        };
        var loginRequest = new { loggedInUser.Username, loggedInUser.HashedPassword };
        var updateResponse = await api.PutAsJsonAsync("/users/email", updateEmailRequest);
        updateResponse.EnsureSuccessStatusCode();
        response = await api.PostAsJsonAsync("/users/login/username", loginRequest);
        response.EnsureSuccessStatusCode();
        loggedInUser = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(loggedInUser);
        Assert.NotEqual(oldEmail, loggedInUser.Email);

        logResponse = await activityLogContext.GetByUser(loggedInUser.Username);
        Assert.NotEmpty(logResponse.Items);
        latestLog = logResponse.Items.First();
        Assert.Equal(ActionType.Update, latestLog.ActionType);
        Assert.Equal("User updated email", latestLog.ContextDescription);


        // Test update name
        var oldName = new
        {
            First = loggedInUser.FirstName,
            Last = loggedInUser.LastName
        };
        var updateNameRequest = new UpdateNameRequest
        {
            Username = loggedInUser.Username,
            FirstName = "John",
            LastName = "Doe"
        };
        updateResponse = await api.PutAsJsonAsync("/users/name", updateNameRequest);
        updateResponse.EnsureSuccessStatusCode();
        response = await api.PostAsJsonAsync("/users/login/username", loginRequest);
        response.EnsureSuccessStatusCode();
        loggedInUser = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(loggedInUser);
        Assert.NotEqual(oldName.First, loggedInUser.FirstName);
        Assert.NotEqual(oldName.Last,  loggedInUser.LastName);
    }
}