using DataAccess.DataInterfaces;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace VerseAppNew.Server.Endpoints;

public static class ActivityEndpoint
{
    public static void ConfigureActivityEndpoints(this WebApplication app)
    {
        app.MapGet("/activity/user/{username}", GetUserActivity);
        app.MapGet("/activity/friends/{username}", GetFriendsActivity);
        app.MapGet("/activity/friend/{username}", GetFriendActivityForViewer);
    }

    private static async Task<IResult> GetUserActivity(
        string username,
        [FromQuery] int limit,
        [FromServices] IActivityData activityData)
    {
        var activities = await activityData.GetUserActivity(username, limit > 0 ? limit : 10);
        return Results.Ok(activities);
    }

    private static async Task<IResult> GetFriendsActivity(
        string username,
        [FromQuery] int limit,
        [FromServices] IActivityData activityData)
    {
        var activities = await activityData.GetFriendsActivity(username, limit > 0 ? limit : 10);
        return Results.Ok(activities);
    }

    private static async Task<IResult> GetFriendActivityForViewer(
        string username,
        [FromQuery] string viewerUsername,
        [FromQuery] int limit,
        [FromServices] IActivityData activityData)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(viewerUsername))
            {
                return Results.BadRequest("viewerUsername is required.");
            }

            var activities = await activityData.GetFriendActivity(username, viewerUsername, limit > 0 ? limit : 10);
            return Results.Ok(activities);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}





