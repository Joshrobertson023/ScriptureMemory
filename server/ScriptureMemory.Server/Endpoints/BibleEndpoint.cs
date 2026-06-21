using ScriptureMemory.Server.Services;

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

        app.MapGet("/bible/chapter/{bible}/{book}/{chapter}", async (
            string bible,
            string book,
            int chapter,
            BibleApi bibleApi) =>
        {
            return Results.Ok(await bibleApi.GetFullChapter(
                Tools.Data.GetBible(bible),
                new Reference(Books.GetBook(book)
                    ?? throw new InvalidOperationException($"{book} is not a valid book"), chapter)));
        });

        app.MapGet("/bible/verse/{bible}/{book}/{chapter}/{verse}", async (
            string bible,
            string book,
            int chapter,
            int verse,
            BibleApi bibleApi) =>
        {
            return Results.Ok(await bibleApi.GetVerseUsx(
                Tools.Data.GetBible(bible),
                new Reference(Books.GetBook(book), chapter, verse)));
        });
    }
}