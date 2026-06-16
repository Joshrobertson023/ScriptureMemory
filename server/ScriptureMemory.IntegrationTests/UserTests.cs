using DataAccess.Data;
using DataAccess.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.DependencyInjection;
using ScriptureMemory.Server.DataAccess.Models;
using VerseAppNew.Server.Services;

namespace ScriptureMemory.IntegrationTests;

public class UserTests : BaseIntegrationTest
{
    public UserTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }
    
    private record CreateUserDto(User user, string jwt);

    [Fact]
    public async Task CreateUser_Should_Create_User_Jwt_And_Preferences()
    {
        var userContext = _scope.ServiceProvider.GetRequiredService<IUserData>();
        var userService = _scope.ServiceProvider.GetRequiredService<UserService>();

        var results = await userService.CreateUser(new Session()
        {
            DeviceName = "Josh's iPhone",
            DeviceId = "aa:aa:aa:aa",
            Model = "iPhone 15 Pro"
        });
        
        Assert.IsType<CreateUserDto>(results);
        Assert.NotEqual(0, results.User.UserId);
        Assert.True(results.User.Preferences.NotifyCollectionSaved);
        Assert.NotEmpty(results.Jwt);
    }
}