using DataAccess.Models;
using DataAccess.Requests;
using Microsoft.AspNetCore.Identity.Data;
using ScriptureMemoryLibrary;
using System.Net.Http.Json;
using static ScriptureMemoryLibrary.Enums;

namespace ScriptureMemory.IntegrationTests;

public class UserTests : BaseIntegrationTest
{
    public UserTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateUser_ShouldCreateUserAccount()
    {
        var request = new CreateUserRequest
        {
            Username = "testuser123",
            FirstName = "Test",
            LastName = "User",
            Email = "testuser@gmail.com",
            Password = "password1234455",
            BibleVersion = BibleVersion.Kjv
        };

        var result = await client.PostAsJsonAsync("/users", request);

        var loginResponse = await client.PostAsJsonAsync("/auth/login", new { Username = request.Username, Password = request.Password });
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
    }
}