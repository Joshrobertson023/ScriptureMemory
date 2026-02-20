//using DataAccess.DataInterfaces;
//using Microsoft.AspNetCore.Mvc;

//namespace VerseAppNew.Server.Endpoints;

//public static class PracticeLogEndpoint
//{
//    public static void ConfigurePracticeLogEndpoints(this WebApplication app)
//    {
//        app.MapPost("/practice/record/{username}", RecordPractice);
//        app.MapGet("/practice/streak/{username}", GetStreakLength);
//        app.MapGet("/practice/history/{username}", GetPracticeHistory);
//    }

//    private static async Task<IResult> RecordPractice(
//        string username,
//        [FromServices] IPracticeLogData practiceLogData)
//    {
//        try
//        {
//            var streakLength = await practiceLogData.RecordPractice(username);
//            return Results.Ok(new { streakLength });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetStreakLength(
//        string username,
//        [FromServices] IPracticeLogData practiceLogData)
//    {
//        try
//        {
//            var streakLength = await practiceLogData.GetCurrentStreakLength(username);
//            return Results.Ok(new { streakLength });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetPracticeHistory(
//        string username,
//        [FromServices] IPracticeLogData practiceLogData)
//    {
//        try
//        {
//            var history = await practiceLogData.GetPracticeHistory(username);
//            return Results.Ok(history);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }
//}

