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


    // Test creating an account, loggin in, and updating user attributes, and make sure related logs and notifications are created
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

        result.EnsureSuccessStatusCode();


        // Verify login in with username/password and with token
        var loginServiceResponse = await userService.Login(request.Username, request.Password);
        Assert.NotNull(loginServiceResponse);
        var loginResponse = await api.PostAsJsonAsync("/users/login/username", new { Username = request.Username, Password = request.Password });

        loginResponse.EnsureSuccessStatusCode();
         

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

        var tokenLoginResponse = await api.PostAsJsonAsync("/users/login/token", loggedInUser.AuthToken );
        tokenLoginResponse.EnsureSuccessStatusCode();

        var tokenUser = await tokenLoginResponse.Content.ReadFromJsonAsync<User>();

        Assert.NotNull(tokenUser);
        Assert.Equal(tokenUser.Username, loggedInUser.Username);
        Assert.Equal(tokenUser.AuthToken, loggedInUser.AuthToken);
        Assert.Equal(tokenUser.Settings.ThemePreference, loggedInUser.Settings.ThemePreference);


        // Verify welcome notification is created
        List<Notification> notifications = await notificationContext.GetUserNotifications(loggedInUser.Id);
        
        Assert.NotNull(notifications);
        Assert.Single(notifications);
        Notification latestNotification = notifications.First();
        Assert.Equal(loggedInUser.Id, latestNotification.ReceiverId);
        Assert.Equal(Data.welcomeNotificationBody, latestNotification.Message);
        Assert.Equal(Data.NOTIFICATION_SYSTEM_ID, latestNotification.SenderId);
        Assert.Equal(NotificationType.Welcome, latestNotification.NotificationType);


        // -- Updates -------------------------------

        int userId = loggedInUser.Id;


        // Test update username
        var oldUsername = request.Username;
        var newUsername = $"testuser{Guid.NewGuid().ToString().Substring(0, 8)}";
        var updateUsernameRequest = new UpdateUsernameRequest
        {
            UserId = userId,
            OldUsername = oldUsername,
            NewUsername = newUsername
        };
        var usernameAvailable = await userService.IsUsernameAvailable(newUsername);
        Assert.True(usernameAvailable);
        
        await userService.UpdateUsername(updateUsernameRequest);

        loggedInUser = await userContext.GetUserFromUserId(userId);

        Assert.NotNull(loggedInUser);
        Assert.NotEqual(oldUsername, loggedInUser.Username);


        // Test update description
        var updateDescriptionRequest = new UpdateDescriptionRequest
        {
            Description = "This is my profile description",
            UserId = loggedInUser.Id
        };
        var updateDescriptionResponse = await api.PutAsJsonAsync("/users/description", updateDescriptionRequest);
        var response = await api.PostAsJsonAsync("/users/login/username", new { loggedInUser.Username, request.Password });
        response.EnsureSuccessStatusCode();
        loggedInUser = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(loggedInUser);
        Assert.Equal(updateDescriptionRequest.Description, loggedInUser.ProfileDescription);


        // Test increment verses memorized count
        var incrementVersesMemorizedResponse = await api.PutAsJsonAsync("/users/incrementVersesMemorized", loggedInUser.Id);
        incrementVersesMemorizedResponse.EnsureSuccessStatusCode();

        User beforeIncrement = loggedInUser;
        response = await api.PostAsJsonAsync("/users/login/username", new { loggedInUser.Username, request.Password });
        response.EnsureSuccessStatusCode();
        loggedInUser = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(loggedInUser);
        Assert.Equal(beforeIncrement.VersesMemorizedCount + 1, loggedInUser.VersesMemorizedCount);
        Assert.Equal(beforeIncrement.Username, loggedInUser.Username);


        // Test update email
        var oldEmail = loggedInUser.Username;
        var updateEmailRequest = new UpdateEmailRequest
        {
            UserId = loggedInUser.Id,
            NewEmail = "newemail@gmail.com"
        };
        var loginRequest = new { loggedInUser.Username, request.Password };
        var updateResponse = await api.PutAsJsonAsync("/users/email", updateEmailRequest);
        updateResponse.EnsureSuccessStatusCode();
        response = await api.PostAsJsonAsync("/users/login/username", loginRequest);
        response.EnsureSuccessStatusCode();
        loggedInUser = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(loggedInUser);
        Assert.NotEqual(oldEmail, loggedInUser.Email);


        // Test update name
        var oldName = new
        {
            First = loggedInUser.FirstName,
            Last = loggedInUser.LastName
        };
        var updateNameRequest = new UpdateNameRequest
        {
            UserId = loggedInUser.Id,
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