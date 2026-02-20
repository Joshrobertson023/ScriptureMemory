//using System;
//using DataAccess.DataInterfaces;
//using DataAccess.Models;
//using Microsoft.AspNetCore.Mvc;
//using VerseAppNew.Server.Services;

//namespace VerseAppNew.Server.Endpoints;

//public static class RelationshipEndpoint
//{
//    public static void ConfigureRelationshipEndpoints(this WebApplication app)
//    {
//        app.MapPost("/relationships/send", SendFriendRequest);
//        app.MapPost("/relationships/respond", RespondToFriendRequest);
//        app.MapGet("/relationships/friends/{username}", GetFriends);
//        app.MapGet("/relationships/friends/{username}/names", GetFriendNames);
//        app.MapGet("/relationships/pending/{username}", GetPendingRequests);
//        app.MapGet("/relationships/check/{username1}/{username2}", CheckRelationship);
//        app.MapDelete("/relationships/{username1}/{username2}", DeleteFriend);
//    }

//    private static async Task<IResult> SendFriendRequest(
//        [FromBody] SendFriendRequestRequest request,
//        [FromServices] IRelationshipData relationshipData,
//        [FromServices] INotificationDispatcher notificationDispatcher)
//    {
//        try
//        {
            
//            var existingRelationship = await relationshipData.GetRelationship(request.FromUsername, request.ToUsername);
            
//            if (existingRelationship != null)
//            {
//                if (existingRelationship.Type == 1) 
//                {
//                    return Results.BadRequest(new { message = "You are already friends" });
//                }
//                if (existingRelationship.Type == 0) 
//                {
//                    return Results.BadRequest(new { message = "Friend request already pending" });
//                }
//            }

            
//            var relationship = new Relationship
//            {
//                Username1 = request.FromUsername,
//                Username2 = request.ToUsername,
//                Type = 0 
//            };
            
//            await relationshipData.CreateRelationship(relationship);

            
//            var notification = new Notification
//            {
//                Username = request.ToUsername,
//                SenderUsername = request.FromUsername,
//                Message = $"{request.FromUsername} sent you a friend request",
//                NotificationType = "FRIEND_REQUEST",
//                CreatedDate = DateTime.UtcNow
//            };

//            await notificationDispatcher.SendNotificationAsync(notification);

//            return Results.Ok(new { message = "Friend request sent successfully" });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> RespondToFriendRequest(
//        [FromBody] RespondToFriendRequestRequest request,
//        [FromServices] IRelationshipData relationshipData,
//        [FromServices] INotificationDispatcher notificationDispatcher)
//    {
//        try
//        {
            
//            var relationship = await relationshipData.GetRelationship(request.RequesterUsername, request.RecipientUsername);
            
//            if (relationship == null)
//            {
//                return Results.NotFound(new { message = "Friend request not found" });
//            }

//            if (relationship.Type != 0)
//            {
//                return Results.BadRequest(new { message = "Friend request has already been handled" });
//            }

//            if (request.Accept)
//            {
                
//                await relationshipData.UpdateRelationship(request.RequesterUsername, request.RecipientUsername, 1); 

                
//                var notification = new Notification
//                {
//                    Username = request.RequesterUsername,
//                    SenderUsername = request.RecipientUsername,
//                    Message = $"{request.RecipientUsername} accepted your friend request",
//                    NotificationType = "FRIEND_ACCEPTED",
//                    CreatedDate = DateTime.UtcNow
//                };

//                await notificationDispatcher.SendNotificationAsync(notification);
//            }
//            else
//            {
                
//                await relationshipData.UpdateRelationship(request.RequesterUsername, request.RecipientUsername, 2); 
//            }

//            return Results.Ok(new { message = request.Accept ? "Friend request accepted" : "Friend request rejected" });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetFriends(
//        string username,
//        [FromServices] IRelationshipData relationshipData)
//    {
//        try
//        {
//            var friends = await relationshipData.GetFriends(username);
//            return Results.Ok(friends);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetFriendNames(
//        string username,
//        [FromServices] IRelationshipData relationshipData)
//    {
//        try
//        {
//            var friends = await relationshipData.GetFriendNames(username);
//            return Results.Ok(friends);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetPendingRequests(
//        string username,
//        [FromServices] IRelationshipData relationshipData)
//    {
//        try
//        {
//            var pending = await relationshipData.GetPendingRequests(username);
//            return Results.Ok(pending);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> CheckRelationship(
//        string username1,
//        string username2,
//        [FromServices] IRelationshipData relationshipData)
//    {
//        try
//        {
//            var relationship = await relationshipData.GetRelationship(username1, username2);
//            var areFriends = await relationshipData.AreFriends(username1, username2);
            
//            return Results.Ok(new { 
//                relationship = relationship?.Type,
//                areFriends 
//            });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> DeleteFriend(
//        string username1,
//        string username2,
//        [FromServices] IRelationshipData relationshipData)
//    {
//        try
//        {
//            await relationshipData.DeleteRelationship(username1, username2);
//            return Results.Ok(new { message = "Friend removed" });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }
//}

//public class SendFriendRequestRequest
//{
//    public string FromUsername { get; set; }
//    public string ToUsername { get; set; }
//}

//public class RespondToFriendRequestRequest
//{
//    public string RequesterUsername { get; set; }
//    public string RecipientUsername { get; set; }
//    public bool Accept { get; set; }
//}

