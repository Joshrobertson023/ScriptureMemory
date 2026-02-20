//using System;
//using DataAccess.DataInterfaces;
//using Microsoft.AspNetCore.Mvc;

//namespace VerseAppNew.Server.Endpoints;

//public static class PushTokenEndpoint
//{
//    public static void ConfigurePushTokenEndpoints(this WebApplication app)
//    {
//        app.MapPost("/push-tokens", RegisterPushToken);
//        app.MapDelete("/push-tokens/{expoPushToken}", RemovePushToken);
//    }

//    private static async Task<IResult> RegisterPushToken(
//        [FromBody] RegisterPushTokenRequest request,
//        [FromServices] IPushTokenData pushTokenData)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.ExpoPushToken))
//            {
//                return Results.BadRequest(new { message = "Username and Expo push token are required." });
//            }

//            var platform = string.IsNullOrWhiteSpace(request.Platform) ? "unknown" : request.Platform;

//            await pushTokenData.UpsertTokenAsync(request.Username.Trim(), request.ExpoPushToken.Trim(), platform.Trim());
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> RemovePushToken(
//        string expoPushToken,
//        [FromQuery] string username,
//        [FromServices] IPushTokenData pushTokenData)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(expoPushToken))
//            {
//                return Results.BadRequest(new { message = "Username and Expo push token are required." });
//            }

//            await pushTokenData.RemoveTokenAsync(username.Trim(), expoPushToken.Trim());
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public sealed class RegisterPushTokenRequest
//    {
//        public string Username { get; set; } = string.Empty;
//        public string ExpoPushToken { get; set; } = string.Empty;
//        public string? Platform { get; set; }
//    }
//}



