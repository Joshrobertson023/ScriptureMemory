//using DataAccess.DataInterfaces;
//using DataAccess.Models;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Threading.Tasks;

//namespace VerseAppNew.Server.Endpoints;

//public static class BanEndpoint
//{
//    public static void ConfigureBanEndpoints(this WebApplication app)
//    {
//        app.MapGet("/bans/user/{username}", GetActiveBan);
//        app.MapGet("/bans/user/{username}/all", GetAllBans);
//        app.MapPost("/bans", CreateBan);
//        app.MapDelete("/bans/{banId:int}", DeleteBan);
//        app.MapGet("/bans/check/{username}", CheckIfBanned);
//    }

//    private static async Task<IResult> GetActiveBan(
//        string username,
//        [FromServices] IBanData banData)
//    {
//        try
//        {
//            var ban = await banData.GetActiveBan(username);
//            if (ban == null)
//                return Results.NotFound();
//            return Results.Ok(ban);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetAllBans(
//        string username,
//        [FromServices] IBanData banData)
//    {
//        try
//        {
//            var bans = await banData.GetAllBans(username);
//            return Results.Ok(bans);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private sealed record CreateBanRequest(string Username, string AdminBanned, string? Reason, DateTime? BanExpireDate);

//    private static async Task<IResult> CreateBan(
//        [FromBody] CreateBanRequest request,
//        [FromServices] IBanData banData)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.AdminBanned))
//            {
//                return Results.BadRequest("Username and AdminBanned are required");
//            }

//            var ban = await banData.CreateBan(request.Username, request.AdminBanned, request.Reason, request.BanExpireDate);
//            return Results.Ok(ban);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> DeleteBan(
//        int banId,
//        [FromServices] IBanData banData)
//    {
//        try
//        {
//            await banData.DeleteBan(banId);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> CheckIfBanned(
//        string username,
//        [FromServices] IBanData banData)
//    {
//        try
//        {
//            var isBanned = await banData.IsUserBanned(username);
//            return Results.Ok(new { IsBanned = isBanned });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }
//}































