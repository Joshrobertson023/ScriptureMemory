using Microsoft.AspNet.Identity;
using ScriptureMemory.Server.Data.DataAccess;
using ScriptureMemory.Server.Data.Responses;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VerseAppNew.Server.Services;

public sealed class UserService
{
    private readonly IUserData _userContext;
    private readonly ISessionData _sessionContext;
    private readonly UserSettingsData _settingsContext;
    private readonly TokenProvider _tokenProvider;
    //private readonly PaidData _paidContext;
    private readonly NotificationService _notificationService;
    private readonly ActivityLogger _logger;
    //private readonly EmailSenderService _emailSender;
    

    public UserService(
        IUserData userContext, 
        UserSettingsData settingsContext, 
        ISessionData sessionContext,
        //PaidData paidContext,
        TokenProvider tokenProvider,
        NotificationService notificationService,
        ActivityLogger logger
        //EmailSenderService emailSender
        )
    {
        _userContext = userContext;
        _tokenProvider = tokenProvider;
        _sessionContext = sessionContext;
        _settingsContext = settingsContext;
        //this.paidContext = paidContext;
        _notificationService = notificationService;
        _logger = logger;
        //_emailSender = emailSender;
    }

    /// <summary>
    /// Method that creates a user from the user's device session info.
    /// This method runs when the user first opens the app.
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    public async Task<CreateUserResponse> CreateUser(Session session)
    {
        var hasher = new PasswordHasher<User>();

        var newUser = await _userContext.CreateUser();
        
        newUser.Role = UserRole.User;

        session.RefreshTokenHash = hasher.HashPassword(newUser, Guid.NewGuid().ToString());

        newUser.Sessions = new List<Session>() { session };

        await _sessionContext.CreateSession(
            newUser.UserId, 
            newUser.Sessions.First());

        return new CreateUserResponse { User = newUser, Jwt = _tokenProvider.Create(newUser) };
    }
    //
    // public async Task<IResult> GetNewJwt(Session session)
    // {
    //     session.LastSeenAt = DateTime.UtcNow;
    //     await _sessionContext.LoginSession(session);
    //
    //     var user = await _userContext.GetUserByDeviceId(session.DeviceId);
    //     user.Sessions = new List<Session>
    //     {
    //         session
    //     };
    //
    //     return Results.Ok(new
    //     {
    //         Jwt = _tokenProvider.Create(user)
    //     });
    // }
    //
    // public async Task<IResult> TokenLogin(Session session)
    // {
    //     if (session.RefreshTokenHash is null)
    //         return Results.Unauthorized();
    //
    //     var hasher = new PasswordHasher<string>();
    //     var user = await _userContext.GetUserByRefreshToken(session.RefreshTokenHash);
    //
    //     if (user is null)
    //         return Results.Unauthorized();
    //
    //     user.Role = UserRole.User;
    //
    //     return Results.Ok(new
    //     {
    //         User = user,
    //         RefreshTokenHash = hasher.HashPassword(null!, Guid.NewGuid().ToString()),
    //         Jwt = _tokenProvider.Create(user)
    //     });
    // }

    // public async Task<IResult> Login(LoginRequest request)
    // {
    //     var hasher = new PasswordHasher<string>();
    //
    //     var user = await _userContext.GetUserByUsername(request.Username.Trim().ToLower()); 
    //         // Usernames are not case sensitive
    //
    //     if (user is null)
    //         return Results.Unauthorized();
    //
    //     if (hasher.VerifyHashedPassword(
    //         null!, 
    //         user?.HashedPassword ?? throw new ArgumentNullException(nameof(user.HashedPassword)), 
    //         request.Password.Trim())
    //         == PasswordVerificationResult.Failed)
    //     {
    //         return Results.Unauthorized();
    //     }
    //
    //     request.Session.RefreshTokenHash = hasher.HashPassword(null!, Guid.NewGuid().ToString());
    //
    //     if (request.Session.DeviceId.Trim() != await _sessionContext.GetDeviceId(request.Session.RefreshTokenHash))
    //     {
    //         await _sessionContext.CreateSession(user.Id, request.Session);
    //     }
    //     else
    //     {
    //         await _sessionContext.UpdateRefreshToken(user.Id, request.Session);
    //     }
    //
    //     return Results.Ok(new
    //     {
    //         User = user,
    //         RefreshTokenHash = request.Session.RefreshTokenHash,
    //         Jwt = _tokenProvider.Create(user)
    //     });
    // }
}
