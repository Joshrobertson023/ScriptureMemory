//using DataAccess.DataInterfaces;
//using DataAccess.Models;
//using Microsoft.AspNetCore.Mvc;
//using System.Linq;
//using VerseAppLibrary;
//using VerseAppNew.Server.Services;

//namespace VerseAppNew.Server.Endpoints;

//public static class NotificationEndpoint
//{
//    public static void ConfigureNotificationEndpoints(this WebApplication app)
//    {
//        app.MapGet("/notifications/{username}", GetUserNotifications);
//        app.MapGet("/notifications/{username}/paged", GetUserNotificationsPaged);
//        app.MapPost("/notifications/read/{notificationId}", MarkNotificationAsRead);
//        app.MapPost("/notifications/readall/{username}", MarkAllNotificationsAsRead);
//        app.MapPost("/notifications/expire/{notificationId}", ExpireNotification);
//        app.MapGet("/notifications/count/{username}", GetUnreadNotificationCount);
//        app.MapPost("/notifications/share-collection", ShareCollection);
//        app.MapPost("/notifications/share-verse", ShareVerse);
//    }

//    private static async Task<IResult> GetUserNotifications(
//        string username,
//        [FromServices] INotificationData notificationData)
//    {
//        try
//        {
//            var notifications = await notificationData.GetUserNotifications(username);
//            return Results.Ok(notifications);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetUserNotificationsPaged(
//        string username,
//        [FromQuery] int pageSize,
//        [FromQuery] int? cursorId,
//        [FromServices] INotificationData notificationData)
//    {
//        try
//        {
//            List<Notification> items;

//            if (cursorId.HasValue)
//            {
//                items = await notificationData.GetNotificationsBefore(username, cursorId.Value, pageSize + 1);
//            }
//            else
//            {
//                items = await notificationData.GetTopNotifications(username, pageSize + 1);
//            }

//            var hasMore = items.Count > pageSize;
//            if (hasMore)
//            {
//                items = items.Take(pageSize).ToList();
//            }

//            int? nextCursorId = null;
//            if (hasMore && items.Count > 0)
//            {
//                var last = items[^1];
//                nextCursorId = last.Id;
//            }

//            return Results.Ok(new { items, nextCursorId });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> MarkNotificationAsRead(
//        int notificationId,
//        [FromServices] INotificationData notificationData)
//    {
//        try
//        {
//            await notificationData.MarkNotificationAsRead(notificationId);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetUnreadNotificationCount(
//        string username,
//        [FromServices] INotificationData notificationData)
//    {
//        try
//        {
//            var count = await notificationData.GetUnreadNotificationCount(username);
//            return Results.Ok(new { count });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> MarkAllNotificationsAsRead(
//        string username,
//        [FromServices] INotificationData notificationData)
//    {
//        try
//        {
//            await notificationData.MarkAllNotificationsAsRead(username);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> ExpireNotification(
//        int notificationId,
//        [FromServices] INotificationData notificationData)
//    {
//        try
//        {
//            await notificationData.ExpireNotification(notificationId);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> ShareCollection(
//        [FromBody] ShareCollectionRequest request,
//        [FromServices] ICollectionData collectionData,
//        [FromServices] IUserData userData,
//        [FromServices] INotificationDispatcher notificationDispatcher)
//    {
//        try
//        {
            
//            var collection = await collectionData.GetCollectionById(request.CollectionId);
//            if (collection == null)
//            {
//                return Results.NotFound("Collection not found");
//            }

            
//            var sharingUser = await userData.GetUser(request.FromUsername);
            
            
//            var notification = new Notification
//            {
//                Username = request.ToUsername,
//                SenderUsername = request.FromUsername,
//                Message = $"{request.FromUsername} shared a collection with you \"{collection.Title}\"",
//                CreatedDate = DateTime.UtcNow.ToUniversalTime(),
//                ExpirationDate = DateTime.UtcNow.ToUniversalTime().AddDays(1), 
//                IsRead = false,
//                NotificationType = "SHARED_COLLECTION",
//            };

            
//            notification.Message += $"|COLLECTION_ID:{request.CollectionId}|COLLECTION_TITLE:{collection.Title}";

//            await notificationDispatcher.SendNotificationAsync(notification);

//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public class ShareCollectionRequest
//    {
//        public string FromUsername { get; set; }
//        public string ToUsername { get; set; }
//        public int CollectionId { get; set; }
//    }

//    private static async Task<IResult> ShareVerse(
//        [FromBody] ShareVerseRequest request,
//        [FromServices] IUserPassageData userVerseData,
//        [FromServices] DataAccess.DataInterfaces.IVerseData verseData,
//        [FromServices] INotificationDispatcher notificationDispatcher)
//    {
//        try
//        {
//            // Support both UserVerseIds (for saved verses) and VerseReferences (for unsaved verses)
//            List<string> verseReferences = new List<string>();
            
//            if (request.UserVerseIds != null && request.UserVerseIds.Count > 0)
//            {
//                // Get userVerses by IDs if provided
//                var userVerses = await userVerseData.GetUserVersesByIds(request.UserVerseIds);
//                if (userVerses != null && userVerses.Count > 0)
//                {
//                    foreach (var uv in userVerses)
//                    {
//                        if (!string.IsNullOrWhiteSpace(uv.ReadableReference))
//                        {
//                            verseReferences.Add(uv.ReadableReference);
//                        }
//                    }
//                }
//            }
            
//            // Add direct verse references if provided
//            if (request.VerseReferences != null && request.VerseReferences.Count > 0)
//            {
//                verseReferences.AddRange(request.VerseReferences.Where(r => !string.IsNullOrWhiteSpace(r)));
//            }
            
//            if (verseReferences.Count == 0)
//            {
//                return Results.BadRequest("No verses found to share. Make sure to just share reference");
//            }

//            // Parse all references to get individual verse references
//            List<string> allIndividualReferences = new List<string>();
//            foreach (var refStr in verseReferences)
//            {
//                var individualRefs = ReferenceParse.GetReferencesFromVersesInReference(refStr);
//                allIndividualReferences.AddRange(individualRefs);
//            }

//            // Get verse text for all individual references
//            var verses = await verseData.GetAllVersesFromReferenceList(allIndividualReferences);
//            var verseTextMap = verses.ToDictionary(v => v.verse_reference, v => v.Text);

//            // Build verse data with text: reference:text|reference:text|...
//            var verseDataParts = new List<string>();
//            foreach (var refStr in allIndividualReferences)
//            {
//                var text = verseTextMap.ContainsKey(refStr) ? verseTextMap[refStr] : "";
//                verseDataParts.Add($"{refStr}:{text}");
//            }
//            var verseDataString = string.Join("|", verseDataParts);

//            // Build notification message with all verses
//            var verseCount = verseReferences.Count;
//            var messagePrefix = verseCount == 1 
//                ? $"{request.FromUsername} shared a verse with you" 
//                : $"{request.FromUsername} shared {verseCount} passages with you";

//            // Include both the original readable references and the verse data with text
//            var readableReferencesString = string.Join("|", verseReferences);

//            var notification = new Notification
//            {
//                Username = request.ToUsername,
//                SenderUsername = request.FromUsername,
//                Message = $"{messagePrefix}|VERSE_REFERENCES:{readableReferencesString}|VERSE_DATA:{verseDataString}",
//                CreatedDate = DateTime.UtcNow.ToUniversalTime(),
//                ExpirationDate = DateTime.UtcNow.ToUniversalTime().AddDays(7),
//                IsRead = false,
//                NotificationType = "SHARED_VERSE",
//            };

//            await notificationDispatcher.SendNotificationAsync(notification);

//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public class ShareVerseRequest
//    {
//        public string FromUsername { get; set; }
//        public string ToUsername { get; set; }
//        public List<int> UserVerseIds { get; set; }
//        public List<string> VerseReferences { get; set; }
//    }

//    public class SavedPublishedRequest
//    {
//        public string SaverUsername { get; set; }
//        public int CollectionId { get; set; }
//    }
//}
