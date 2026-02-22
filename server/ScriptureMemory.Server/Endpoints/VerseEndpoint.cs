using DataAccess.DataInterfaces;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Cryptography.Xml;
using System.Text.Json;
using ScriptureMemoryLibrary;
using DataAccess.Requests;

namespace VerseAppNew.Server.Endpoints;

public static class VerseEndpoint
{
    public static void ConfigureVerseEndpoints(this WebApplication app)
    {
        app.MapGet("/verses", async (
            [FromBody] string reference, 
            [FromServices] IVerseData data) =>
        {
            var results = await data.GetVerse(reference);
            if (results == null)
                return Results.NotFound();
            return Results.Ok(results);
        });

        app.MapGet("/verses/chapter", async (
            [FromBody] GetChapterRequest request,
            [FromServices] IVerseData data) =>
        {
            var results = await data.GetChapterVerses(request.Book, request.Chapter);
            return Results.Ok(results);
        });

        app.MapPut("/verses/saved/{reference}", async (
            string reference, 
            [FromServices] IVerseData data) =>
        {
            await data.UpdateUsersSavedVerse(reference);
            return Results.Ok();
        });

        app.MapPut("/verses/memorized/{reference}", async (
            string reference, 
            [FromServices] IVerseData data) =>
        {
            await data.UpdateUsersMemorizedVerse(reference);
            return Results.Ok();
        });

        //app.MapGet("/verses/search/{search}", async (string search, [FromServices] IVerseData data) =>
        //{
        //    var results = await data.GetVerseSearchResults(search);
        //    return Results.Ok(results);
        //});

        app.MapGet("/verses/top/saved/{top}", async (
            int top, 
            [FromServices] IVerseData data) =>
        {
            if (top == 0) top = 30;
            var results = await data.GetTopSavedVerses(top);
            return Results.Ok(results);
        });

        app.MapGet("/verses/top/memorized/{top}", async (
            int top, 
            [FromServices] IVerseData data) =>
        {
            if (top == 0) top = 30;
            var results = await data.GetTopMemorizedVerses(top);
            return Results.Ok(results);
        });
    }
}