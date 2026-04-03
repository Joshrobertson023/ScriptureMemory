using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Cryptography.Xml;
using System.Text.Json;
using ScriptureMemory.Server.Tools;
using DataAccess.Requests;
using DataAccess.Data;

namespace VerseAppNew.Server.Endpoints;

public static class VerseEndpoint
{
    public static void ConfigureVerseEndpoints(this WebApplication app)
    {
        app.MapGet("/verses", async (
            [FromBody] string reference,
            [FromServices] VerseData data) =>
        {
            DataAccess.Models.Reference _reference = ReferenceParser.Parse(reference);
            var results = await data.GetVerses(
                _reference.Book,
                _reference.Chapter,
                _reference.Verses);
            if (results == null)
                return Results.NotFound();
            return Results.Ok(results);
        }).RequireAuthorization("User");

        app.MapGet("/verses/book/exists", async (
            [FromBody] string book) =>
        {
            bool exists = Books.TryGetBook(book, out string displayName);
            return Results.Ok(displayName);
        }).RequireAuthorization("User");

        //app.MapGet("/verses/chapter", async (
        //    [FromBody] GetChapterRequest request,
        //    [FromServices] VerseData data) =>
        //{
        //    var results = await data.GetChapterVerses(request.Book, request.Chapter);
        //    return Results.Ok(results);
        //});

        //app.MapPut("/verses/saved/{reference}", async (
        //    string reference,
        //    [FromServices] VerseData data) =>
        //{
        //    await data.UpdateUsersSavedVerse(reference);
        //    return Results.Ok();
        //});

        //app.MapPut("/verses/memorized/{reference}", async (
        //    string reference,
        //    [FromServices] VerseData data) =>
        //{
        //    await data.UpdateUsersMemorizedVerse(reference);
        //    return Results.Ok();
        //});

        //app.MapGet("/verses/search/{search}", async (string search, [FromServices] VerseData data) =>
        //{
        //    var results = await data.GetVerseSearchResults(search);
        //    return Results.Ok(results);
        //});

        //app.MapGet("/verses/top/saved/{top}", async (
        //    int top,
        //    [FromServices] VerseData data) =>
        //{
        //    if (top == 0) top = 30;
        //    var results = await data.GetTopSavedVerses(top);
        //    return Results.Ok(results);
        //});

        //app.MapGet("/verses/top/memorized/{top}", async (
        //    int top,
        //    [FromServices] VerseData data) =>
        //{
        //    if (top == 0) top = 30;
        //    var results = await data.GetTopMemorizedVerses(top);
        //    return Results.Ok(results);
        //});
    }
}