//using DataAccess.DataInterfaces;
//using DataAccess.Requests;
//using Microsoft.AspNetCore.Mvc;
//using System.Diagnostics;
//using System.Linq;
//using System.Text.Json;
//using VerseAppLibrary;
//using VerseAppNew.Server.Services;

//namespace VerseAppNew.Server.Endpoints;

//public static class UserPassageEndpoint
//{
//    public static void ConfigureUserPassageEndpoints(this WebApplication app)
//    {
//        app.MapPost("/userverses", InsertUserVerse);
//        app.MapPost("/userverses/newcollection/{colId:int}", InsertUserVersesInNewCollection);
//        app.MapPost("/userverses/populate", PopulateUserVerses);
//        app.MapGet("/userverses/{id:int}", GetUserVerse);
//        app.MapGet("/userverses/user/{username}", GetUserVersesByUsername);
//        app.MapGet("/userverses/collection/{id:int}", GetUserVersesByCollection);
//        app.MapPut("/userverses/memorize", MemorizeUserVerse);
//        app.MapDelete("/userverses/", DeleteUserVerse);
//        app.MapDelete("/userverses/collection/{id:int}", DeleteUserVersesByCollection);
//        app.MapGet("/userverses/memorized/unpopulated/{username}", GetUnpopulatedMemorized);
//        app.MapGet("/userverses/inprogress/{username}", GetInProgress);
//        app.MapGet("/userverses/notstarted/{username}", GetNotStarted);
//        app.MapGet("/userverses/recent/{username}", GetRecentPractice);
//        app.MapGet("/userverses/overdue/{username}", GetOverdueVerses);
//        app.MapPost("/userverses/memorize-verse-of-day", MemorizeVerseOfDay);
//        app.MapPost("/userverses/parse", GetUserVerseParts);
//    }

//    public static async Task<IResult> GetUserPassageParts(
//        [FromBody] UserPassage userPassage,
//        [FromServices] IUserPassageService service)
//    {
//        return await service.GetUserPassageParts(userPassage);
//    }

//    public static async Task<IResult> GetUnpopulatedMemorized(
//    string username,
//    [FromServices] IUserPassageData uvData,
//    [FromServices] ICollectionData colData)
//    {
//        try
//        {
//            var memorized = await uvData.GetMemorized(username);
//            Debug.WriteLine(JsonSerializer.Serialize(
//                memorized,
//                new JsonSerializerOptions { WriteIndented = true }
//            ));
//            return Results.Ok(memorized);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> GetInProgress(
//    string username,
//    [FromServices] IUserPassageData uvData,
//    [FromServices] ICollectionData colData)
//    {
//        try
//        {
//            var inProgress = await uvData.GetInProgress(username);
//            return Results.Ok(inProgress);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> GetNotStarted(
//        string username,
//        [FromServices] IUserPassageData uvData,
//        [FromServices] ICollectionData colData)
//    {
//        try
//        {
//            var notStarted = await uvData.GetNotStarted(username);
//            return Results.Ok(notStarted);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> PopulateUserVerses(
//        [FromBody] UserPassage[] userVerses,
//        [FromServices] ICollectionData colData)
//    {
//        try
//        {
//            if (userVerses == null || userVerses.Length == 0)
//                return Results.Ok(userVerses);
//            var collection = new Collection();
//            collection.UserVerses = userVerses.ToList();
//            var result = await colData.GetCollection(collection);
//            // NextPracticeDate is already calculated in GetCollection
//            return Results.Ok(result.UserVerses);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> InsertUserVersesInNewCollection(
//        [FromRoute] int colId,
//        [FromBody] UserPassage[] userVerses,
//        [FromServices] IUserPassageData uvData)
//    {
//        try
//        {
//            await uvData.InsertUserVersesToNewCollection(userVerses.ToList(), colId);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> InsertUserVerse(
//        [FromBody] UserPassage userVerse,
//        [FromServices] IUserPassageData uvData)
//    {
//        try
//        {
//            await uvData.InsertUserVerse(userVerse);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public class MemorizedInfo
//    {
//        public int UserVerseId { get; set; }
//        public int Accuracy { get; set; }
//    }

//    public static async Task<IResult> MemorizeUserVerse(
//        [FromBody] MemorizedInfo info,
//        [FromServices] IUserPassageData uvData,
//        [FromServices] IUserData userData,
//        [FromServices] IRelationshipData relationshipData,
//        [FromServices] DataAccess.DataInterfaces.IVerseData vData,
//        [FromServices] IActivityData activityData,
//        [FromServices] INotificationDispatcher notificationDispatcher)
//    {
//        try
//        {
//            PassageMemorizedRequest uvInfo = new();

//            uvInfo.userVerse = await uvData.GetUserVerse(info.UserVerseId);

//            uvInfo.PointsGained = (int)Math.Floor((Math.Pow(info.Accuracy, 4) / 1000000));
//            // Accuracy = 95, so 95^4 = 81450625. Then 81.450625, then 81
//            // Accuracy of 100 gains 100 points, accuracy of 80 gains half than an accuracy of 95 does

//            await userData.IncrementPoints(uvInfo.userVerse.Username, uvInfo.PointsGained);

//            uvInfo.userVerse.DueDate = DateTime.Now.AddDays((int)(((info.Accuracy / 10) / 3) + 1) + Math.Pow(uvInfo.userVerse.TimesMemorized + 1, 3) - 1);
//            // Accuracy of 95 => 9, 9 / 3 = 3, 3 + 1 = 4 days from now. Then multiply timesMemorized^3
//            // Accuracy of 55 => 2.6 = 2 days from now
//            // 1^3 = 1, 2^3 = 8, 3^3 = 27 4^3 = 64
//            // So for 90 accuracy: times memorized = 0 (0+1) => 4 days, tm = 1 => 11 days, tm = 2 => 30 days, tm = 3 => 67
            
//            uvInfo.userVerse.TimesMemorized++;
//            uvInfo.userVerse.ProgressPercent = uvInfo.userVerse.TimesMemorized * (info.Accuracy * 2);
//            await uvData.UpdateUserVerse(uvInfo.userVerse);

//            var user = await userData.GetUser(uvInfo.userVerse.Username);

//            if (user != null && user.NotifyMemorizedVerse == true)
//            {
//                var activity = new DataAccess.Models.Activity
//                {
//                    Text = $"You memorized {uvInfo.userVerse.ReadableReference}",
//                    Username = uvInfo.userVerse.Username,
//                    DateCreated = DateTime.UtcNow
//                };
//                await activityData.CreateActivity(activity);

//                var friends = await relationshipData.GetFriends(uvInfo.userVerse.Username);

//                var notifications = friends.Select(friend => new Notification
//                {
//                    Username = friend.Username,
//                    SenderUsername = user.Username,
//                    Message = $"{user.FirstName} {user.LastName} memorized {uvInfo.userVerse.ReadableReference}!",
//                    CreatedDate = DateTime.UtcNow,
//                    IsRead = false,
//                    NotificationType = "ACTIVITY"
//                });

//                await notificationDispatcher.SendNotificationsAsync(notifications);
//            }

//            List<string> verses = ReferenceParse.GetReferencesFromVersesInReference(uvInfo.userVerse.ReadableReference);
//            foreach (var verse in verses)
//            {
//                await vData.UpdateUsersMemorizedVerse(verse);
//            }
            
//            return Results.Ok(uvInfo);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> DeleteUserVerse(
//        [FromBody] UserPassage userVerse,
//        [FromServices] IUserPassageData uvData)
//    {
//        try
//        {
//            await uvData.DeleteUserVerse(userVerse);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> GetUserVerse(
//        [FromRoute] int id, 
//        [FromServices] IUserPassageData uvData,
//        [FromServices] DataAccess.DataInterfaces.IVerseData vData)
//    {
//        try
//        {
//            var userVerse = await uvData.GetUserVerse(id);
//            if (userVerse == null)
//                return Results.NotFound();

//            var verseReferences = ReferenceParse.GetReferencesFromVersesInReference(
//                    userVerse.ReadableReference);
//            foreach (var reference in verseReferences)
//            {
//                var verse = await vData.GetVerse(reference);
//                if (verse != null)
//                {
//                    userVerse.Verses.Add(verse);
//                }
//            }

//            return Results.Ok(userVerse);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> GetUserVersesByUsername(
//        [FromRoute] string username,
//        [FromServices] IUserPassageData uvData)
//    {
//        try
//        {
//            var userVerses = await uvData.GetUserVersesByUsername(username);
//            return Results.Ok(userVerses);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> GetUserVersesByCollection(
//        [FromRoute] int id,
//        [FromServices] IUserPassageData uvData,
//        [FromServices] DataAccess.DataInterfaces.IVerseData vData)
//    {
//        try
//        {
//            var userVerses = await uvData.GetUserVersesByCollection(id);
//            foreach (var uv in userVerses)
//            {
//                var verseReferences = ReferenceParse.GetReferencesFromVersesInReference(
//                    uv.ReadableReference);
//                foreach (var reference in verseReferences)
//                {
//                    var verse = await vData.GetVerse(reference);
//                    if (verse != null)
//                    {
//                        uv.Verses.Add(verse);
//                    }
//                }
//            }
//            return Results.Ok(userVerses);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> DeleteUserVersesByCollection(
//        [FromRoute] int id,
//        [FromServices] IUserPassageData uvData)
//    {
//        try
//        {
//            await uvData.DeleteUserVersesByCollection(id);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> GetRecentPractice(
//        string username,
//        [FromServices] IUserPassageData uvData)
//    {
//        try
//        {
//            var recent = await uvData.GetRecentPractice(username, 3);
//            return Results.Ok(recent);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> GetOverdueVerses(
//        string username,
//        [FromServices] IUserPassageData uvData)
//    {
//        try
//        {
//            var overdue = await uvData.GetOverdueVerses(username);
//            return Results.Ok(overdue);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public class MemorizeVerseOfDayRequest
//    {
//        public string Username { get; set; }
//        public string ReadableReference { get; set; }
//    }

//    public static async Task<IResult> MemorizeVerseOfDay(
//        [FromBody] MemorizeVerseOfDayRequest request,
//        [FromServices] IUserData userData,
//        [FromServices] IRelationshipData relationshipData,
//        [FromServices] DataAccess.DataInterfaces.IVerseData vData,
//        [FromServices] IActivityData activityData,
//        [FromServices] INotificationDispatcher notificationDispatcher)
//    {
//        try
//        {
//            List<string> verses = ReferenceParse.GetReferencesFromVersesInReference(request.ReadableReference);
//            foreach (var verse in verses)
//            {
//                await vData.UpdateUsersMemorizedVerse(verse);
//            }

//            var user = await userData.GetUser(request.Username);
            
//            // Check if user wants to notify friends about memorized verses
//            if (user != null && user.NotifyMemorizedVerse == true)
//            {
//                var activity = new DataAccess.Models.Activity
//                {
//                    Text = $"You memorized {request.ReadableReference}",
//                    Username = request.Username,
//                    DateCreated = DateTime.UtcNow
//                };
//                await activityData.CreateActivity(activity);
                
//                var friends = await relationshipData.GetFriends(request.Username);

//                var notifications = friends.Select(friend => new Notification
//                {
//                    Username = friend.Username,
//                    SenderUsername = user.Username,
//                    Message = $"{user.FirstName} {user.LastName} memorized {request.ReadableReference}!",
//                    CreatedDate = DateTime.UtcNow,
//                    IsRead = false,
//                    NotificationType = "ACTIVITY"
//                });

//                await notificationDispatcher.SendNotificationsAsync(notifications);
//            }
            
//            await userData.IncrementPoints(request.Username, 25);
            
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }
//}
