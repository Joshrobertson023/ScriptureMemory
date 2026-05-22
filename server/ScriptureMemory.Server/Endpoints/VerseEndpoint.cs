using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using ScriptureMemory.Server.Tools;
using DataAccess.Requests;
using DataAccess.Data;
using ScriptureMemory.Server.DataAccess.Data;
using ScriptureMemory.Server.DataAccess.Requests;
using Pgvector;
using ScriptureMemory.Server.DataAccess.Models;

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
        });//.RequireAuthorization("User");

        // TODO: Client: update to use new endpoint without body and using route parameter
        app.MapGet("/verses/{book}/exists", async (
            string book) =>
        {
            bool exists = Books.TryGetBook(book, out string displayName);
            return Results.Ok(displayName);
        });//.RequireAuthorization("User");
        
        // Todo: Client: Update to use route params
        // Gets a full chapter's content to display on the client
        app.MapGet("/{book}/{chapter}", async (
            string book,
            int chapter,
            [FromServices] VerseData data) =>
        {
            bool bookExists = Books.TryGetBook(book, out string displayName);
            if (!bookExists)
                return Results.NotFound();
            var results = await data.GetChapterVerses(displayName, chapter);
            return Results.Ok(results);
        });//.RequireAuthorization("User");

        // Todo: old route: "/verses/reference"
        // Try to get the Reference object from a user query
        app.MapPost("/reference/{query}", async (
            string query) =>
        {
            return Results.Ok(ReferenceParser.Parse(query));
        });//.RequireAuthorization("User");

        app.MapPost("/verses/cross-reference", async (
            [FromBody] List<int> verseIds,
            [FromServices] CrossReferenceData crossReferenceData) =>
        {
            return Results.Ok(await crossReferenceData.GetCrossReferences(verseIds));
        });//.RequireAuthorization("User");

        app.MapPost("/verses/cross-reference/reference", async (
            [FromBody] List<string> references,
            [FromServices] CrossReferenceData crossReferenceData) =>
        {
            return Results.Ok(await crossReferenceData.GetCrossReferences(
                references.Select(r => ReferenceParser.Parse(r)).ToList()));
        });//.RequireAuthorization("User");

        // Todo: old route: "/verses/cross-reference/reference/{reference}"
        // Gets all cross-references for a reference
        app.MapGet("/verses/cross-reference/{reference}", async (
            string reference,
            [FromServices] CrossReferenceData crossReferenceData) =>
        {
            Reference? parsedReference = ReferenceParser.Parse(reference);
            
            if (parsedReference is null) 
                return Results.NotFound();
            
            return Results.Ok(await crossReferenceData.GetCrossReferences( new List<Reference> { parsedReference }));
        });//.RequireAuthorization("User");

        // Todo: old route: "verses/verse-card"
        // Gets content for the passage bottom sheet card on the client
        app.MapPost("/passage-card", async (
            [FromBody] GetVerseCardRequest request,
            [FromServices] VerseData data) =>
        {
            return Results.Ok(await data.GetVerseCardResponse(request.UserId, request.VerseIds));
        });//.RequireAuthorization("User");

        // Todo: refactor to receive List<string> since that's all that's required
        // Makes it easier for client to know what to send
        // Gets semantically similar verses to a passage
        app.MapPost("/verses/similar", async(
            [FromBody] Passage passage,
            [FromServices] VerseData data,
            [FromServices] EmbeddingGenerator embeddingGenerator) =>
        {
            List<string> references = new();
            passage.Verses.ForEach(v => references.Add(v.GetEmbeddingText()));

            var similarVerses = await data.GetVersesSemanticSearch(
                    await embeddingGenerator.GenerateEmbeddings(references));

            List<Passage> results = new();
            similarVerses.ForEach(v => results.Add(new Passage
            {
                Reference = v.Reference,
                Verses = new List<Verse> { v }
            }));

            return Results.Ok(results);
        });
    }
}