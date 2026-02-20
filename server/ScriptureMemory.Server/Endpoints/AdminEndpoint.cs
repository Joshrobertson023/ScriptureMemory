//using System;
//using System.Linq;
//using DataAccess.DataInterfaces;
//using DataAccess.Models;
//using Microsoft.AspNetCore.Mvc;
//using VerseAppNew.Server.Services;

//namespace VerseAppNew.Server.Endpoints;

//public static class AdminEndpoint
//{
//    public static void ConfigureAdminEndpoints(this WebApplication app)
//    {
//        app.MapGet("/admin/users", GetAllUsers);
//        app.MapGet("/admin/notifications", GetAllNotifications);
//        app.MapGet("/admin/usernames", GetAdminUsernames);
//        app.MapGet("/admin/admins", GetAdmins);
//        app.MapPost("/admin/notifications/send", SendNotificationToAllUsers);
//        app.MapPost("/admin/verseofday", CreateVerseOfDay);
//        app.MapGet("/admin/verseofday/upcoming", GetUpcomingVerseOfDay);
//        app.MapDelete("/admin/verseofday/{id}", DeleteVerseOfDay);
//        app.MapGet("/admin/users/{username}/check", CheckIfAdmin);
//        app.MapPut("/admin/users/{username}/make-admin", MakeUserAdmin);
//        app.MapPut("/admin/users/{username}/remove-admin", RemoveUserAdmin);
//        app.MapGet("/banner", GetBanner);
//        app.MapGet("/admin/banner", GetBanner);
//        app.MapPut("/admin/banner", UpdateBanner);
//        app.MapDelete("/admin/banner", DeleteBanner);
//        app.MapGet("/admin/notes/unapproved", GetUnapprovedNotes);
//        app.MapPut("/admin/notes/{id}/approve", ApproveNote);
//        app.MapPut("/admin/notes/{id}/deny", DenyNote);
//        app.MapGet("/admin/verseofday/suggestions", GetVerseOfDaySuggestions);
//        app.MapPost("/admin/verseofday/suggestions/{id}/approve", ApproveVerseOfDaySuggestion);
//        app.MapDelete("/admin/verseofday/suggestions/{id}", DeleteVerseOfDaySuggestion);
//        app.MapGet("/admin/collections/pending", GetPendingCollections);
//        app.MapPut("/admin/collections/{id}/approve", ApproveCollection);
//        app.MapPut("/admin/collections/{id}/reject", RejectCollection);
//        app.MapPut("/admin/users/{username}/make-paid", MakeUserPaid);
//        app.MapPut("/admin/users/{username}/remove-paid", RemoveUserPaid);
//        app.MapGet("/admin/users/paid", GetPaidUsers);
//    }

//    private static async Task<IResult> GetAllUsers(
//        string? search,
//        [FromServices] IUserData userData)
//    {
//        try
//        {
//            var users = await userData.GetAllUsers();
            
//            if (!string.IsNullOrEmpty(search))
//            {
//                users = users.Where(u => 
//                    u.Username.Contains(search, StringComparison.OrdinalIgnoreCase) ||
//                    u.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
//                    u.LastName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
//                    u.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
//                    $"{u.FirstName} {u.LastName}".Trim().Contains(search, StringComparison.OrdinalIgnoreCase)
//                );
//            }
            
//            return Results.Ok(users);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetAllNotifications(
//        [FromServices] INotificationData notificationData)
//    {
//        try
//        {
//            var notifications = await notificationData.GetAllNotifications();
//            return Results.Ok(notifications);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetAdminUsernames(
//        [FromServices] IAdminData adminData)
//    {
//        try
//        {
//            var usernames = await adminData.GetAdminUsernames();
//            return Results.Ok(usernames);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetAdmins(
//        [FromServices] IAdminData adminData)
//    {
//        try
//        {
//            var admins = await adminData.GetAdminsWithDetails();
//            return Results.Ok(admins);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> SendNotificationToAllUsers(
//        [FromBody] SendNotificationRequest request,
//        [FromServices] IUserData userData,
//        [FromServices] IAdminData adminData,
//        [FromServices] INotificationDispatcher notificationDispatcher)
//    {
//        try
//        {
            
//            var sender = await userData.GetUser(request.SenderUsername);
//            if (sender == null || !await adminData.IsAdmin(sender.Username))
//            {
//                return Results.Unauthorized();
//            }

//            var senderUsername = string.IsNullOrWhiteSpace(request.SenderUsername) ? "SYSTEM" : request.SenderUsername;
//            await notificationDispatcher.BroadcastToAllUsersAsync(request.Message, senderUsername);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> CreateVerseOfDay(
//        [FromBody] CreateVerseOfDayRequest request,
//        [FromServices] IVerseOfDayData verseOfDayData,
//        [FromServices] IUserData userData,
//        [FromServices] DataAccess.DataInterfaces.IVerseData verseData,
//        [FromServices] IAdminData adminData)
//    {
//        try
//        {
//            var sender = await userData.GetUser(request.SenderUsername);
//            if (sender == null || !await adminData.IsAdmin(sender.Username))
//            {
//                return Results.Unauthorized();
//            }

//            var trimmedReference = request.ReadableReference?.Trim();
//            if (string.IsNullOrWhiteSpace(trimmedReference))
//            {
//                return Results.BadRequest(new { message = "Verse reference is required." });
//            }

//            // Use GetVerseSearchResults to parse and normalize the reference (e.g., "Psalm 119 2" -> "Psalms 119:2")
//            var searchResults = await verseData.GetVerseSearchResults(trimmedReference);

//            if (searchResults == null || searchResults.Verses == null || !searchResults.Verses.Any())
//            {
//                return Results.BadRequest(new { message = $"Verse reference '{trimmedReference}' not found in database. Please check the spelling and format (e.g., 'John 3:16')." });
//            }

//            if (searchResults.Verses.Any(v => (v.Verse_Number == "0" && v.Text.Contains("Error")) || v.Text.Contains("Invalid")))
//            {
//                return Results.BadRequest(new { message = $"Invalid verse reference: '{trimmedReference}'. Please check the format (e.g., 'John 3:16')." });
//            }

//            // Use the readable reference from search results, which will be properly normalized
//            // If it's a passage search, use the readable_Reference from SearchData
//            // Otherwise, construct from the first verse's reference
//            string normalizedReference;
//            if (!string.IsNullOrWhiteSpace(searchResults.Readable_Reference))
//            {
//                normalizedReference = searchResults.Readable_Reference;
//            }
//            else if (searchResults.Verses.Count == 1)
//            {
//                normalizedReference = searchResults.Verses[0].verse_reference ?? trimmedReference;
//            }
//            else
//            {
//                // For multiple verses, construct a readable reference from the first and last verse
//                var firstVerse = searchResults.Verses.First();
//                var lastVerse = searchResults.Verses.Last();
//                // Extract book and chapter from first verse reference (format: "Book Chapter:Verse")
//                var firstRefParts = firstVerse.verse_reference?.Split(':');
//                if (firstRefParts != null && firstRefParts.Length >= 2)
//                {
//                    var bookChapter = firstRefParts[0].Trim();
//                    var firstVerseNum = firstRefParts[1].Trim();
//                    var lastVerseNum = lastVerse.verse_reference?.Split(':').LastOrDefault()?.Trim();
//                    if (lastVerseNum != null && firstVerseNum != lastVerseNum)
//                    {
//                        normalizedReference = $"{bookChapter}:{firstVerseNum}-{lastVerseNum}";
//                    }
//                    else
//                    {
//                        normalizedReference = firstVerse.verse_reference ?? trimmedReference;
//                    }
//                }
//                else
//                {
//                    normalizedReference = firstVerse.verse_reference ?? trimmedReference;
//                }
//            }

//            var verseOfDay = new VerseOfDay
//            {
//                ReadableReference = normalizedReference
//            };

//            await verseOfDayData.CreateVerseOfDay(verseOfDay);
//            return Results.Ok(new { message = $"Verse '{normalizedReference}' added to queue successfully" });
//        }
//        catch (Exception ex)
//        {
//            return Results.BadRequest(new { message = ex.Message });
//        }
//    }

//    private static async Task<IResult> GetUpcomingVerseOfDay(
//        [FromServices] IVerseOfDayData verseOfDayData)
//    {
//        try
//        {
//            var verses = await verseOfDayData.GetUpcomingVerseOfDay();
//            var info = await verseOfDayData.GetLastUsedVerseOfDayData();
//            int currentVerseId = info?.LastUsedVodId ?? 0;
            
//            var result = verses.Select(v => new
//            {
//                v.Id,
//                v.ReadableReference,
//                v.Sequence,
//                IsCurrent = v.Id == currentVerseId
//            });
            
//            return Results.Ok(result);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> DeleteVerseOfDay(
//        int id,
//        [FromServices] IVerseOfDayData verseOfDayData,
//        [FromServices] IUserData userData,
//        [FromQuery] string username,
//        [FromServices] IAdminData adminData)
//    {
//        try
//        {
            
//            var sender = await userData.GetUser(username);
//            if (sender == null || !await adminData.IsAdmin(sender.Username))
//            {
//                return Results.Unauthorized();
//            }

//            await verseOfDayData.DeleteVerseOfDay(id);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetBanner(
//        [FromServices] IBannerData bannerData)
//    {
//        try
//        {
//            var banner = await bannerData.GetBanner();
//            if (banner is null)
//            {
//                return Results.Ok(new BannerResponse
//                {
//                    HasBanner = false,
//                    Message = null
//                });
//            }

//            return Results.Ok(new BannerResponse
//            {
//                HasBanner = true,
//                Message = banner.Message ?? string.Empty
//            });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> UpdateBanner(
//        [FromBody] UpdateBannerRequest request,
//        [FromServices] IBannerData bannerData,
//        [FromServices] IUserData userData,
//        [FromServices] IAdminData adminData)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(request.Username))
//            {
//                return Results.BadRequest("Username is required.");
//            }

//            if (!string.Equals(request.Pin, OwnerPin, StringComparison.Ordinal))
//            {
//                return Results.Unauthorized();
//            }

//            var sender = await userData.GetUser(request.Username);
//            if (sender == null || !await adminData.IsAdmin(sender.Username))
//            {
//                return Results.Unauthorized();
//            }

//            var normalizedMessage = (request.Message ?? string.Empty).Trim();
//            if (string.IsNullOrWhiteSpace(normalizedMessage))
//            {
//                return Results.BadRequest("Banner message cannot be empty.");
//            }

//            var banner = new SiteBanner
//            {
//                BannerId = 1,
//                Message = normalizedMessage
//            };

//            await bannerData.SetBanner(banner);

//            return Results.Ok(new BannerResponse
//            {
//                HasBanner = true,
//                Message = normalizedMessage
//            });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> DeleteBanner(
//        [FromBody] DeleteBannerRequest request,
//        [FromServices] IBannerData bannerData,
//        [FromServices] IUserData userData,
//        [FromServices] IAdminData adminData)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(request.Username))
//            {
//                return Results.BadRequest("Username is required.");
//            }

//            if (!string.Equals(request.Pin, OwnerPin, StringComparison.Ordinal))
//            {
//                return Results.Unauthorized();
//            }

//            var sender = await userData.GetUser(request.Username);
//            if (sender == null || !await adminData.IsAdmin(sender.Username))
//            {
//                return Results.Unauthorized();
//            }

//            await bannerData.RemoveBanner();

//            return Results.Ok(new BannerResponse
//            {
//                HasBanner = false,
//                Message = null
//            });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }
//    private const string OwnerPin = "3151";

//    private static async Task<IResult> CheckIfAdmin(
//        string username,
//        [FromServices] IAdminData adminData)
//    {
//        try
//        {
//            bool isAdmin = await adminData.IsAdmin(username);
//            return Results.Ok(new { isAdmin });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> MakeUserAdmin(
//        string username,
//        [FromServices] IUserData userData,
//        [FromServices] IAdminData adminData)
//    {
//        try
//        {
//            var user = await userData.GetUser(username);
//            if (user == null)
//            {
//                return Results.NotFound(new { message = "User not found" });
//            }

//            if (await adminData.IsAdmin(user.Username))
//            {
//                return Results.Ok(new { message = "User is already an admin" });
//            }

//            await adminData.AddAdmin(user.Username);

//            return Results.Ok(new { message = "User is now an admin" });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> RemoveUserAdmin(
//        string username,
//        [FromServices] IUserData userData,
//        [FromServices] IAdminData adminData)
//    {
//        try
//        {
//            var user = await userData.GetUser(username);
//            if (user == null)
//            {
//                return Results.NotFound(new { message = "User not found" });
//            }

//            var existing = await adminData.IsAdmin(username);
//            if (!existing)
//            {
//                return Results.NotFound(new { message = "User is not an admin" });
//            }

//            await adminData.RemoveAdmin(username);

//            return Results.Ok(new { message = "User is no longer an admin" });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetUnapprovedNotes(
//        [FromQuery] string username,
//        [FromServices] IVerseNoteData noteData,
//        [FromServices] IUserData userData,
//        [FromServices] IAdminData adminData)
//    {
//        try
//        {
//            var sender = await userData.GetUser(username);
//            if (sender == null || !await adminData.IsAdmin(sender.Username))
//            {
//                return Results.Unauthorized();
//            }

//            var notes = await noteData.GetUnapprovedNotes();
//            return Results.Ok(notes);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> ApproveNote(
//        int id,
//        [FromQuery] string username,
//        [FromServices] IVerseNoteData noteData,
//        [FromServices] IUserData userData,
//        [FromServices] IAdminData adminData)
//    {
//        try
//        {
//            var sender = await userData.GetUser(username);
//            if (sender == null || !await adminData.IsAdmin(sender.Username))
//            {
//                return Results.Unauthorized();
//            }

//            var note = await noteData.GetNoteById(id);
//            if (note == null)
//            {
//                return Results.NotFound();
//            }

//            await noteData.UpdateNoteApproval(id, true);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> DenyNote(
//        int id,
//        [FromBody] DenyNoteRequest request,
//        [FromQuery] string username,
//        [FromServices] IVerseNoteData noteData,
//        [FromServices] IUserData userData,
//        [FromServices] IAdminData adminData,
//        [FromServices] IEmailService emailService)
//    {
//        try
//        {
//            var sender = await userData.GetUser(username);
//            if (sender == null || !await adminData.IsAdmin(sender.Username))
//            {
//                return Results.Unauthorized();
//            }

//            var note = await noteData.GetNoteById(id);
//            if (note == null)
//            {
//                return Results.NotFound();
//            }

//            // Get user email
//            var noteUser = await userData.GetUser(note.Username);
//            if (noteUser != null && !string.IsNullOrWhiteSpace(noteUser.Email))
//            {
//                var reason = string.IsNullOrWhiteSpace(request.Reason) 
//                    ? "Your note did not meet our community guidelines." 
//                    : request.Reason;

//                var emailBody = $@"Hi {note.Username},

//We wanted to let you know that your public note on '{note.VerseReference}' has been removed from VerseApp.

//Reason: {reason}

//If you have any questions or concerns, please feel free to reach out to us.

//Sincerely,
//The VerseApp Team";

//                try
//                {
//                    await emailService.SendEmailAsync(noteUser.Email, "Your VerseApp note was removed", emailBody);
//                }
//                catch (Exception emailEx)
//                {
//                    // Log email error but continue with deletion
//                    Console.WriteLine($"Failed to send denial email: {emailEx.Message}");
//                }
//            }

//            // Delete the note
//            await noteData.DeleteNote(id);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetVerseOfDaySuggestions(
//        [FromQuery] string username,
//        [FromServices] IVerseOfDaySuggestionData suggestionData,
//        [FromServices] IUserData userData,
//        [FromServices] IAdminData adminData)
//    {
//        try
//        {
//            var sender = await userData.GetUser(username);
//            if (sender == null || !await adminData.IsAdmin(sender.Username))
//            {
//                return Results.Unauthorized();
//            }

//            var suggestions = await suggestionData.GetAllSuggestions();
//            return Results.Ok(suggestions);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> ApproveVerseOfDaySuggestion(
//        int id,
//        [FromQuery] string username,
//        [FromServices] IVerseOfDaySuggestionData suggestionData,
//        [FromServices] IVerseOfDayData verseOfDayData,
//        [FromServices] DataAccess.DataInterfaces.IVerseData verseData,
//        [FromServices] IUserData userData,
//        [FromServices] IAdminData adminData)
//    {
//        try
//        {
//            var sender = await userData.GetUser(username);
//            if (sender == null || !await adminData.IsAdmin(sender.Username))
//            {
//                return Results.Unauthorized();
//            }

//            var suggestion = await suggestionData.GetSuggestion(id);
//            if (suggestion == null)
//            {
//                return Results.NotFound("Suggestion not found");
//            }

//            // Validate the verse reference
//            var trimmedReference = suggestion.ReadableReference?.Trim();
//            if (string.IsNullOrWhiteSpace(trimmedReference))
//            {
//                return Results.BadRequest(new { message = "Verse reference is required." });
//            }

//            var searchResults = await verseData.GetVerseSearchResults(trimmedReference);
//            if (searchResults == null || searchResults.Verses == null || !searchResults.Verses.Any())
//            {
//                return Results.BadRequest(new { message = $"Verse reference '{trimmedReference}' not found in database." });
//            }

//            // Add to verse of day queue
//            var verseOfDay = new VerseOfDay
//            {
//                ReadableReference = trimmedReference
//            };
//            await verseOfDayData.CreateVerseOfDay(verseOfDay);

//            // Delete the suggestion after adding to queue
//            await suggestionData.DeleteSuggestion(id);

//            return Results.Ok(new { message = $"Verse '{trimmedReference}' added to queue successfully" });
//        }
//        catch (Exception ex)
//        {
//            return Results.BadRequest(new { message = ex.Message });
//        }
//    }

//    private static async Task<IResult> DeleteVerseOfDaySuggestion(
//        int id,
//        [FromQuery] string username,
//        [FromServices] IVerseOfDaySuggestionData suggestionData,
//        [FromServices] IUserData userData,
//        [FromServices] IAdminData adminData)
//    {
//        try
//        {
//            var sender = await userData.GetUser(username);
//            if (sender == null || !await adminData.IsAdmin(sender.Username))
//            {
//                return Results.Unauthorized();
//            }

//            await suggestionData.DeleteSuggestion(id);
//            return Results.Ok(new { message = "Suggestion deleted successfully" });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetPendingCollections(
//        [FromQuery] string username,
//        [FromServices] ICollectionData collectionData,
//        [FromServices] IUserData userData,
//        [FromServices] IAdminData adminData)
//    {
//        try
//        {
//            var sender = await userData.GetUser(username);
//            if (sender == null || !await adminData.IsAdmin(sender.Username))
//            {
//                return Results.Unauthorized();
//            }

//            var pendingCollections = await collectionData.GetPendingCollections();
//            return Results.Ok(pendingCollections);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> ApproveCollection(
//        int id,
//        [FromQuery] string username,
//        [FromServices] ICollectionData collectionData,
//        [FromServices] IUserData userData,
//        [FromServices] IAdminData adminData,
//        [FromServices] INotificationDispatcher notificationDispatcher,
//        [FromServices] IRelationshipData relationshipData,
//        [FromServices] IActivityData activityData)
//    {
//        try
//        {
//            var sender = await userData.GetUser(username);
//            if (sender == null || !await adminData.IsAdmin(sender.Username))
//            {
//                return Results.Unauthorized();
//            }

//            var collection = await collectionData.GetPublishedCollectionById(id);
//            if (collection == null)
//            {
//                return Results.NotFound("Collection not found");
//            }

//            if (collection.Status != "PENDING")
//            {
//                return Results.BadRequest("Collection is not pending review");
//            }

//            await collectionData.ApprovePublishedCollection(id);

//            // Notify the author that their collection was approved
//            var author = await userData.GetUser(collection.Author);
//            if (author != null)
//            {
//                var notification = new Notification
//                {
//                    Username = collection.Author,
//                    SenderUsername = "SYSTEM",
//                    Message = $"Your collection \"{collection.Title}\" has been approved and published!",
//                    CreatedDate = DateTime.UtcNow,
//                    ExpirationDate = DateTime.UtcNow.AddDays(7),
//                    IsRead = false,
//                    NotificationType = "COLLECTION_APPROVED"
//                };

//                await notificationDispatcher.SendNotificationAsync(notification);

//                // Create activity and notify friends if user wants to notify about published collections
//                if (author.NotifyPublishedCollection == true)
//                {
//                    var activity = new DataAccess.Models.Activity
//                    {
//                        Text = $"You published the collection \"{collection.Title}\"",
//                        Username = collection.Author,
//                        DateCreated = DateTime.UtcNow
//                    };
//                    await activityData.CreateActivity(activity);

//                    var friends = await relationshipData.GetFriends(collection.Author);
                    
//                    // Filter friends to only those who have activity notifications enabled
//                    var friendNotifications = new List<Notification>();
//                    foreach (var friend in friends)
//                    {
//                        var friendUser = await userData.GetUser(friend.Username);
//                        if (friendUser != null && friendUser.ActivityNotificationsEnabled == true)
//                        {
//                            friendNotifications.Add(new Notification
//                            {
//                                Username = friend.Username,
//                                SenderUsername = author.Username,
//                                Message = $"{author.FirstName} {author.LastName} published the collection \"{collection.Title}\"!",
//                                CreatedDate = DateTime.UtcNow,
//                                IsRead = false,
//                                NotificationType = "ACTIVITY"
//                            });
//                        }
//                    }

//                    if (friendNotifications.Count > 0)
//                    {
//                        await notificationDispatcher.SendNotificationsAsync(friendNotifications);
//                    }
//                }
//            }

//            return Results.Ok(new { message = "Collection approved successfully" });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> RejectCollection(
//        int id,
//        [FromQuery] string username,
//        [FromBody] RejectCollectionRequest? request,
//        [FromServices] ICollectionData collectionData,
//        [FromServices] IUserData userData,
//        [FromServices] IAdminData adminData,
//        [FromServices] INotificationDispatcher notificationDispatcher)
//    {
//        try
//        {
//            var sender = await userData.GetUser(username);
//            if (sender == null || !await adminData.IsAdmin(sender.Username))
//            {
//                return Results.Unauthorized();
//            }

//            var collection = await collectionData.GetPublishedCollectionById(id);
//            if (collection == null)
//            {
//                return Results.NotFound("Collection not found");
//            }

//            if (collection.Status != "PENDING")
//            {
//                return Results.BadRequest("Collection is not pending review");
//            }

//            await collectionData.RejectPublishedCollection(id);

//            // Notify the author that their collection was rejected
//            var reason = request?.Reason ?? "Your collection did not meet our community guidelines.";
//            var notification = new Notification
//            {
//                Username = collection.Author,
//                SenderUsername = "SYSTEM",
//                Message = $"Your collection \"{collection.Title}\" was not approved. Reason: {reason} -- please fix errors and re-submit.",
//                CreatedDate = DateTime.UtcNow,
//                ExpirationDate = DateTime.UtcNow.AddDays(7),
//                IsRead = false,
//                NotificationType = "COLLECTION_REJECTED"
//            };

//            await notificationDispatcher.SendNotificationAsync(notification);

//            return Results.Ok(new { message = "Collection rejected successfully" });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> MakeUserPaid(
//        string username,
//        [FromBody] MakeUserPaidRequest request,
//        [FromServices] IUserData userData,
//        [FromServices] IAdminData adminData)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(request.AdminUsername))
//            {
//                return Results.BadRequest("Admin username is required.");
//            }

//            if (!string.Equals(request.Pin, OwnerPin, StringComparison.Ordinal))
//            {
//                return Results.Unauthorized();
//            }

//            var admin = await userData.GetUser(request.AdminUsername);
//            if (admin == null || !await adminData.IsAdmin(admin.Username))
//            {
//                return Results.Unauthorized();
//            }

//            var user = await userData.GetUser(username);
//            if (user == null)
//            {
//                return Results.NotFound(new { message = "User not found" });
//            }

//            if (user.IsPaid == true)
//            {
//                return Results.Ok(new { message = "User is already a paid user" });
//            }

//            await userData.UpdateIsPaid(true, username);

//            return Results.Ok(new { message = "User is now a paid user" });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> RemoveUserPaid(
//        string username,
//        [FromBody] MakeUserPaidRequest request,
//        [FromServices] IUserData userData,
//        [FromServices] IAdminData adminData)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(request.AdminUsername))
//            {
//                return Results.BadRequest("Admin username is required.");
//            }

//            if (!string.Equals(request.Pin, OwnerPin, StringComparison.Ordinal))
//            {
//                return Results.Unauthorized();
//            }

//            var admin = await userData.GetUser(request.AdminUsername);
//            if (admin == null || !await adminData.IsAdmin(admin.Username))
//            {
//                return Results.Unauthorized();
//            }

//            var user = await userData.GetUser(username);
//            if (user == null)
//            {
//                return Results.NotFound(new { message = "User not found" });
//            }

//            if (user.IsPaid != true)
//            {
//                return Results.Ok(new { message = "User is not a paid user" });
//            }

//            await userData.UpdateIsPaid(false, username);

//            return Results.Ok(new { message = "Paid status removed from user" });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetPaidUsers(
//        [FromServices] IUserData userData)
//    {
//        try
//        {
//            var paidUsers = await userData.GetPaidUsers();
//            var result = paidUsers.Select(u => new
//            {
//                username = u.Username,
//                email = u.Email
//            });
//            return Results.Ok(result);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }
//}

//public class SendNotificationRequest
//{
//    public string Message { get; set; }
//    public string SenderUsername { get; set; }
//}

//public class CreateVerseOfDayRequest
//{
//    public string ReadableReference { get; set; } = string.Empty;
//    public string SenderUsername { get; set; } = string.Empty;
//}

//public class ResetVerseOfDayQueueRequest
//{
//    public string SenderUsername { get; set; } = string.Empty;
//}

//public class BannerResponse
//{
//    public bool HasBanner { get; set; }
//    public string? Message { get; set; }
//}

//public class UpdateBannerRequest
//{
//    public string? Message { get; set; }
//    public string Username { get; set; } = string.Empty;
//    public string Pin { get; set; } = string.Empty;
//}

//public class DeleteBannerRequest
//{
//    public string Username { get; set; } = string.Empty;
//    public string Pin { get; set; } = string.Empty;
//}

//public class DenyNoteRequest
//{
//    public string? Reason { get; set; }
//}

//public class RejectCollectionRequest
//{
//    public string? Reason { get; set; }
//}

//public class MakeUserPaidRequest
//{
//    public string AdminUsername { get; set; } = string.Empty;
//    public string Pin { get; set; } = string.Empty;
//}
