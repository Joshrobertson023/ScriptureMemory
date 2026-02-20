//using System;
//using System.Linq;
//using DataAccess.DataInterfaces;
//using DataAccess.Models;
//using Microsoft.AspNetCore.Mvc;

//namespace VerseAppNew.Server.Endpoints;

//public static class VerseOfDayEndpoint
//{
//    public static void ConfigureVerseOfDayEndpoints(this WebApplication app)
//    {
//        app.MapGet("/verseofday/current", GetCurrentVerseOfDay);
//        app.MapGet("/verseofday/current/userverse", GetCurrentVerseOfDayAsUserVerse);
//        app.MapPost("/verseofday/suggest", SuggestVerseOfDay);
//        app.MapPost("/verseofday/reset-queue", ResetQueueToBeginning);
//    }

//    private static async Task<IResult> GetCurrentVerseOfDay(
//        [FromServices] IVerseOfDayData verseOfDayData)
//    {
//        try
//        {
//            var verse = await verseOfDayData.GetCurrentVerseOfDay();
//            if (verse == null)
//            {
//                return Results.NoContent();
//            }

//            return Results.Ok(verse);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetCurrentVerseOfDayAsUserVerse(
//        [FromServices] IVerseOfDayData verseOfDayData,
//        [FromServices] IVerseData verseData)
//    {
//        try
//        {
//            var verseOfDay = await verseOfDayData.GetCurrentVerseOfDay();
//            if (verseOfDay == null || string.IsNullOrWhiteSpace(verseOfDay.ReadableReference))
//            {
//                return Results.NoContent();
//            }

//            var searchResults = await verseData.GetVerseSearchResults(verseOfDay.ReadableReference);
//            if (searchResults == null || searchResults.Verses == null || !searchResults.Verses.Any())
//            {
//                return Results.NoContent();
//            }

//            string readableReference = !string.IsNullOrWhiteSpace(searchResults.Readable_Reference) 
//                ? searchResults.Readable_Reference 
//                : verseOfDay.ReadableReference;

//            var userVerse = new Passage
//            {
//                Username = string.Empty,
//                ReadableReference = readableReference,
//                Verses = searchResults.Verses.ToList()
//            };

//            return Results.Ok(userVerse);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> SuggestVerseOfDay(
//        [FromBody] SuggestVerseOfDayRequest request,
//        [FromServices] IVerseOfDaySuggestionData suggestionData)
//    {
//        try
//        {
//            var trimmedReference = request.ReadableReference?.Trim();
//            if (string.IsNullOrWhiteSpace(trimmedReference))
//            {
//                return Results.BadRequest(new { message = "Verse reference is required." });
//            }

//            if (string.IsNullOrWhiteSpace(request.Username))
//            {
//                return Results.BadRequest(new { message = "Username is required." });
//            }

//            var suggestion = new VerseOfDaySuggestion
//            {
//                ReadableReference = trimmedReference,
//                SuggesterUsername = request.Username,
//                Status = "PENDING"
//            };

//            await suggestionData.CreateSuggestion(suggestion);
//            return Results.Ok(new { message = "Suggestion submitted successfully" });
//        }
//        catch (Exception ex)
//        {
//            return Results.BadRequest(new { message = ex.Message });
//        }
//    }

//    private static async Task<IResult> ResetQueueToBeginning(
//        [FromServices] IVerseOfDayData verseOfDayData)
//    {
//        try
//        {
//            await verseOfDayData.ResetQueueToBeginning();
//            return Results.Ok(new { message = "Queue reset to beginning successfully" });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }
//}

//public class SuggestVerseOfDayRequest
//{
//    public string ReadableReference { get; set; } = string.Empty;
//    public string Username { get; set; } = string.Empty;
//}

