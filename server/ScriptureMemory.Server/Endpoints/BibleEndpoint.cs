using Microsoft.AspNetCore.Mvc;
using ScriptureMemory.Server.CustomExceptions;
using ScriptureMemory.Server.Services;

namespace ScriptureMemory.Server.Endpoints;

public static class BibleEndpoint
{
    public static void ConfigureBibleEndpoints(this WebApplication app)
    {
        app.MapGet("/bible/example", async (
            BibleSyncerService syncer) =>
        {
            return Results.Ok(await syncer.GetChapterContentExample());
        });
        
        // app.MapGet("/bibles", async (
        //     BibleSyncer syncer) =>
        // {
        //     return Results.Ok(await syncer.)
        // })

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