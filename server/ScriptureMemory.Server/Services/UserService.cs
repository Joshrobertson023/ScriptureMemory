using Azure.Core;
using DataAccess.Data;
using DataAccess.Models;
using DataAccess.Requests;
using DataAccess.Requests.UpdateRequests;
using Microsoft.AspNetCore.Identity;
using ScriptureMemory.Server.DataAccess.Data;
using ScriptureMemory.Server.DataAccess.Models;
using ScriptureMemory.Server.Tools;
using System.Text.Json;
using static ScriptureMemory.Server.Tools.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VerseAppNew.Server.Services;

public sealed class UserService
{
    private readonly UserData _userContext;
    private readonly SessionData _sessionContext;
    private readonly UserSettingsData _settingsContext;
    private readonly TokenProvider _tokenProvider;
    //private readonly PaidData _paidContext;
    private readonly NotificationService _notificationService;
    private readonly ActivityLogger _logger;
    private readonly EmailSenderService _emailSender;

    public UserService(
        UserData userContext, 
        UserSettingsData settingsContext, 
        //PaidData paidContext,
        TokenProvider tokenProvider,
        NotificationService notificationService,
        ActivityLogger logger,
        EmailSenderService emailSender)
    {
        _userContext = userContext;
        _tokenProvider = tokenProvider;
        _settingsContext = settingsContext;
        //this.paidContext = paidContext;
        _notificationService = notificationService;
        _logger = logger;
        _emailSender = emailSender;
    }

    public async Task<IResult> CreateUser(Session session)
    {
        var hasher = new PasswordHasher<string>();

        int newUserId = await _userContext.CreateUser();

        session.RefreshTokenHash = hasher.HashPassword(null!, Guid.NewGuid().ToString());

        int sessionId = await _sessionContext.CreateSession(newUserId, session);

        return Results.Ok(new
        {
            UserId = newUserId,
            RefreshTokenHash = session.RefreshTokenHash,
            Jwt = _tokenProvider.Create(new User { Id = newUserId })
        });
    }




















    public async Task CreateUserAccount(CreateUserRequest request)
    {
        PasswordHasher<User> hasher = new();

        if (await userContext.CheckUsernameExists(request.Username))
            throw new ArgumentException("Username is already taken.");

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
        user.Id = await userContext.GetUserIdFromUsername(user.Username.Trim());

        await settingsContext.CreateUserSettings(user.Preferences, user.Id);

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
        var user = await userContext.GetUserFromUsername(username.Trim());

        PasswordHasher<User> hasher = new();

        if (user is null)
            return Results.Problem("Username does not exist.");
        else if (hasher.VerifyHashedPassword(null!, user.HashedPassword!, password.Trim())
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
        return (await userContext.GetTokenFromUsername(username)) is null;
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
