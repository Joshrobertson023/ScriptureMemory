//using DataAccess.DataInterfaces;
//using DataAccess.Models;
//using Microsoft.AspNetCore.Mvc;
//using System.Diagnostics;
//using System.Security.Cryptography.Xml;
//using System.Text.Json;
//using VerseAppLibrary;

//namespace VerseAppNew.Server.Endpoints;

//public static class VerseEndpoint
//{
//    public static void ConfigureVerseEndpoints(this WebApplication app)
//    {
//        app.MapGet("/verses/{reference}", GetVerse);
//        app.MapGet("/verses/chapter/{book}/{chapter}", GetChapterVerses);
//        app.MapPost("/verses", InsertVerse);
//        app.MapPut("/verses/saved/{reference}", UpdateUsersSavedVerse);
//        app.MapPut("/verses/memorized/{reference}", UpdateUsersMemorizedVerse);
//        app.MapGet("/verses/search/{search}", GetVersesSearchResult);
//        app.MapGet("/verses/top/saved/{top}", GetTopSavedVerses);
//        app.MapGet("/verses/top/memorized/{top}", GetTopMemorizedVerses);
//    }

//    public static async Task<IResult> GetChapterVerses(
//        string book,
//        int chapter,
//        [FromServices] IVerseData data)
//    {
//        try
//        {
//            var results = await data.GetChapterVerses(book, chapter);
//            return Results.Ok(results);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> GetVersesSearchResult(
//        string search,
//        [FromServices] IVerseData data)
//    {
//        try
//        {
//            var results = await data.GetVerseSearchResults(search);
//            return Results.Ok(results);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> GetVerse(
//        string reference, 
//        [FromServices] IVerseData data)
//    {
//        try
//        {
//            var results = await data.GetVerse(reference);
//            if (results == null)
//                return Results.NotFound();
//            return Results.Ok(results);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> InsertVerse(
//        [FromBody] Verse verse, 
//        [FromServices] IVerseData data)
//    {
//        try
//        {
//            await data.InsertVerse(verse);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> UpdateUsersSavedVerse(
//        string reference, 
//        [FromServices] IVerseData data)
//    {
//        try
//        {
//            await data.UpdateUsersSavedVerse(reference);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> UpdateUsersMemorizedVerse(
//        string reference, 
//        [FromServices] IVerseData data)
//    {
//        try
//        {
//            await data.UpdateUsersMemorizedVerse(reference);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> GetTopSavedVerses(
//        int top,
//        [FromServices] IVerseData data)
//    {
//        try
//        {
//            if (top == 0) top = 30;
//            var results = await data.GetTopSavedVerses(top);
//            return Results.Ok(results);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> GetTopMemorizedVerses(
//        int top,
//        [FromServices] IVerseData data)
//    {
//        try
//        {
//            if (top == 0) top = 30;
//            var results = await data.GetTopMemorizedVerses(top);
//            return Results.Ok(results);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }
//}
