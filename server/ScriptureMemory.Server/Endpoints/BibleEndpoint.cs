using Microsoft.AspNetCore.Mvc;
using ScriptureMemory.Server.CustomExceptions;
using ScriptureMemory.Server.Services;
using ScriptureMemory.Server.SignalR;
using System.Security.Claims;

namespace ScriptureMemory.Server.Endpoints;

public static class BibleEndpoint
{
    public static void ConfigureBibleEndpoints(this WebApplication app)
    {
        app.MapGet("/bible/example", async (
            BibleSyncer syncer) =>
        {
            return Results.Ok(await syncer.GetChapterContentExample());
        });

        app.MapGet("/bible/syncer/data", async (
            [FromServices] BibleSyncer syncer) =>
        {
            return Results.Ok(await syncer.GetBibleSyncData());
        }).RequireAuthorization("Admin");

        app.MapHub<SyncHub>("/bible/syncer/stream");

        app.MapPost("/bible/syncer/{bibleId}/set-visible", async (
            string bibleId,
            [FromServices] BibleSyncer syncer) =>
        {
            
        }).RequireAuthorization("SuperAdmin");

        app.MapPost("/bible/syncer/{bibleId}/set-not-visible", async (
            string bibleId,
            [FromServices] BibleSyncer syncer) =>
        {
            
        }).RequireAuthorization("SuperAdmin");

        app.MapPost("/bible/syncer/{bibleId}/queue-sync", async (
            string bibleId,
            [FromServices] BibleSyncer syncer,
            CancellationToken cancellationToken,
            ClaimsPrincipal user) =>
        {
            if (user.Identity?.Name is null)
                return Results.Unauthorized();

            await syncer.QueueBibleForSync(cancellationToken, bibleId, user.Identity.Name);

            return Results.Ok();
        }).RequireAuthorization("Admin");

        app.MapPost("/bible/syncer/{bibleId}/{bibleName}/dequeue-sync", async (
            string bibleId,
            string bibleName,
            [FromServices] BibleSyncer syncer,
            ClaimsPrincipal user) =>
        {
            if (user.Identity?.Name is null)
                return Results.Unauthorized();
            
            await syncer.CancelSync(bibleId, bibleName, user.Identity.Name);

            return Results.Ok();
        }).RequireAuthorization("Admin");

        app.MapPost("/bible/chapter/{bible}/{book}", async (
            string bible,
            string book,
            [FromBody] int chapter,
            [FromServices] BibleApi bibleApi,
            [FromServices] ILogger<Program> _logger) =>
        {
            _logger.LogInformation("Requested {Book} from {Bible} in chapter {Chapter}", book, bible, chapter);
            
            return Results.Ok(await bibleApi.GetFullChapter(
                Tools.Bibles.GetBible(bible),
                new Reference(Books.GetBook(book), chapter)));
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