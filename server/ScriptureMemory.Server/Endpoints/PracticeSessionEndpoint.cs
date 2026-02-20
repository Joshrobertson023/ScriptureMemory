//using DataAccess.DataInterfaces;
//using DataAccess.Models;
//using Microsoft.AspNetCore.Mvc;

//namespace VerseAppNew.Server.Endpoints;

//public static class PracticeSessionEndpoint
//{
//    public static void ConfigurePracticeSessionEndpoints(this WebApplication app)
//    {
//        app.MapPost("/practicesessions", InsertPracticeSession);
//        app.MapGet("/practicesessions/verse/{username}", GetPracticeSessionsByVerse);
//        app.MapGet("/practicesessions/userverse/{username}/{userVerseId:int}", GetPracticeSessionsByUserVerseId);
//    }

//    private static async Task<IResult> InsertPracticeSession(
//        [FromBody] PracticeSession session,
//        [FromServices] IPracticeSessionData practiceSessionData)
//    {
//        try
//        {
//            if (session == null)
//            {
//                return Results.BadRequest("Practice session data is required");
//            }

//            session.CreatedDate = DateTime.UtcNow;
//            if (session.PracticeDate == default)
//            {
//                session.PracticeDate = DateTime.UtcNow.Date;
//            }

//            var sessionId = await practiceSessionData.InsertPracticeSession(session);
//            return Results.Ok(new { sessionId });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetPracticeSessionsByVerse(
//        string username,
//        [FromQuery] string readableReference,
//        [FromServices] IPracticeSessionData practiceSessionData,
//        [FromQuery] int limit = 5)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(readableReference))
//            {
//                return Results.BadRequest("Username and readableReference are required");
//            }

//            var sessions = await practiceSessionData.GetPracticeSessionsByVerse(username, readableReference, limit);
//            return Results.Ok(sessions);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetPracticeSessionsByUserVerseId(
//        string username,
//        int userVerseId,
//        [FromServices] IPracticeSessionData practiceSessionData,
//        [FromQuery] int limit = 5)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(username) || userVerseId <= 0)
//            {
//                return Results.BadRequest("Username and valid userVerseId are required");
//            }

//            var sessions = await practiceSessionData.GetPracticeSessionsByUserVerseId(username, userVerseId, limit);
//            return Results.Ok(sessions);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }
//}

