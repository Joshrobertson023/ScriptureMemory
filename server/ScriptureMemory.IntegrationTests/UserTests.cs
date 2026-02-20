using DataAccess.Requests;
using ScriptureMemoryLibrary;
using System.Net.Http.Json;
using static ScriptureMemoryLibrary.Enums;

namespace ScriptureMemory.IntegrationTests;

public class UserTests : BaseIntegrationTest
{
    public UserTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateUser_ShouldAddUser()
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

        var createdUser = await userService.Login(request.Username, request.Password);

        Assert.NotNull(createdUser);
        Assert.Equal(request.Username, createdUser.Username);
        Assert.Equal(request.FirstName, createdUser.FirstName);
        Assert.Equal(request.LastName, createdUser.LastName);
        Assert.Equal(request.Email, createdUser.Email);
        Assert.NotNull(createdUser.HashedPassword);
        Assert.NotNull(createdUser.DateRegistered);
        Assert.NotNull(createdUser.AuthToken);
    }
}