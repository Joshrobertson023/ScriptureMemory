//using DataAccess.DataInterfaces;
//using DataAccess.Models;
//using Microsoft.AspNetCore.Mvc;

//namespace VerseAppNew.Server.Endpoints;

//public static class ReportEndpoint
//{
//    public static void ConfigureReportEndpoints(this WebApplication app)
//    {
//        app.MapPost("/reports", CreateReport);
//        app.MapGet("/reports", GetReports);
//        app.MapDelete("/reports/{id:int}", DeleteReport);
//    }

//    private static async Task<IResult> CreateReport(
//        [FromBody] CreateReportRequest request,
//        [FromServices] IReportData reportData,
//        [FromServices] INotificationData notificationData)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(request.ReporterUsername) || string.IsNullOrWhiteSpace(request.ReportedUsername) || string.IsNullOrWhiteSpace(request.Reason))
//            {
//                return Results.BadRequest(new { message = "Missing required fields" });
//            }

//            var report = new Report
//            {
//                Reporter_Username = request.ReporterUsername,
//                Reported_Username = request.ReportedUsername,
//                Reason = request.Reason,
//                Status = "OPEN"
//            };

//            await reportData.CreateReport(report);

            
//            var reportType = request.ReportedUsername == "SYSTEM" ? "bug report" : "user report";
//            var message = $"New {reportType} submitted by @{request.ReporterUsername}";
//            await notificationData.SendNotificationToAdmins(message, "SYSTEM");

//            return Results.Ok(new { message = "Report submitted" });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetReports(
//        [FromServices] IReportData reportData)
//    {
//        try
//        {
//            var reports = await reportData.GetAllReportsWithEmail();
//            return Results.Ok(reports);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> DeleteReport(
//        int id,
//        [FromServices] IReportData reportData)
//    {
//        try
//        {
//            await reportData.DeleteReport(id);
//            return Results.NoContent();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }
//}

//public class CreateReportRequest
//{
//    public string ReporterUsername { get; set; }
//    public string ReportedUsername { get; set; }
//    public string Reason { get; set; }
//}


