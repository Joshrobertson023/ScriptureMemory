using DataAccess.DataInterfaces;
using DataAccess.Models;
using DataAccess.Requests;
using ScriptureMemoryLibrary;
using static ScriptureMemoryLibrary.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VerseAppNew.Server.Services;

public interface IPasswordResetService
{
    Task<IResult> VerifyOtp(VerifyOtpRequest request);
    Task<IResult> Reset(ResetPasswordRequest request);
}

public sealed class PasswordResetService : IPasswordResetService
{
    private readonly IUserData userContext;
    private readonly IEmailSenderService emailSender;
    private readonly IActivityLogger activityLogger;

    public PasswordResetService(
        IUserData userContext,
        IEmailSenderService emailSender,
        IActivityLogger activityLogger)
    {
        this.userContext = userContext;
        this.emailSender = emailSender;
        this.activityLogger = activityLogger;
    }

    public async Task<IResult> VerifyOtp(VerifyOtpRequest request)
    {
        var record = await userContext.GetPasswordResetToken(request.UserId, request.Otp.Trim());
        if (record is null)
        {
            return Results.Ok(new { valid = false, reason = "invalid" });
        }

        var sentLocal = DateTime.SpecifyKind(record.Sent, DateTimeKind.Local);
        if (DateTime.Now > sentLocal.AddMinutes(15))
        {
            return Results.Ok(new { valid = false, reason = "expired" });
        }

        return Results.Ok(new { valid = true });
    }

    public async Task<IResult> Reset(ResetPasswordRequest request)
    {
        if (request.NewPassword.Length < Data.MIN_PASSWORD_LENGTH)
            return Results.BadRequest($"Password must be at least {Data.MIN_PASSWORD_LENGTH} characters long.");

        var record = await userContext.GetPasswordResetToken(request.UserId, request.Otp.Trim());
        if (record == null)
        {
            return Results.BadRequest("Invalid OTP.");
        }

        var sentLocal = DateTime.SpecifyKind(record.Sent, DateTimeKind.Local);
        if (DateTime.Now > sentLocal.AddMinutes(15))
        {
            return Results.BadRequest("OTP has expired.");
        }

        await userContext.UpdatePassword(request.UserId, request.NewPassword);
        await userContext.DeletePasswordResetToken(request.UserId);

        return Results.Ok();
    }



    // -- Forgot ------------------------------------

    public async Task<IResult> ForgotUsername(ForgotUsernameRequest request)
    {
        var emailSenderResults = await emailSender.SendForgotUsernameEmail(request);

        var fullName = request.FirstName + " " + request.LastName;
        await activityLogger.Log(
            new ActivityLog(
                null,
                ActionType.Forgot,
                EntityType.User,
                null,
                "User forgot username",
                new
                {
                    Context = "Username not logged",
                    FullName = fullName
                }
            )
        );

        return Results.Ok();
    }
}
