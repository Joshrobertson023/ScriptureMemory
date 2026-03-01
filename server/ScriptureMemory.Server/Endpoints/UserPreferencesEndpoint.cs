using DataAccess.DataInterfaces;
using Microsoft.AspNetCore.Mvc;
using static ScriptureMemoryLibrary.Enums;
using VerseAppNew.Server.Requests;

namespace VerseAppNew.Server.Endpoints;

public static class UserPreferencesEndpoint
{
    public static void ConfigureUserPreferencesEndpoints(this WebApplication app)
    {
        // Update Bible Version
        app.MapPut("/userpreferences/bibleversion", async (
            [FromBody] UpdateBibleVersionRequest req,
            [FromServices] IUserSettingsData settingsData) =>
        {
            // Assume UpdateBibleVersion is implemented in IUserSettingsData
            await settingsData.UpdateBibleVersion(req.BibleVersion, req.UserId);
            return Results.Ok();
        });

        // Update Type Out Reference
        app.MapPut("/userpreferences/typeoutreference", async (
            [FromBody] UpdateBoolPreferenceRequest req,
            [FromServices] IUserSettingsData settingsData) =>
        {
            await settingsData.UpdateTypeOutReference(req.Enabled, req.UserId);
            return Results.Ok();
        });

        // Update Theme Preference
        app.MapPut("/userpreferences/themepreference", async (
            [FromBody] UpdateThemePreferenceRequest req,
            [FromServices] IUserSettingsData settingsData) =>
        {
            await settingsData.UpdateThemePreference(req.Preference, req.UserId);
            return Results.Ok();
        });

        // Update Collections Sort
        app.MapPut("/userpreferences/collectionssort", async (
            [FromBody] UpdateCollectionsSortRequest req,
            [FromServices] IUserSettingsData settingsData) =>
        {
            await settingsData.UpdateCollectionsSort(req.SortBy, req.UserId);
            return Results.Ok();
        });

        // Update Subscribed Verse Of Day
        app.MapPut("/userpreferences/subscribedverseofday", async (
            [FromBody] UpdateBoolPreferenceRequest req,
            [FromServices] IUserSettingsData settingsData) =>
        {
            await settingsData.UpdateSubscribedVerseOfDay(req.Enabled, req.UserId);
            return Results.Ok();
        });

        // Update Push Notifications
        app.MapPut("/userpreferences/pushnotifications", async (
            [FromBody] UpdateBoolPreferenceRequest req,
            [FromServices] IUserSettingsData settingsData) =>
        {
            await settingsData.UpdatePushNotificationsEnabled(req.Enabled, req.UserId);
            return Results.Ok();
        });

        // Update Notify Memorized Verse
        app.MapPut("/userpreferences/notifymemorizedverse", async (
            [FromBody] UpdateBoolPreferenceRequest req,
            [FromServices] IUserSettingsData settingsData) =>
        {
            await settingsData.UpdateNotifyMemorizedVerse(req.Enabled, req.UserId);
            return Results.Ok();
        });

        // Update Notify Published Collection
        app.MapPut("/userpreferences/notifypublishedcollection", async (
            [FromBody] UpdateBoolPreferenceRequest req,
            [FromServices] IUserSettingsData settingsData) =>
        {
            await settingsData.UpdateNotifyPublishedCollection(req.Enabled, req.UserId);
            return Results.Ok();
        });

        // Update Notify Collection Saved
        app.MapPut("/userpreferences/notifycollectionsaved", async (
            [FromBody] UpdateBoolPreferenceRequest req,
            [FromServices] IUserSettingsData settingsData) =>
        {
            await settingsData.UpdateNotifyCollectionSaved(req.Enabled, req.UserId);
            return Results.Ok();
        });

        // Update Notify Note Liked
        app.MapPut("/userpreferences/notifynoteliked", async (
            [FromBody] UpdateBoolPreferenceRequest req,
            [FromServices] IUserSettingsData settingsData) =>
        {
            await settingsData.UpdateNotifyNoteLiked(req.Enabled, req.UserId);
            return Results.Ok();
        });

        // Update Friends Activity Notifications
        app.MapPut("/userpreferences/friendsactivitynotifications", async (
            [FromBody] UpdateBoolPreferenceRequest req,
            [FromServices] IUserSettingsData settingsData) =>
        {
            await settingsData.UpdateFriendsActivityNotifications(req.Enabled, req.UserId);
            return Results.Ok();
        });

        // Update Streak Reminders
        app.MapPut("/userpreferences/streakreminders", async (
            [FromBody] UpdateBoolPreferenceRequest req,
            [FromServices] IUserSettingsData settingsData) =>
        {
            await settingsData.UpdateStreakReminders(req.Enabled, req.UserId);
            return Results.Ok();
        });

        // Update App Badges Enabled
        app.MapPut("/userpreferences/appbadgesenabled", async (
            [FromBody] UpdateBoolPreferenceRequest req,
            [FromServices] IUserSettingsData settingsData) =>
        {
            await settingsData.UpdateAppBadgesEnabled(req.Enabled, req.UserId);
            return Results.Ok();
        });

        // Update Practice Tab Badges Enabled
        app.MapPut("/userpreferences/practicetabbadgesenabled", async (
            [FromBody] UpdateBoolPreferenceRequest req,
            [FromServices] IUserSettingsData settingsData) =>
        {
            await settingsData.UpdatePracticeTabBadgesEnabled(req.Enabled, req.UserId);
            return Results.Ok();
        });
    }
}

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
