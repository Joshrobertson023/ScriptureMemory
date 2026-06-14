// using Azure.Core;
// using DataAccess.Data;
// using DataAccess.Models;
// using DataAccess.Requests;
// using DataAccess.Requests.UpdateRequests;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using ScriptureMemory.Server.DataAccess.Models;
// using System;
// using System.Linq;
// using System.Security.Claims;
// using System.Security.Cryptography;
// using VerseAppNew.Server.Services;
// using static ScriptureMemory.Server.Tools.Enums;
//
// namespace VerseAppNew.Server.Apis;
//
// public static class UserEndpoint
// {
//     public static void ConfigureUserEndpoints(this WebApplication app)
//     {
//         // Get user by username directly from data context
//         app.MapGet("/users/{username}", async (
//             string username,
//             [FromServices] UserDataDapper dataDapper,
//             ClaimsPrincipal user) =>
//         {
//             return Results.Forbid(); // Currently unsecure
//
//             var requestingUser = user.Identity?.Name;
//
//             if (requestingUser != username)
//                 return Results.Unauthorized();
//
//             var results = await dataDapper.GetUserByUsername(username);
//             if (results == null)
//                 return Results.NotFound();
//             return Results.Ok(results);
//         });
//
//         // Search for a user
//         app.MapGet("/users/search/{query}", async (
//             string query,
//             [FromServices] UserDataDapper dataDapper) =>
//         {
//             var results = await dataDapper.SearchUsers(query);
//             return Results.Ok(results);
//         });
//
//         // Create a new user account
//         app.MapPost("/users", async (
//             [FromBody] Session session,
//             [FromServices] UserService userService) =>
//         {
//             return await userService.CreateUser(session);
//         });
//
//         // Check if username exists
//         app.MapGet("/users/exists/{username}", async (
//             string username,
//             [FromServices] UserDataDapper dataDapper) =>
//         {
//             return Results.Ok(await dataDapper.CheckUsernameExists(username));
//         });
//
//         // Set a user as active
//         app.MapPut("/users/setAsActive", async (
//             [FromBody] int userId,
//             [FromServices] UserDataDapper dataDapper) =>
//         {
//             await dataDapper.UpdateLastSeen(userId);
//             return Results.Ok();
//         });
//
//         // Get users with a specific email address
//         //app.MapGet("/users/email", async (
//         //    [FromBody] string email,
//         //    [FromServices] UserData data) =>
//         //{
//         //    var results = await data.GetUsersFromEmail(email);
//         //    if (results == null)
//         //        return Results.NotFound();
//         //    return Results.Ok(results);
//         //});
//
//         // Get a user's password hash
//         app.MapGet("/users/password/{username}", async (
//             [FromBody] string username,
//             [FromServices] UserDataDapper dataDapper) =>
//         {
//             var result = await dataDapper.GetPasswordHash(username);
//             if (result is null)
//                 return Results.NotFound();
//             return Results.Ok(result);
//         });
//
//         // Login a user with username and password
//         //app.MapPost("/users/login/username", async (
//         //    [FromBody] LoginRequest request,
//         //    [FromServices] UserService userService) =>
//         //{
//         //    return await userService.Login(request);
//         //});
//
//         // Login a user from a login token
//         app.MapPost("/users/login/token", async (
//             [FromBody] Session session,
//             [FromServices] UserService userService) =>
//         {
//             return await userService.TokenLogin(session);
//         });
//
//         app.MapPost("/users/jwt", async (
//             [FromBody] Session session,
//             [FromServices] UserService userService) =>
//         {
//             return await userService.GetNewJwt(session);
//         });
//
//         // Increment the number of verses a user has memorized
//         app.MapPut("/users/incrementVersesMemorized", async (
//             [FromBody] int userId,
//             [FromServices] UserDataDapper dataDapper) =>
//         {
//             await dataDapper.IncrementVersesMemorized(userId);
//             return Results.Ok();
//         });
//
//         app.MapPost("/users/forgot-username", async (
//             [FromBody] ForgotUsernameRequest request,
//             [FromServices] EmailSenderService emailSender) =>
//         {
//             return await emailSender.SendForgotUsernameEmail(request);
//         });
//
//         app.MapPost("/users/forgot-password/request", async (
//             [FromBody] ForgotPasswordRequest request,
//             [FromServices] EmailSenderService emailSender) =>
//         {
//             return await emailSender.SendPasswordResetOtp(request);
//         });
//
//         app.MapPost("/users/forgot-password/verify", async (
//             [FromBody] VerifyOtpRequest request,
//             [FromServices] PasswordResetService service) =>
//         {
//             return await service.VerifyOtp(request);
//         });
//
//         app.MapPost("/users/forgot-password/reset", async (
//             [FromBody] ResetPasswordRequest request,
//             [FromServices] PasswordResetService service) =>
//         {
//             return await service.Reset(request);
//         });
//
//         //app.MapPost("/staff/update-password", async (
//         //    [FromBody] string newPassword,
//         //    [FromServices] AdminData data) =>
//         //{
//         //    return await data.UpdatePassword(newPassword);
//         //});
//
//         //app.MapPut("/users/username", async (
//         //    [FromBody] UpdateUsernameRequest request,
//         //    [FromServices] UserService service) =>
//         //{
//         //    return await service.UpdateUsername(request);
//         //});
//
//         //app.MapPut("/users/email", async (
//         //    [FromBody] UpdateEmailRequest request,
//         //    [FromServices] UserService service) =>
//         //{
//         //    return await service.UpdateEmail(request);
//         //});
//
//         //app.MapPut("/users/name", async (
//         //    [FromBody] UpdateNameRequest request,
//         //    [FromServices] UserService service) =>
//         //{
//         //    return await service.UpdateName(request);
//         //});
//
//         //app.MapPut("/users/description", async (
//         //    [FromBody] UpdateDescriptionRequest request,
//         //    [FromServices] UserService service) =>
//         //{
//         //    return await service.UpdateDescription(request);
//         //});
//
//         app.MapGet("/leaderboard", async (
//             [FromBody] GetLeaderboardRequest request,
//             [FromServices] UserDataDapper dataDapper) =>
//         {
//             return Results.Ok(await dataDapper.GetLeaderboard(request.Page, request.PageSize));
//         });
//
//         app.MapGet("/leaderboard/rank", async (
//             [FromBody] int userId,
//             [FromServices] UserDataDapper dataDapper,
//             [FromServices] ActivityLogger logger) =>
//         {
//             await logger.Log(new ActivityLog(
//                 userId,
//                 ActionType.View,
//                 EntityType.Page,
//                 null,
//                 "User viewed leaderboard",
//                 null));
//             //var rank = await data.GetUserRank(userId);
//             //return Results.Ok(new { rank });
//         });
//     }
// }