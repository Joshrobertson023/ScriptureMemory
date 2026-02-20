//using DataAccess.DataInterfaces;
//using DataAccess.Models;
//using Microsoft.AspNetCore.Mvc;
//using VerseAppLibrary;

//namespace VerseAppNew.Server.Endpoints;

//public static class HighlightEndpoint
//{
//    public static void ConfigureHighlightEndpoints(this WebApplication app)
//    {
//        app.MapPost("/highlights", AddHighlight);
//        app.MapDelete("/highlights/{username}/{verseReference}", RemoveHighlight);
//        app.MapGet("/highlights/{username}", GetHighlightsByUsername);
//        app.MapGet("/highlights/{username}/chapter/{book}/{chapter}", GetHighlightsByChapter);
//        app.MapGet("/highlights/{username}/check/{verseReference}", CheckHighlight);
//    }

//    private static async Task<IResult> AddHighlight(
//        [FromBody] AddHighlightRequest request,
//        [FromServices] IHighlightData highlightData)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.ReadableReference))
//            {
//                return Results.BadRequest("Username and readable reference are required");
//            }

//            // Parse the readable reference into individual verse references
//            var individualReferences = ReferenceParse.GetReferencesFromVersesInReference(request.ReadableReference);
            
//            if (individualReferences.Count == 0)
//            {
//                return Results.BadRequest("Invalid verse reference format");
//            }

//            // Insert each individual verse reference
//            foreach (var verseRef in individualReferences)
//            {
//                var highlight = new Highlight
//                {
//                    Username = request.Username,
//                    VerseReference = verseRef
//                };

//                // Check if already highlighted to avoid duplicates
//                var isHighlighted = await highlightData.IsHighlighted(request.Username, verseRef);
//                if (!isHighlighted)
//                {
//                    await highlightData.InsertHighlight(highlight);
//                }
//            }

//            return Results.Ok(new { message = "Verses highlighted successfully", count = individualReferences.Count });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> RemoveHighlight(
//        string username,
//        string verseReference,
//        [FromServices] IHighlightData highlightData)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(verseReference))
//            {
//                return Results.BadRequest("Username and verse reference are required");
//            }

//            // Parse the readable reference into individual verse references
//            var individualReferences = ReferenceParse.GetReferencesFromVersesInReference(verseReference);
            
//            if (individualReferences.Count == 0)
//            {
//                return Results.BadRequest("Invalid verse reference format");
//            }

//            // Remove each individual verse reference
//            foreach (var verseRef in individualReferences)
//            {
//                await highlightData.DeleteHighlight(username, verseRef);
//            }

//            return Results.Ok(new { message = "Verses unhighlighted successfully", count = individualReferences.Count });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetHighlightsByUsername(
//        string username,
//        [FromServices] IHighlightData highlightData)
//    {
//        try
//        {
//            var highlights = await highlightData.GetHighlightsByUsername(username);
//            return Results.Ok(highlights);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetHighlightsByChapter(
//        string username,
//        string book,
//        int chapter,
//        [FromServices] IHighlightData highlightData)
//    {
//        try
//        {
//            var highlights = await highlightData.GetHighlightsByChapter(username, book, chapter);
//            return Results.Ok(highlights);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> CheckHighlight(
//        string username,
//        string verseReference,
//        [FromServices] IHighlightData highlightData)
//    {
//        try
//        {
//            var isHighlighted = await highlightData.IsHighlighted(username, verseReference);
//            return Results.Ok(new { isHighlighted });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private record AddHighlightRequest(string Username, string ReadableReference);
//}

