using DataAccess.DataInterfaces;
using DataAccess.Models;
using DataAccess.Requests;
using DataAccess.Requests.UpdateRequests;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using VerseAppLibrary;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static VerseAppLibrary.Enums;

namespace VerseAppNew.Server.Services;

public interface IUserService
{
    Task CreateUserFromRequest(CreateUserRequest request);
    Task<IResult> Login(string username, string password);
    Task<IResult> Login(string token);
    Task<IResult> UpdateUsername(UpdateUsernameRequest request);
    Task<IResult> UpdateEmail(UpdateEmailRequest request);
    Task<IResult> UpdateName(UpdateNameRequest request);
    Task<IResult> UpdateDescription(UpdateDescriptionRequest request);
}

public sealed class UserService : IUserService
{
    private readonly IUserData userContext;
    private readonly IUserSettingsData settingsContext;
    //private readonly IPaidData paidContext;
    //private readonly INotificationService notificationService;
    private readonly IActivityLogger logger;

    public UserService(
        IUserData userContext, 
        IUserSettingsData settingsContext, 
        //IPaidData paidContext,
        //INotificationService notificationService,
        IActivityLogger logger)
    {
        this.userContext = userContext;
        this.settingsContext = settingsContext;
        //this.paidContext = paidContext;
        //this.notificationService = notificationService;
        this.logger = logger;
    }

    public async Task CreateUserFromRequest(CreateUserRequest request)
    {
        PasswordHasher<User> hasher = new();

        if (request.Password.Trim().Length < Data.MIN_PASSWORD_LENGTH)
           throw new ArgumentException($"Password must be at least {Data.MIN_PASSWORD_LENGTH} characters long.");

        User user = new User(
            request.Username.Trim(),
            request.FirstName.Trim(),
            request.LastName.Trim(),
            request.Email.Trim(),
            hasher.HashPassword(null!, request.Password.Trim()),
            DateTime.UtcNow,
            Guid.NewGuid().ToString()
        );

        await userContext.CreateUser(user);
        await settingsContext.CreateUserSettings(user.Settings, user.Username.Trim());

        await logger.Log(
            new ActivityLog(
                request.Username.Trim(),
                ActionType.Register,
                EntityType.User,
                null,
                "Created account",
                null
            )
        );

        // Send welcome notification
        //await notificationService.SendNotification(
        //    new Notification(
        //        request.Username.Trim(),
        //        "System",
        //        "To interact with people you know, visit the search page to send them a friend request.",
        //        NotificationType.Welcome
        //    )
        //);
    }

    public async Task<IResult> Login(string username, string password)
    {
        var user = await userContext.GetUserFromUsername(username);

        PasswordHasher<User> hasher = new();

        if (user is null)
            return Results.NotFound("Username does not exist.");
        else if (hasher.VerifyHashedPassword(null!, user.HashedPassword!, password)
            == PasswordVerificationResult.Failed)
        {
            return Results.Unauthorized();
        }

        user.Settings = await settingsContext.GetUserSettingsFromUsername(username);

        await logger.Log(
            new ActivityLog(
                user.Username,
                ActionType.Login,
                EntityType.User,
                null,
                "User logged into account",
                null
            )
        );

        return Results.Ok(user);
    }

    public async Task<IResult> Login(string token)
    {
        var user = await userContext.GetUserFromToken(token);

        if (user is null)
            return Results.NotFound("Invalid auth token.");

        user.Settings = await settingsContext.GetUserSettingsFromUsername(user.Username);

        await logger.Log(
            new ActivityLog(
                user.Username,
                ActionType.Login,
                EntityType.User,
                null,
                "User logged into account",
                null
            )
        );

        return Results.Ok(user);
    }

    public async Task<IResult> UpdateUsername(UpdateUsernameRequest request)
    {
        var user = await userContext.GetUserFromUsername(request.OldUsername);
        if (user is null)
            return Results.NotFound();

        // Check if new username is available
        var existingUser = await userContext.GetUserFromUsername(request.NewUsername);
        if (existingUser is not null)
            return Results.BadRequest("Username already taken");

        user.Username = request.NewUsername;
        await userContext.UpdateUsername(request.OldUsername, request.NewUsername);

        await logger.Log(
            new ActivityLog(
                user.Username,
                ActionType.Update,
                EntityType.User,
                null,
                "User updated username",
                new
                {
                    Old = request.OldUsername,
                    New = request.NewUsername
                }
            )
        );

        return Results.Ok();
    }

    public async Task<IResult> UpdateEmail(UpdateEmailRequest request)
    {
        var user = await userContext.GetUserFromUsername(request.Username);
        if (user is null)
            return Results.NotFound();

        await userContext.UpdateEmail(request.Username, request.NewEmail);

        await logger.Log(
            new ActivityLog(
                request.Username,
                ActionType.Update,
                EntityType.User,
                null,
                "User updated email",
                new
                {
                    Old = user.Email,
                    New = request.NewEmail
                }
            )
        );

        return Results.Ok();
    }

    public async Task<IResult> UpdateName(UpdateNameRequest request)
    {
        var user = await userContext.GetUserFromUsername(request.Username);
        if (user is null)
            return Results.NotFound();

        await userContext.UpdateName(request.Username, request.FirstName, request.LastName);

        await logger.Log(
            new ActivityLog(
                request.Username,
                ActionType.Update,
                EntityType.User,
                null,
                "User updated name",
                new
                {
                    OldFirst = user.FirstName,
                    NewFirst = request.FirstName,
                    OldLast = user.LastName,
                    NewLast = request.LastName
                }
            )
        );

        return Results.Ok();
    }

    public async Task<IResult> UpdateDescription(UpdateDescriptionRequest request)
    {
        var user = await userContext.GetUserFromUsername(request.Username);
        if (user is null)
            return Results.NotFound();

        await userContext.UpdateDescription(request.Username, request.Description);

        await logger.Log(
            new ActivityLog(
                request.Username,
                ActionType.Update,
                EntityType.User,
                null,
                "User updated profile description",
                new
                {
                    Old = user.ProfileDescription,
                    New = request.Description
                }
            )
        );

        return Results.Ok();
    }

    //public async Task<IResult> GetLeaderboard()
    //{

    //}
}
