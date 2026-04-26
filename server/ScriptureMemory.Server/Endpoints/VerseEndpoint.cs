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

        app.MapGet("/verses/book/exists", async (
            [FromBody] string book) =>
        {
            bool exists = Books.TryGetBook(book, out string displayName);
            return Results.Ok(displayName);
        });//.RequireAuthorization("User");

        app.MapPost("/verses/chapter", async (
            [FromBody] GetChapterRequest request,
            [FromServices] VerseData data) =>
        {
            bool bookExists = Books.TryGetBook(request.Book, out string displayName);
            if (!bookExists)
                return Results.NotFound();
            var results = await data.GetChapterVerses(displayName, request.Chapter);
            return Results.Ok(results);
        });//.RequireAuthorization("User");

        app.MapPost("/verses/reference", async (
            [FromBody] string query) =>
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

        app.MapGet("/verses/cross-reference/reference/{reference}", async (
            string reference,
            [FromServices] CrossReferenceData crossReferenceData) =>
        {
            return Results.Ok(await crossReferenceData.GetCrossReferences(
                new List<Reference> { ReferenceParser.Parse(reference) }));
        });//.RequireAuthorization("User");

        app.MapPost("/verses/verse-card", async (
            [FromBody] GetVerseCardRequest request,
            [FromServices] VerseData data) =>
        {
            return Results.Ok(await data.GetVerseCardResponse(request.UserId, request.VerseIds));
        });//.RequireAuthorization("User");

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