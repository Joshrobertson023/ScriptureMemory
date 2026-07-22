using Microsoft.AspNetCore.Mvc;
using ScriptureMemory.Server.CustomExceptions;
using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.Services;
using ScriptureMemory.Server.SignalR;
using StackExchange.Redis;
using System.Security.Claims;

namespace ScriptureMemory.Server.Endpoints;

public static class BibleEndpoint
{
    public static void ConfigureBibleEndpoints(this WebApplication app)
    {
        app.MapGet("/bible/{bibleId}", async (
            string bibleId,
            [FromServices] BibleData bibleContext) =>
        {
            return Results.Ok(await bibleContext.GetBibleById(bibleId));
        }).RequireAuthorization("Admin");

        app.MapPost("/bible/authorization-sync", async (
            [FromBody] string initiator,
            [FromServices] BibleSyncer bibleSyncer,
            [FromServices] BibleData bibleData) =>
        {
            await bibleSyncer.SyncBibleAuthorization(initiator);
            return Results.Ok(await bibleData.GetBibles());
        }).RequireAuthorization("Admin");

        app.MapGet("/bible/syncer/data", async (
            [FromServices] BibleSyncer syncer) =>
        {
            return Results.Ok(await syncer.GetBibleSyncData());
        }).RequireAuthorization("Admin");

        app.MapGet("/bible/syncer/logs", async (
            [FromServices] BibleSyncLogData logData) =>
        {
            return Results.Ok(await logData.GetSyncLogs());
        }).RequireAuthorization("Admin");

        app.MapHub<SyncHub>("/bible/syncer/stream");

        app.MapPost("/bible/syncer/{bibleId}/set-visible", async (
            string bibleId,
            [FromBody] string username,
            [FromServices] BibleSyncer syncer) =>
        {
            await syncer.SetVisible(bibleId, username);
            
            return Results.Ok();
        }).RequireAuthorization("SuperAdmin");

        app.MapPost("/bible/syncer/{bibleId}/set-not-visible", async (
            string bibleId,
            [FromBody] string username,
            [FromServices] BibleSyncer syncer) =>
        {
            await syncer.SetInvisible(bibleId, username);

            return Results.Ok();
        }).RequireAuthorization("SuperAdmin");

        // app.MapPost("/bible/syncer/{bibleId}/queue-sync", async (
        //     string bibleId,
        //     [FromServices] BibleSyncer syncer,
        //     [FromBody] string username) =>
        // {
        //     if (string.IsNullOrEmpty(username))
        //         return Results.Unauthorized();
        //
        //     await syncer.QueueBibleForSync(bibleId, username);
        //
        //     return Results.Ok();
        // }).RequireAuthorization("Admin");
        //
        // app.MapPost("/bible/syncer/{bibleId}/{bibleName}/cancel-sync", async (
        //     string bibleId,
        //     string bibleName,
        //     [FromServices] BibleSyncer syncer,
        //     [FromBody] string username) =>
        // {
        //     if (string.IsNullOrEmpty(username))
        //         return Results.Unauthorized();
        //     
        //     await syncer.CancelSync(bibleId, bibleName, username);
        //
        //     return Results.Ok();
        // }).RequireAuthorization("Admin");

        app.MapPost("/bible/chapter/{bible}/{book}/{chapter}", async (
            string bible,
            string book,
            int chapter,
            [FromBody] int userId,
            [FromServices] BibleService bibleService) =>
        {
            return Results.Ok(await bibleService.GetChapter(userId, bible, book, chapter));
        });

        // app.MapGet("/bible/verse/{bible}/{book}/{chapter}/{verse}", async (
        //     string bible,
        //     string book,
        //     int chapter,
        //     int verse,
        //     BibleApi bibleApi) =>
        // {
        //     return Results.Ok(await bibleApi.GetVerseUsx(
        //         Tools.Bibles.GetBible(bible),
        //         new Reference(Books.GetBook(book), chapter, verse)));
        // });
    }
}