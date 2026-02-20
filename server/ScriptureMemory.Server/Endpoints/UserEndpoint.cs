using Azure.Core;
using DataAccess.DataInterfaces;
using DataAccess.Models;
using DataAccess.Requests;
using DataAccess.Requests.UpdateRequests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using VerseAppNew.Server.Services;
using static VerseAppLibrary.Enums;

namespace VerseAppNew.Server.Apis;

public static class UserEndpoint
{
    public static void ConfigureUserEndpoints(this WebApplication app)
    {
        app.MapGet("/users/{username}", GetUser);
        app.MapGet("/users/search/{query}", SearchUsers);
        app.MapPost("/users", CreateUser);
        app.MapPut("/users/setAsActive/{username}", UpdateLastSeen);
        app.MapGet("/users/email/{email}", GetEmail);
        app.MapGet("/users/password/{username}", GetPasswordHash);
        app.MapGet("/users/username", LoginWithUsernamePassword);
        app.MapGet("/users/token", LoginWithToken);
        app.MapPut("/users/incrementVersesMemorized/{username}", IncrementVersesMemorized);
        app.MapPost("/users/forgot-username", SendForgotUsernameEmail);
        app.MapPost("/users/forgot-password/request", SendPasswordResetOtp);
        app.MapPost("/users/forgot-password/verify", VerifyPasswordResetOtp);
        app.MapPost("/users/forgot-password/reset", ResetPasswordWithOtp);
        app.MapPut("/users/username", UpdateUsername);
        app.MapPut("/users/email", UpdateEmail);
        app.MapPut("/users/name", UpdateName);
        app.MapPut("/users/description", UpdateDescription);
        app.MapGet("/leaderboard", GetLeaderboard);
        app.MapGet("/leaderboard/rank/{username}", GetUserRank);
    }

    private static async Task<IResult> CreateUser(
        [FromBody] CreateUserRequest request,
        [FromServices] IUserService userService)
    {
        await userService.CreateUserFromRequest(request);
        return Results.Created();
    }

    private static async Task<IResult> LoginWithUsernamePassword(
        [FromBody] LoginRequest request,
        [FromServices] IUserService userService)
    {
        return await userService.Login(request.Username, request.Password);
    }

    private static async Task<IResult> LoginWithToken(
        [FromBody] string token,
        [FromServices] IUserService userService)
    {
        return await userService.Login(token);
    }

    private static async Task<IResult> GetPasswordHash(
        [FromBody] string username,
        [FromServices] IUserData data)
    {
        var result = await data.GetPasswordHash(username);
        if (result is null)
            return Results.NotFound();
        return Results.Ok(result);
    }

    private static async Task<IResult> SearchUsers(
        string query,
        [FromServices] IUserData data)
    {
        var results = await data.SearchUsers(query);
        return Results.Ok(results);
    }

    // ----------------------------------------------------------------
    //  ** Add Jwt authentication to app **
    // ----------------------------------------------------------------
    private static async Task<IResult> GetUser(
        string username,
        [FromServices] IUserData data,
        ClaimsPrincipal user)
    {
        return Results.Forbid(); // Currently unsecure

        var requestingUser = user.Identity?.Name;

        if (requestingUser != username)
            return Results.Unauthorized();

        var results = await data.GetUserFromUsername(username);
        if (results == null)
            return Results.NotFound();
        return Results.Ok(results);
    }

    private static async Task<IResult> UpdateLastSeen(
        string username,
        [FromServices] IUserData data)
    {
        await data.UpdateLastSeen(username);
        return Results.Ok();
    }

    private static async Task<IResult> GetEmail(
        string email,
        [FromServices] IUserData data)
    {
        var results = await data.GetUsersFromEmail(email);
        if (results == null)
            return Results.NotFound();
        return Results.Ok(results);
    }

    private static async Task<IResult> IncrementVersesMemorized(
        string username,
        [FromServices] IUserData data)
    {
        await data.IncrementVersesMemorized(username);
        return Results.Ok();
    }

    private static async Task<IResult> SendForgotUsernameEmail(
        [FromBody] ForgotUsernameRequest request,
        [FromServices] IEmailSenderService emailSender)
    {
        return await emailSender.SendForgotUsernameEmail(request);
    }

    private static async Task<IResult> SendPasswordResetOtp(
        [FromBody] ForgotPasswordRequest request,
        [FromServices] IEmailSenderService emailSender)
    {
        return await emailSender.SendPasswordResetOtp(request);
    }

    private static async Task<IResult> VerifyPasswordResetOtp(
        [FromBody] VerifyOtpRequest request,
        [FromServices] IPasswordResetService service)
    {
        return await service.VerifyOtp(request);
    }

    private static async Task<IResult> ResetPasswordWithOtp(
        [FromBody] ResetPasswordRequest request,
        [FromServices] IPasswordResetService service)
    {
        return await service.Reset(request);
    }

    private static async Task<IResult> UpdateUsername(
        [FromBody] UpdateUsernameRequest request,
        [FromServices] IUserService service)
    {
        return await service.UpdateUsername(request);
    }

    private static async Task<IResult> UpdateEmail(
        [FromBody] UpdateEmailRequest request,
        [FromServices] IUserService service)
    {
        return await service.UpdateEmail(request);
    }

    private static async Task<IResult> UpdateName(
        [FromBody] UpdateNameRequest request,
        [FromServices] IUserService service)
    {
        return await service.UpdateName(request);
    }

    private static async Task<IResult> UpdateDescription(
        [FromBody] UpdateDescriptionRequest request,
        [FromServices] IUserService service)
    {
        return await service.UpdateDescription(request);
    }

    private static async Task<IResult> GetLeaderboard(
        [FromBody] GetLeaderboardRequest request,
        [FromServices] IUserData data)
    {
        return Results.Ok(await data.GetLeaderboard(request.Page, request.PageSize));
    }

    private static async Task<IResult> GetUserRank(
        string username,
        [FromServices] IUserData data,
        [FromServices] IActivityLogger logger)
    {
        await logger.Log(new ActivityLog(
            username,
            ActionType.View,
            EntityType.Page,
            null,
            "User viewed leaderboard",
            null));
        var rank = await data.GetUserRank(username);
        return Results.Ok(new { rank });
    }
}
