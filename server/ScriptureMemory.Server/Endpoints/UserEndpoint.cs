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
using static ScriptureMemoryLibrary.Enums;

namespace VerseAppNew.Server.Apis;

public static class UserEndpoint
{
    public static void ConfigureUserEndpoints(this WebApplication app)
    {
        // Get user by username directly from data context
        app.MapGet("/users/{username}", async (
            string username,
            [FromServices] IUserData data,
            ClaimsPrincipal user) =>
        {
            return Results.Forbid(); // Currently unsecure

            var requestingUser = user.Identity?.Name;

            if (requestingUser != username)
                return Results.Unauthorized();

            var results = await data.GetUserFromUsername(username);
            if (results == null)
                return Results.NotFound();
            return Results.Ok(results);
        });

        // Search for a user
        app.MapGet("/users/search/{query}", async (
            string query,
            [FromServices] IUserData data) =>
        {
            var results = await data.SearchUsers(query);
            return Results.Ok(results);
        });

        // Create a new user account
        app.MapPost("/users", async (
            [FromBody] CreateUserRequest request,
            [FromServices] IUserService userService) =>
        {
            await userService.CreateUserFromRequest(request);
            return Results.Created();
        });

        // Set a user as active
        app.MapPut("/users/setAsActive/{username}", async (
            string username,
            [FromServices] IUserData data) =>
        {
            await data.UpdateLastSeen(username);
            return Results.Ok();
        });

        // Get users with a specific email address
        app.MapGet("/users/email/{email}", async (
            string email,
            [FromServices] IUserData data) =>
        {
            var results = await data.GetUsersFromEmail(email);
            if (results == null)
                return Results.NotFound();
            return Results.Ok(results);
        });

        // Get a user's password hash
        app.MapGet("/users/password/{username}", async (
            [FromBody] string username,
            [FromServices] IUserData data) =>
        {
            var result = await data.GetPasswordHash(username);
            if (result is null)
                return Results.NotFound();
            return Results.Ok(result);
        });

        // Login a user with username and password
        app.MapPost("/users/login/username", async (
            [FromBody] LoginRequest request,
            [FromServices] IUserService userService) =>
        {
            return await userService.Login(request.Username, request.Password);
        });

        // Login a user from a login token
        app.MapPost("/users/login/token", async (
            [FromBody] string token,
            [FromServices] IUserService userService) =>
        {
            return await userService.Login(token);
        });

        // Increment the number of verses a user has memorized
        app.MapPut("/users/incrementVersesMemorized", async (
            [FromBody] string username,
            [FromServices] IUserData data) =>
        {
            await data.IncrementVersesMemorized(username);
            return Results.Ok();
        });

        app.MapPost("/users/forgot-username", async (
            [FromBody] ForgotUsernameRequest request,
            [FromServices] IEmailSenderService emailSender) =>
        {
            return await emailSender.SendForgotUsernameEmail(request);
        });

        app.MapPost("/users/forgot-password/request", async (
            [FromBody] ForgotPasswordRequest request,
            [FromServices] IEmailSenderService emailSender) =>
        {
            return await emailSender.SendPasswordResetOtp(request);
        });

        app.MapPost("/users/forgot-password/verify", async (
            [FromBody] VerifyOtpRequest request,
            [FromServices] IPasswordResetService service) =>
        {
            return await service.VerifyOtp(request);
        });

        app.MapPost("/users/forgot-password/reset", async (
            [FromBody] ResetPasswordRequest request,
            [FromServices] IPasswordResetService service) =>
        {
            return await service.Reset(request);
        });

        app.MapPut("/users/username", async (
            [FromBody] UpdateUsernameRequest request,
            [FromServices] IUserService service) =>
        {
            return await service.UpdateUsername(request);
        });

        app.MapPut("/users/email", async (
            [FromBody] UpdateEmailRequest request,
            [FromServices] IUserService service) =>
        {
            return await service.UpdateEmail(request);
        });

        app.MapPut("/users/name", async (
            [FromBody] UpdateNameRequest request,
            [FromServices] IUserService service) =>
        {
            return await service.UpdateName(request);
        });

        app.MapPut("/users/description", async (
            [FromBody] UpdateDescriptionRequest request,
            [FromServices] IUserService service) =>
        {
            return await service.UpdateDescription(request);
        });

        app.MapGet("/leaderboard", async (
            [FromBody] GetLeaderboardRequest request,
            [FromServices] IUserData data) =>
        {
            return Results.Ok(await data.GetLeaderboard(request.Page, request.PageSize));
        });

        app.MapGet("/leaderboard/rank/{username}", async (
            string username,
            [FromServices] IUserData data,
            [FromServices] IActivityLogger logger) =>
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
        });
    }
}