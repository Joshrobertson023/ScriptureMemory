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
        var newUser = await _userContext.GetUserById(newUserId);

        session.RefreshTokenHash = hasher.HashPassword(null!, Guid.NewGuid().ToString());

        int sessionId = await _sessionContext.CreateSession(newUserId, session);

        return Results.Ok(new
        {
            User = newUser,
            RefreshTokenHash = session.RefreshTokenHash,
            Jwt = _tokenProvider.Create(newUser)
        });
    }

    public async Task<IResult> TokenLogin(Session session)
    {
        var hasher = new PasswordHasher<string>();
        var user = await _userContext.GetUserByRefreshToken(session.RefreshTokenHash);

        if (user is null)
            return Results.Unauthorized();

        return Results.Ok(new
        {
            User = user,
            RefreshTokenHash = hasher.HashPassword(null!, Guid.NewGuid().ToString()),
            Jwt = _tokenProvider.Create(user)
        });
    }

    public async Task<IResult> Login(LoginRequest request)
    {
        var hasher = new PasswordHasher<string>();

        var user = await _userContext.GetUserByUsername(request.Username);

        if (user is null)
            return Results.Unauthorized();

        if (hasher.VerifyHashedPassword(
            null!, 
            user?.HashedPassword ?? throw new ArgumentNullException(nameof(user.HashedPassword)), 
            request.Password)
            == PasswordVerificationResult.Failed)
        {
            return Results.Unauthorized();
        }

        request.Session.RefreshTokenHash = hasher.HashPassword(null!, Guid.NewGuid().ToString());

        if (request.Session.DeviceId.Trim() != await _sessionContext.GetDeviceId(request.Session.RefreshTokenHash))
        {
            await _sessionContext.CreateSession(user.Id, request.Session);
        }
        else
        {
            await _sessionContext.UpdateRefreshToken(user.Id, request.Session);
        }

        return Results.Ok(new
        {
            User = user,
            RefreshTokenHash = request.Session.RefreshTokenHash,
            Jwt = _tokenProvider.Create(user)
        });
    }
}
