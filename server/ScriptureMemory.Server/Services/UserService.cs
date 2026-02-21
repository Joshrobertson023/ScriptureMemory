using Azure.Core;
using DataAccess.DataInterfaces;
using DataAccess.Models;
using DataAccess.Requests;
using DataAccess.Requests.UpdateRequests;
using Microsoft.AspNetCore.Identity;
using ScriptureMemoryLibrary;
using System.Text.Json;
using static ScriptureMemoryLibrary.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    Task<bool> IsUsernameAvailable(string username);
}

public sealed class UserService : IUserService
{
    private readonly IUserData userContext;
    private readonly IUserSettingsData settingsContext;
    //private readonly IPaidData paidContext;
    private readonly INotificationService notificationService;
    private readonly IActivityLogger logger;
    private readonly IEmailSenderService emailSender;

    public UserService(
        IUserData userContext, 
        IUserSettingsData settingsContext, 
        //IPaidData paidContext,
        INotificationService notificationService,
        IActivityLogger logger,
        IEmailSenderService emailSender)
    {
        this.userContext = userContext;
        this.settingsContext = settingsContext;
        //this.paidContext = paidContext;
        this.notificationService = notificationService;
        this.logger = logger;
        this.emailSender = emailSender;
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
        user.Id = await userContext.GetUserIdFromUsername(user.Username);

        await settingsContext.CreateUserSettings(user.Settings, user.Id);

        await logger.Log(
            new ActivityLog(
                user.Id,
                ActionType.Register,
                EntityType.User,
                null,
                "Created account",
                null
            )
        );

        //Send welcome notification
        await notificationService.SendNotification(
            new Notification(
                user.Id,
                Data.NOTIFICATION_SYSTEM_ID,
                Data.welcomeNotificationBody,
                NotificationType.Welcome
           )
       );
    }

    public async Task<IResult> Login(string username, string password)
    {
        var user = await userContext.GetUserFromUsername(username);

        PasswordHasher<User> hasher = new();

        if (user is null)
            return Results.Problem("Username does not exist.");
        else if (hasher.VerifyHashedPassword(null!, user.HashedPassword!, password)
            == PasswordVerificationResult.Failed)
        {
            return Results.Unauthorized();
        }

        user.Settings = await settingsContext.GetUserSettingsFromUserId(user.Id);

        await logger.Log(
            new ActivityLog(
                user.Id,
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

        user.Settings = await settingsContext.GetUserSettingsFromUserId(user.Id);

        await logger.Log(
            new ActivityLog(
                user.Id,
                ActionType.Login,
                EntityType.User,
                null,
                "User logged into account",
                null
            )
        );

        return Results.Ok(user);
    }

    public async Task<bool> IsUsernameAvailable(string username)
    {
        var existingUser = await userContext.GetUserFromUsername(username);

        return (existingUser is null);
    }

    public async Task<IResult> UpdateUsername(UpdateUsernameRequest request)
    {
        var user = await userContext.GetUserFromUsername(request.OldUsername);

        if (user is null)
            return Results.Problem("Old username does not exist");

        if (!await IsUsernameAvailable(request.NewUsername))
            return Results.Problem("Username is not available");

        user.Username = request.NewUsername;
        await userContext.UpdateUsername(request.UserId, request.NewUsername);

        await logger.Log(
            new ActivityLog(
                user.Id,
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
        var user = await userContext.GetUserFromUserId(request.UserId);
        if (user is null)
            return Results.NotFound();

        await userContext.UpdateEmail(request.UserId, request.NewEmail);
        await logger.Log(
            new ActivityLog(
                request.UserId,
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
        var user = await userContext.GetUserFromUserId(request.UserId);
        if (user is null)
            return Results.NotFound();

        await userContext.UpdateName(request.UserId, request.FirstName, request.LastName);
        await logger.Log(
            new ActivityLog(
                request.UserId,
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
        var user = await userContext.GetUserFromUserId(request.UserId);
        if (user is null)
            return Results.NotFound();

        await userContext.UpdateDescription(request.UserId, request.Description);
        await logger.Log(
            new ActivityLog(
                request.UserId,
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
