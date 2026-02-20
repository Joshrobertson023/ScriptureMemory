//using Azure.Core;
//using DataAccess.Models;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text.Json;
//using System.Xml;
//using VerseAppNew.Server.Services;
//using VerseAppLibrary;
//using DataAccess.DataInterfaces;

//namespace VerseAppNew.Server.Endpoints;

//public static class CollectionEndpoint
//{
//    public static void ConfigureCollectionEndpoints(this WebApplication app)
//    {
//        app.MapPost("/collections", InsertCollection);
//        app.MapPut("/collections", UpdateCollection);
//        app.MapPut("/collections/userSaved", SetUserSavedCollection);
//        app.MapPut("/collections/published/saved/{username}", IncrementPublishedCollectionSaveCount);
//        app.MapDelete("/collections/{id:int}", DeleteCollection);
//        app.MapPost("/collections/get", GetCollection);
//        app.MapGet("/collections/mostrecent/{username}", GetRecentCollection);
//        app.MapGet("/collections/all/{username}", GetUserCollectionsWithUserVerses);
//        app.MapGet("/collections/public/{username}", GetUserPublicCollections);
//        app.MapGet("/collections/friend/{username}", GetUserFriendCollections);
//        app.MapGet("/collections/friend/{collectionId:int}/verses", GetFriendCollectionWithVerses);
//        app.MapGet("/collections/byId/{id:int}", GetCollectionById);
//        app.MapGet("/collections/published/{id:int}", GetPublishedCollectionById);
//        app.MapGet("/collections/published/{id:int}/source", GetPublishedCollectionSource);
//        app.MapGet("/collections/published/author/{username}", GetPublishedCollectionsByAuthor);
//        app.MapGet("/collections/published/popular/{top}", GetPopularPublishedCollections);
//        app.MapGet("/collections/published/recent/{top}", GetRecentPublishedCollections);
//        app.MapPost("/collections/published/search", SearchPublishedCollections);
//        app.MapPost("/collections/publish", PublishCollection);
//        app.MapPost("/collections/notes", InsertCollectionNote);
//        app.MapPut("/collections/notes", UpdateCollectionNote);
//        app.MapDelete("/collections/notes/{noteId}/{collectionId:int}", DeleteCollectionNote);
//    }

//    public static async Task<IResult> GetRecentCollection(
//        string username,
//        [FromServices] ICollectionData colData)
//    {
//        try
//        {
//            var results = await colData.GetMostRecentCollection(username);
//            if (results == null)
//                return Results.NotFound();
//            return Results.Text(results.ToString());
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> InsertCollection(
//        [FromBody] Collection collection,
//        [FromServices] ICollectionData colData)
//    {
//        try
//        {
//            Debug.WriteLine(JsonSerializer.Serialize(collection, new JsonSerializerOptions { WriteIndented = true }));
//            await colData.InsertCollection(collection);
            
//            // Get the collection ID and insert notes if any
//            var collectionId = await colData.GetMostRecentCollection(collection.Username);
//            if (collectionId > 0 && collection.Notes != null && collection.Notes.Count > 0)
//            {
//                foreach (var note in collection.Notes)
//                {
//                    note.CollectionId = collectionId;
//                    await colData.InsertCollectionNote(note);
//                }
//            }
            
//            return Results.Ok();
//        }
//        catch (InvalidOperationException ex)
//        {
//            return Results.BadRequest(ex.Message);
//        }
//        catch (ArgumentException ex)
//        {
//            return Results.BadRequest(ex.Message);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> UpdateCollection(
//        [FromBody] Collection collection,
//        [FromServices] ICollectionData colData,
//        [FromServices] IUserPassageData uvData)
//    {
//        try
//        {
//            await colData.UpdateCollection(collection);
//            if (collection.CollectionId > 0)
//            {
//                var verseReferences = (collection.VerseOrder ?? string.Empty)
//                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
//                    .Select(r => r.Trim());
//                await uvData.DeleteUserVersesNotInOrder(collection.CollectionId, verseReferences);
                
//                // Handle notes: delete all existing notes and re-insert
//                await colData.DeleteCollectionNotesByCollection(collection.CollectionId);
//                if (collection.Notes != null && collection.Notes.Count > 0)
//                {
//                    foreach (var note in collection.Notes)
//                    {
//                        note.CollectionId = collection.CollectionId;
//                        await colData.InsertCollectionNote(note);
//                    }
//                }
//            }
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> SetUserSavedCollection(
//        [FromBody] int id,
//        [FromServices] ICollectionData colData)
//    {
//        try
//        {
//            await colData.SetUserSavedCollection(id);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> IncrementPublishedCollectionSaveCount(
//        string username,
//        [FromBody] PublishedCollection collection,
//        [FromServices] ICollectionData colData,
//        [FromServices] IUserData userData,
//        [FromServices] INotificationDispatcher notificationDispatcher)
//    {
//        try
//        {
//            await colData.IncrementPublishedCollectionSaves(collection.PublishedId);

//            // Check if the author wants to be notified when their collection is saved
//            var author = await userData.GetUser(collection.Author);
//            if (author != null && author.NotifyCollectionSaved == true)
//            {
//                var notification = new Notification
//                {
//                    Username = collection.Author,
//                    SenderUsername = "SYSTEM",
//                    Message = $"Someone saved your published collection \"{collection.Title}\"",
//                    CreatedDate = DateTime.UtcNow.ToUniversalTime(),
//                    ExpirationDate = DateTime.UtcNow.ToUniversalTime().AddDays(7),
//                    IsRead = false,
//                    NotificationType = "SAVED_PUBLISHED_COLLECTION",
//                };

//                await notificationDispatcher.SendNotificationAsync(notification);
//            }

//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> DeleteCollection(
//        [FromRoute] int id,
//        [FromServices] ICollectionData colData,
//        [FromServices] IUserPassageData uvData)
//    {
//        try
//        {
            
//            await colData.DeleteCollection(id, uvData);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> GetCollection(
//        [FromBody] Collection collection,
//        [FromServices] ICollectionData colData)
//    {
//        try
//        {
//            var results = await colData.GetCollection(collection);
//            if (results == null)
//                return Results.NotFound();
//            return Results.Ok(results);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> GetUserCollectionsWithUserVerses(
//        string username,
//        [FromServices] ICollectionData colData)
//    {
//        try
//        {
//            Debug.WriteLine("\n\nGetting user collections: " + username);
//            var results = await colData.GetUserCollectionsWithUserVerses(username);
//            if (results == null)
//                return Results.NotFound();
//            return Results.Ok(results);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> GetUserPublicCollections(
//        string username,
//        [FromServices] ICollectionData colData)
//    {
//        try
//        {
//            var results = await colData.GetUserPublicCollections(username);
//            return Results.Ok(results);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> GetUserFriendCollections(
//        string username,
//        string viewerUsername,
//        [FromServices] ICollectionData colData)
//    {
//        try
//        {
//            Debug.WriteLine("\n\n\n\n\n\n\n\n" + username + " " + viewerUsername);
//            var results = await colData.GetUserFriendCollections(username, viewerUsername);
//            return Results.Ok(results);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> GetFriendCollectionWithVerses(
//        int collectionId,
//        string? viewerUsername,
//        [FromServices] ICollectionData colData)
//    {
//        try
//        {
//            var result = await colData.GetFriendCollectionWithVerses(collectionId, viewerUsername);
//            if (result == null)
//            {
//                return Results.NotFound();
//            }

//            return Results.Ok(result);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> GetCollectionById(
//        [FromRoute] int id,
//        [FromServices] ICollectionData colData)
//    {
//        try
//        {
//            var result = await colData.GetCollectionById(id);
//            if (result == null)
//                return Results.NotFound();
//            return Results.Ok(result);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> GetPublishedCollectionById(
//        [FromRoute] int id,
//        [FromServices] ICollectionData colData)
//    {
//        try
//        {
//            var result = await colData.GetPublishedCollectionById(id);
//            if (result == null)
//                return Results.NotFound();
//            return Results.Ok(result);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> GetPublishedCollectionSource(
//        [FromRoute] int id,
//        [FromServices] ICollectionData colData)
//    {
//        try
//        {
//            var result = await colData.GetCollectionByPublishedId(id);
//            if (result == null)
//            {
//                return Results.NotFound();
//            }

//            return Results.Ok(result);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> GetPublishedCollectionsByAuthor(
//        string username,
//        [FromServices] ICollectionData colData)
//    {
//        if (string.IsNullOrWhiteSpace(username))
//        {
//            return Results.BadRequest("Username is required.");
//        }

//        try
//        {
//            var results = await colData.GetPublishedCollectionsByAuthor(username);
//            return Results.Ok(results);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> GetPopularPublishedCollections(
//        int top,
//        [FromServices] ICollectionData colData)
//    {
//        try
//        {
//            var results = await colData.GetPopularCollections(top);
//            Debug.WriteLine(JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true }));
//            return Results.Ok(results);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> GetRecentPublishedCollections(
//        int top,
//        [FromServices] ICollectionData colData)
//    {
//        try
//        {
//            var results = await colData.GetRecentCollections(top);
//            return Results.Ok(results);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public class SearchPublishedCollectionsRequest
//    {
//        public string? Query { get; set; }
//        public List<string>? VerseReferences { get; set; }
//        public int? Limit { get; set; }
//    }

//    public static async Task<IResult> SearchPublishedCollections(
//        [FromBody] SearchPublishedCollectionsRequest request,
//        [FromServices] ICollectionData colData)
//    {
//        if (request == null)
//        {
//            return Results.BadRequest("Request cannot be null.");
//        }

//        var verseReferences = request.VerseReferences ?? new List<string>();
//        var limit = request.Limit.HasValue && request.Limit.Value > 0 ? request.Limit.Value : 50;

//        try
//        {
//            var results = await colData.SearchPublishedCollections(request.Query ?? string.Empty, verseReferences, limit);
//            return Results.Ok(results);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public class PublishCollectionInfo
//    {
//        public Collection Collection { get; set; }
//        public string Description { get; set; }
//        public List<int> CategoryIds { get; set; }
//    }

//    public static async Task<IResult> PublishCollection(
//        [FromBody] PublishCollectionInfo info,
//        [FromServices] ICollectionData colData,
//        [FromServices] IUserPassageData uvData,
//        [FromServices] IAdminData adminData,
//        [FromServices] INotificationDispatcher notificationDispatcher)
//    {
//        try
//        {
//            if (info == null)
//            {
//                return Results.BadRequest("Request body cannot be null.");
//            }
            
//            if (info.Collection == null)
//            {
//                return Results.BadRequest("Collection cannot be null.");
//            }

//            var collectionBeingPublished = info.Collection;
            
            
//            if (string.IsNullOrWhiteSpace(collectionBeingPublished.Title))
//            {
//                return Results.BadRequest("Collection title is required.");
//            }
            
//            if (string.IsNullOrWhiteSpace(collectionBeingPublished.AuthorUsername))
//            {
//                return Results.BadRequest("Collection author username is required.");
//            }
            
            
//            if (collectionBeingPublished.UserVerses == null)
//            {
//                collectionBeingPublished.UserVerses = new List<UserPassage>();
//            }
            
            
//            if (info.CategoryIds == null)
//            {
//                info.CategoryIds = new List<int>();
//            }
            
//            var publishedId = await colData.PublishCollection(collectionBeingPublished, info.Description);
//            if (publishedId == 0)
//            {
//                return Results.Problem("Failed to retrieve published collection ID after publishing.");
//            }
//            await colData.AssignNewPublishedCollectionCategories(publishedId, info.CategoryIds);
            
//            // Save notes for published collection (notes are saved in PublishCollection method)
//            await uvData.AddUserVersesToNewlyPublishedCollection(collectionBeingPublished.UserVerses.ToList(), publishedId);
            
//            // Notify all admins that a collection needs review
//            var adminUsernames = await adminData.GetAdminUsernames();
//            var adminNotifications = adminUsernames.Select(adminUsername => new Notification
//            {
//                Username = adminUsername,
//                SenderUsername = "SYSTEM",
//                Message = $"Collection \"{collectionBeingPublished.Title}\" by {collectionBeingPublished.AuthorUsername} needs review",
//                CreatedDate = DateTime.UtcNow,
//                ExpirationDate = DateTime.UtcNow.AddDays(7),
//                IsRead = false,
//                NotificationType = "COLLECTION_REVIEW"
//            });

//            await notificationDispatcher.SendNotificationsAsync(adminNotifications);
            
//            return Results.Ok(new { message = "Collection submitted for review. It usually takes under a day for review." });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> InsertCollectionNote(
//        [FromBody] CollectionNote note,
//        [FromServices] ICollectionData colData)
//    {
//        try
//        {
//            await colData.InsertCollectionNote(note);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> UpdateCollectionNote(
//        [FromBody] CollectionNote note,
//        [FromServices] ICollectionData colData)
//    {
//        try
//        {
//            await colData.UpdateCollectionNote(note);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    public static async Task<IResult> DeleteCollectionNote(
//        [FromRoute] string noteId,
//        [FromRoute] int collectionId,
//        [FromServices] ICollectionData colData)
//    {
//        try
//        {
//            await colData.DeleteCollectionNote(noteId, collectionId);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }
//}
