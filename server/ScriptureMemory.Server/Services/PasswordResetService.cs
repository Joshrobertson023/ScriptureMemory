using DataAccess.DataInterfaces;
using DataAccess.Requests;
using VerseAppLibrary;
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

    public PasswordResetService(IUserData userContext)
    {
        this.userContext = userContext;
    }

    public async Task<IResult> VerifyOtp(VerifyOtpRequest request)
    {
        var record = await userContext.GetPasswordResetToken(request.Username.Trim(), request.Otp.Trim());
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

        var record = await userContext.GetPasswordResetToken(request.Username.Trim(), request.Otp.Trim());
        if (record == null)
        {
            return Results.BadRequest("Invalid OTP.");
        }

        var sentLocal = DateTime.SpecifyKind(record.Sent, DateTimeKind.Local);
        if (DateTime.Now > sentLocal.AddMinutes(15))
        {
            return Results.BadRequest("OTP has expired.");
        }

        await userContext.UpdatePassword(request.Username.Trim(), request.NewPassword);
        await userContext.DeletePasswordResetToken(request.Username.Trim());

        return Results.Ok();
    }
}
