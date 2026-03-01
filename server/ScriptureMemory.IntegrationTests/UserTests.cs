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
    public async Task CreateUser_CreatesUserAndReturnsUser()
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

        var result = await Api.PostAsJsonAsync("/users", request);
        result.EnsureSuccessStatusCode();
        var createdUser = await result.Content.ReadFromJsonAsync<User>();

        Assert.NotNull(createdUser);
        Assert.Equal(request.Username, createdUser.Username);
        Assert.Equal(request.FirstName, createdUser.FirstName);
        Assert.Equal(request.LastName, createdUser.LastName);
        Assert.Equal(request.Email, createdUser.Email);
        Assert.NotNull(createdUser.HashedPassword);
        Assert.NotNull(createdUser.DateRegistered);
        Assert.NotNull(createdUser.AuthToken);
        Assert.NotNull(createdUser.Settings);
        Assert.Equal(request.BibleVersion, createdUser.Settings.BibleVersion);
        Assert.True(createdUser.Settings.PushNotificationsEnabled);
    }

    [Fact]
    public async Task LoginWithUsernamePassword_ReturnsUser()
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
        await Api.PostAsJsonAsync("/users", request);
        var loginResponse = await Api.PostAsJsonAsync("/users/login/username", new { Username = request.Username, Password = request.Password });
        loginResponse.EnsureSuccessStatusCode();
        var loggedInUser = await loginResponse.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(loggedInUser);
        Assert.Equal(request.Username, loggedInUser.Username);
    }

    [Fact]
    public async Task LoginWithToken_ReturnsUser()
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
        await Api.PostAsJsonAsync("/users", request);
        var loginResponse = await Api.PostAsJsonAsync("/users/login/username", new { Username = request.Username, Password = request.Password });
        var loggedInUser = await loginResponse.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(loggedInUser);
        var tokenLoginResponse = await Api.PostAsJsonAsync("/users/login/token", loggedInUser.AuthToken);
        tokenLoginResponse.EnsureSuccessStatusCode();
        var tokenUser = await tokenLoginResponse.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(tokenUser);
        Assert.Equal(tokenUser.Username, loggedInUser.Username);
        Assert.Equal(tokenUser.AuthToken, loggedInUser.AuthToken);
        Assert.Equal(tokenUser.Settings.ThemePreference, loggedInUser.Settings.ThemePreference);
    }

    [Fact]
    public async Task WelcomeNotification_IsCreatedOnUserCreation()
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
        var result = await Api.PostAsJsonAsync("/users", request);
        var createdUser = await result.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(createdUser);
        List<Notification> notifications = await notificationContext.GetUserNotifications(createdUser.Id);
        Assert.NotNull(notifications);
        Assert.Single(notifications);
        Notification latestNotification = notifications.First();
        Assert.Equal(createdUser.Id, latestNotification.ReceiverId);
        Assert.Equal(Data.welcomeNotificationBody, latestNotification.Message);
        Assert.Equal(Data.NOTIFICATION_SYSTEM_ID, latestNotification.SenderId);
        Assert.Equal(NotificationType.Welcome, latestNotification.NotificationType);
    }

    [Fact]
    public async Task UpdateUsername_ChangesUsername()
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
        var createResult = await Api.PostAsJsonAsync("/users", request);
        var createdUser = await createResult.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(createdUser);
        var newUsername = $"testuser{Guid.NewGuid().ToString().Substring(0, 8)}";
        var existsResponse = await Api.GetAsync($"/users/exists/{newUsername}");
        var exists = await existsResponse.Content.ReadFromJsonAsync<bool>();
        Assert.False(exists); // Username should be available
        var updateUsernameRequest = new UpdateUsernameRequest
        {
            UserId = createdUser.Id,
            OldUsername = createdUser.Username,
            NewUsername = newUsername
        };
        var updateResponse = await Api.PutAsJsonAsync("/users/username", updateUsernameRequest);
        updateResponse.EnsureSuccessStatusCode();
        var loginResponse = await Api.PostAsJsonAsync("/users/login/username", new { Username = newUsername, request.Password });
        loginResponse.EnsureSuccessStatusCode();
        var updatedUser = await loginResponse.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(updatedUser);
        Assert.Equal(newUsername, updatedUser.Username);
    }

    [Fact]
    public async Task UpdateDescription_ChangesProfileDescription()
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
        await Api.PostAsJsonAsync("/users", request);
        var loginResponse = await Api.PostAsJsonAsync("/users/login/username", new { Username = request.Username, Password = request.Password });
        var loggedInUser = await loginResponse.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(loggedInUser);
        var updateDescriptionRequest = new UpdateDescriptionRequest
        {
            Description = "This is my profile description",
            UserId = loggedInUser.Id
        };
        var updateDescriptionResponse = await Api.PutAsJsonAsync("/users/description", updateDescriptionRequest);
        updateDescriptionResponse.EnsureSuccessStatusCode();
        var response = await Api.PostAsJsonAsync("/users/login/username", new { loggedInUser.Username, request.Password });
        response.EnsureSuccessStatusCode();
        var updatedUser = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(updatedUser);
        Assert.Equal(updateDescriptionRequest.Description, updatedUser.ProfileDescription);
    }

    [Fact]
    public async Task IncrementVersesMemorized_IncrementsCount()
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
        await Api.PostAsJsonAsync("/users", request);
        var loginResponse = await Api.PostAsJsonAsync("/users/login/username", new { Username = request.Username, Password = request.Password });
        var loggedInUser = await loginResponse.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(loggedInUser);
        var beforeCount = loggedInUser.VersesMemorizedCount;
        var incrementResponse = await Api.PutAsJsonAsync("/users/incrementVersesMemorized", loggedInUser.Id);
        incrementResponse.EnsureSuccessStatusCode();
        var response = await Api.PostAsJsonAsync("/users/login/username", new { loggedInUser.Username, request.Password });
        var updatedUser = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(updatedUser);
        Assert.Equal(beforeCount + 1, updatedUser.VersesMemorizedCount);
    }

    [Fact]
    public async Task UpdateEmail_ChangesEmail()
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
        await Api.PostAsJsonAsync("/users", request);
        var loginResponse = await Api.PostAsJsonAsync("/users/login/username", new { Username = request.Username, Password = request.Password });
        var loggedInUser = await loginResponse.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(loggedInUser);
        var updateEmailRequest = new UpdateEmailRequest
        {
            UserId = loggedInUser.Id,
            NewEmail = "newemail@gmail.com"
        };
        var updateResponse = await Api.PutAsJsonAsync("/users/email", updateEmailRequest);
        updateResponse.EnsureSuccessStatusCode();
        var response = await Api.PostAsJsonAsync("/users/login/username", new { loggedInUser.Username, request.Password });
        var updatedUser = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(updatedUser);
        Assert.NotEqual(request.Email, updatedUser.Email);
        Assert.Equal(updateEmailRequest.NewEmail, updatedUser.Email);
    }

    [Fact]
    public async Task UpdateName_ChangesName()
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
        await Api.PostAsJsonAsync("/users", request);
        var loginResponse = await Api.PostAsJsonAsync("/users/login/username", new { Username = request.Username, Password = request.Password });
        var loggedInUser = await loginResponse.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(loggedInUser);
        var updateNameRequest = new UpdateNameRequest
        {
            UserId = loggedInUser.Id,
            FirstName = "John",
            LastName = "Doe"
        };
        var updateResponse = await Api.PutAsJsonAsync("/users/name", updateNameRequest);
        updateResponse.EnsureSuccessStatusCode();
        var response = await Api.PostAsJsonAsync("/users/login/username", new { loggedInUser.Username, request.Password });
        var updatedUser = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(updatedUser);
        Assert.Equal(updateNameRequest.FirstName, updatedUser.FirstName);
        Assert.Equal(updateNameRequest.LastName, updatedUser.LastName);
    }
}