//using DataAccess.DataInterfaces;
//using Microsoft.AspNetCore.Mvc;
//using static VerseAppLibrary.Enums;

//namespace VerseAppNew.Server.Endpoints;

//public static class UserPreferencesEndpoint
//{
//    public static void ConfigureUserPreferencesEndpoints(this WebApplication app)
//    {

//    }

//    private static async Task<IResult> UpdateBibleVersion(
//        string username,
//        [FromBody] BibleVersion bibleVersion,
//        [FromServices] IUserData data)
//    {
//        try
//        {
//            var user = await data.GetUser(username);
//            if (user == null)
//            {
//                return Results.NotFound();
//            }

//            user.BibleVersion = bibleVersion;
//            await data.UpdateUser(user);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> UpdateTypeOutReference(
//        string username,
//        [FromBody] bool typeOut,
//        [FromServices] IUserData data)
//    {
//        await data.SetTypeOutReference(typeOut, username);
//        return Results.Ok();
//    }

//    private static async Task<IResult> UpdateThemePreference(
//        string username,
//        [FromBody] ThemePreference preference,
//        [FromServices] IUserData data)
//    {
//        await data.UpdateThemePreference(preference, username);
//        return Results.Ok();
//    }

//    private static async Task<IResult> UpdateCollectionsOrder(
//        string username,
//        [FromBody] string order,
//        [FromServices] IUserData data)
//    {
//        await data.UpdateCollectionsOrder(order, username);
//        return Results.Ok();
//    }

//    private static async Task<IResult> UpdateCollectionsSortBy(
//        string username,
//        [FromBody] int sortBy,
//        [FromServices] IUserData data)
//    {
//        try
//        {
//            await data.UpdateCollectionsSortBy(sortBy, username);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> UpdateSubscribedVerseOfDay(
//        string username,
//        [FromBody] bool subscribed,
//        [FromServices] IUserData data)
//    {
//        try
//        {
//            await data.UpdateSubscribedVerseOfDay(subscribed, username);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> UpdatePushNotifications(
//        string username,
//        [FromBody] bool enabled,
//        [FromServices] IUserData data)
//    {
//        try
//        {
//            var user = await data.GetUser(username);
//            if (user == null)
//            {
//                return Results.NotFound();
//            }

//            user.PushNotificationsEnabled = enabled;
//            await data.UpdateUser(user);

//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> UpdateActivityNotifications(
//        string username,
//        [FromBody] bool enabled,
//        [FromServices] IUserData data)
//    {
//        try
//        {
//            await data.UpdateActivityNotifications(enabled, username);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> UpdateNotifyMemorizedVerse(
//        string username,
//        [FromBody] bool enabled,
//        [FromServices] IUserData data)
//    {
//        try
//        {
//            await data.UpdateNotifyMemorizedVerse(enabled, username);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> UpdateNotifyPublishedCollection(
//        string username,
//        [FromBody] bool enabled,
//        [FromServices] IUserData data)
//    {
//        try
//        {
//            await data.UpdateNotifyPublishedCollection(enabled, username);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> UpdateNotifyCollectionSaved(
//        string username,
//        [FromBody] bool enabled,
//        [FromServices] IUserData data)
//    {
//        try
//        {
//            await data.UpdateNotifyCollectionSaved(enabled, username);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> UpdateNotifyNoteLiked(
//        string username,
//        [FromBody] bool enabled,
//        [FromServices] IUserData data)
//    {
//        try
//        {
//            await data.UpdateNotifyNoteLiked(enabled, username);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> UpdateBadgeNotificationsEnabled(
//        string username,
//        [FromBody] bool enabled,
//        [FromServices] IUserData data)
//    {
//        try
//        {
//            await data.UpdateBadgeNotificationsEnabled(enabled, username);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> UpdateBadgeOverdueEnabled(
//        string username,
//        [FromBody] bool enabled,
//        [FromServices] IUserData data)
//    {
//        try
//        {
//            await data.UpdateBadgeOverdueEnabled(enabled, username);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> UpdatePracticeNotifications(
//        string username,
//        [FromBody] bool enabled,
//        [FromServices] IUserData data)
//    {
//        try
//        {
//            await data.UpdatePracticeNotifications(enabled, username);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> UpdateFriendsActivityNotifications(
//        string username,
//        [FromBody] bool enabled,
//        [FromServices] IUserData data)
//    {
//        try
//        {
//            await data.UpdateFriendsActivityNotifications(enabled, username);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> UpdateStreakReminders(
//        string username,
//        [FromBody] bool enabled,
//        [FromServices] IUserData data)
//    {
//        try
//        {
//            await data.UpdateStreakReminders(enabled, username);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }
//}
