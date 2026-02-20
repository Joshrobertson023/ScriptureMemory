//using DataAccess.DataInterfaces;
//using DataAccess.Models;
//using Microsoft.AspNetCore.Mvc;
//using ScriptureMemoryLibrary;
//using VerseAppNew.Server.Services;

//namespace VerseAppNew.Server.Endpoints;

//public static class VerseNoteEndpoint
//{
//    public static void ConfigureVerseNoteEndpoints(this WebApplication app)
//    {
//        app.MapPost("/notes", CreateNote);
//        app.MapPut("/notes/{id}", UpdateNote);
//        app.MapDelete("/notes/{id}", DeleteNote);
//        app.MapGet("/notes/{id}", GetNoteById);
//        app.MapGet("/notes/verse/{verseReference}", GetNotesByVerseReference);
//        app.MapGet("/notes/verse/{verseReference}/public", GetPublicNotesByVerseReference);
//        app.MapGet("/notes/user/{username}", GetNotesByUsername);
//        app.MapGet("/notes/chapter/{book}/{chapter}/private", GetVersesWithPrivateNotes);
//        app.MapGet("/notes/chapter/{book}/{chapter}/public", GetVersesWithPublicNotes);
//        app.MapGet("/notes/chapter/{book}/{chapter}/all/private", GetAllNotesByChapterPrivate);
//        app.MapGet("/notes/chapter/{book}/{chapter}/all/public", GetAllNotesByChapterPublic);
//        app.MapPost("/notes/{id}/like", LikeNote);
//        app.MapDelete("/notes/{id}/like", UnlikeNote);
//    }

//    private static async Task<IResult> CreateNote(
//        [FromBody] CreateNoteRequest request,
//        [FromServices] IVerseNoteData noteData)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(request.OriginalReference ?? request.VerseReference) || 
//                string.IsNullOrWhiteSpace(request.Username) || 
//                string.IsNullOrWhiteSpace(request.Text))
//            {
//                return Results.BadRequest("Verse reference, username, and text are required");
//            }

//            // Use original reference if provided, otherwise use verse reference
//            var originalReference = request.OriginalReference ?? request.VerseReference;
//            var individualVerses = ReferenceParse.GetReferencesFromVersesInReference(originalReference);
//            if (individualVerses == null || individualVerses.Count == 0)
//            {
//                individualVerses = new List<string> { request.VerseReference };
//            }

//            var createdNotes = new List<VerseNote>();
//            foreach (var verseRef in individualVerses)
//            {
//                var note = new VerseNote
//                {
//                    VerseReference = verseRef,
//                    Username = request.Username,
//                    Text = request.Text,
//                    IsPublic = request.IsPublic,
//                    Approved = request.IsPublic ? (bool?)null : false,
//                    OriginalReference = originalReference
//                };

//                var createdNote = await noteData.InsertNote(note);
//                createdNotes.Add(createdNote);
//            }

//            return Results.Ok(createdNotes.FirstOrDefault());
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> UpdateNote(
//        int id,
//        [FromBody] UpdateNoteRequest request,
//        [FromServices] IVerseNoteData noteData)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(request.Text))
//            {
//                return Results.BadRequest("Text is required");
//            }

//            var note = await noteData.GetNoteById(id);
//            if (note == null)
//            {
//                return Results.NotFound("Note not found");
//            }

//            note.Text = request.Text;
//            var updatedNote = await noteData.UpdateNote(note);
//            return Results.Ok(updatedNote);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> DeleteNote(
//        int id,
//        [FromServices] IVerseNoteData noteData)
//    {
//        try
//        {
//            await noteData.DeleteNote(id);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetNoteById(
//        int id,
//        [FromServices] IVerseNoteData noteData)
//    {
//        try
//        {
//            var note = await noteData.GetNoteById(id);
//            if (note == null)
//                return Results.NotFound();
//            return Results.Ok(note);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetNotesByVerseReference(
//        string verseReference,
//        [FromQuery] string? username,
//        [FromQuery] string? currentUsername,
//        [FromServices] IVerseNoteData noteData,
//        [FromServices] INoteLikeData likeData)
//    {
//        try
//        {
//            var notes = await noteData.GetNotesByVerseReference(verseReference, username);
            
//            if (!string.IsNullOrEmpty(currentUsername) && notes.Any())
//            {
//                var noteIds = notes.Select(n => n.Id).ToList();
//                var likeCounts = await likeData.GetLikeCountsForNotes(noteIds);
//                var userLikes = await likeData.GetUserLikesForNotes(noteIds, currentUsername);
                
//                foreach (var note in notes)
//                {
//                    note.LikeCount = likeCounts.GetValueOrDefault(note.Id, 0);
//                    note.UserLiked = userLikes.GetValueOrDefault(note.Id, false);
//                }
//            }
            
//            return Results.Ok(notes);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetPublicNotesByVerseReference(
//        string verseReference,
//        [FromQuery] string? currentUsername,
//        [FromServices] IVerseNoteData noteData,
//        [FromServices] INoteLikeData likeData)
//    {
//        try
//        {
//            var notes = await noteData.GetPublicNotesByVerseReference(verseReference);
            
//            if (notes.Any())
//            {
//                var noteIds = notes.Select(n => n.Id).ToList();
//                var likeCounts = await likeData.GetLikeCountsForNotes(noteIds);
//                var userLikes = !string.IsNullOrEmpty(currentUsername) 
//                    ? await likeData.GetUserLikesForNotes(noteIds, currentUsername)
//                    : new Dictionary<int, bool>();
                
//                foreach (var note in notes)
//                {
//                    note.LikeCount = likeCounts.GetValueOrDefault(note.Id, 0);
//                    note.UserLiked = userLikes.GetValueOrDefault(note.Id, false);
//                }
                
//                // Order by likes (desc) then by created date (desc)
//                notes = notes.OrderByDescending(n => n.LikeCount)
//                            .ThenByDescending(n => n.CreatedDate)
//                            .ToList();
//            }
            
//            return Results.Ok(notes);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetNotesByUsername(
//        string username,
//        [FromServices] IVerseNoteData noteData)
//    {
//        try
//        {
//            var notes = await noteData.GetNotesByUsername(username);
//            return Results.Ok(notes);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetVersesWithPrivateNotes(
//        string book,
//        int chapter,
//        [FromQuery] string username,
//        [FromServices] IVerseNoteData noteData)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(username))
//            {
//                return Results.BadRequest("Username is required");
//            }
            
//            var verseReferences = await noteData.GetVersesWithPrivateNotes(username, book, chapter);
//            return Results.Ok(verseReferences);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetVersesWithPublicNotes(
//        string book,
//        int chapter,
//        [FromServices] IVerseNoteData noteData)
//    {
//        try
//        {
//            var verseReferences = await noteData.GetVersesWithPublicNotes(book, chapter);
//            return Results.Ok(verseReferences);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetAllNotesByChapterPrivate(
//        string book,
//        int chapter,
//        [FromQuery] string username,
//        [FromQuery] string? currentUsername,
//        [FromServices] IVerseNoteData noteData,
//        [FromServices] INoteLikeData likeData)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(username))
//            {
//                return Results.BadRequest("Username is required");
//            }

//            var notes = await noteData.GetAllNotesByChapter(book, chapter, username, isPublic: false);
            
//            // Enrich with like data if current user is provided
//            if (!string.IsNullOrEmpty(currentUsername) && notes.Any())
//            {
//                var noteIds = notes.Select(n => n.Id).ToList();
//                var likeCounts = await likeData.GetLikeCountsForNotes(noteIds);
//                var userLikes = await likeData.GetUserLikesForNotes(noteIds, currentUsername);
                
//                foreach (var note in notes)
//                {
//                    note.LikeCount = likeCounts.GetValueOrDefault(note.Id, 0);
//                    note.UserLiked = userLikes.GetValueOrDefault(note.Id, false);
//                }
//            }
            
//            // Deduplicate by verseReference (notes with same reference should only appear once)
//            var seenReferences = new HashSet<string>();
//            var deduplicatedNotes = new List<VerseNote>();
//            foreach (var note in notes)
//            {
//                var noteRef = note.VerseReference ?? "";
//                if (!seenReferences.Contains(noteRef))
//                {
//                    seenReferences.Add(noteRef);
//                    deduplicatedNotes.Add(note);
//                }
//            }
            
//            return Results.Ok(deduplicatedNotes);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetAllNotesByChapterPublic(
//        string book,
//        int chapter,
//        [FromQuery] string? currentUsername,
//        [FromServices] IVerseNoteData noteData,
//        [FromServices] INoteLikeData likeData)
//    {
//        try
//        {
//            var notes = await noteData.GetAllNotesByChapter(book, chapter, null, isPublic: true);
            
//            // Enrich with like data if current user is provided
//            if (notes.Any())
//            {
//                var noteIds = notes.Select(n => n.Id).ToList();
//                var likeCounts = await likeData.GetLikeCountsForNotes(noteIds);
//                var userLikes = !string.IsNullOrEmpty(currentUsername) 
//                    ? await likeData.GetUserLikesForNotes(noteIds, currentUsername)
//                    : new Dictionary<int, bool>();
                
//                foreach (var note in notes)
//                {
//                    note.LikeCount = likeCounts.GetValueOrDefault(note.Id, 0);
//                    note.UserLiked = userLikes.GetValueOrDefault(note.Id, false);
//                }
                
//                // Order by likes (desc) then by created date (desc)
//                notes = notes.OrderByDescending(n => n.LikeCount)
//                            .ThenByDescending(n => n.CreatedDate)
//                            .ToList();
//            }
            
//            // Deduplicate by verseReference (notes with same reference should only appear once)
//            var seenReferences = new HashSet<string>();
//            var deduplicatedNotes = new List<VerseNote>();
//            foreach (var note in notes)
//            {
//                var noteRef = note.VerseReference ?? "";
//                if (!seenReferences.Contains(noteRef))
//                {
//                    seenReferences.Add(noteRef);
//                    deduplicatedNotes.Add(note);
//                }
//            }
            
//            return Results.Ok(deduplicatedNotes);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> LikeNote(
//        int id,
//        [FromQuery] string username,
//        [FromServices] INoteLikeData likeData,
//        [FromServices] IVerseNoteData noteData,
//        [FromServices] IUserData userData,
//        [FromServices] INotificationDispatcher notificationDispatcher)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(username))
//            {
//                return Results.BadRequest("Username is required");
//            }

//            var note = await noteData.GetNoteById(id);
//            if (note == null)
//            {
//                return Results.NotFound("Note not found");
//            }

//            // Check if user already liked this note
//            var alreadyLiked = await likeData.HasUserLikedNote(id, username);
//            if (alreadyLiked)
//            {
//                return Results.Ok(new { message = "Note already liked" });
//            }

//            // Like the note
//            await likeData.LikeNote(id, username);

//            // Send notification to note author if it's a public note and user is not the author
//            if (note.IsPublic && note.Username != username)
//            {
//                var noteAuthor = await userData.GetUser(note.Username);
//                if (noteAuthor != null && noteAuthor.NotifyNoteLiked == true)
//                {
//                    var liker = await userData.GetUser(username);
//                    if (liker != null)
//                    {
//                        // Build liker name with first name, last name, and username
//                        var likerNameParts = new List<string>();
//                        if (!string.IsNullOrWhiteSpace(liker.FirstName))
//                            likerNameParts.Add(liker.FirstName);
//                        if (!string.IsNullOrWhiteSpace(liker.LastName))
//                            likerNameParts.Add(liker.LastName);
//                        if (likerNameParts.Count == 0)
//                            likerNameParts.Add(username); // Fallback to username if no name
//                        likerNameParts.Add($"(@{username})"); // Always include username
                        
//                        var likerName = string.Join(" ", likerNameParts);
                        
//                        var notification = new Notification
//                        {
//                            Username = note.Username,
//                            SenderUsername = username,
//                            Message = $"{likerName} liked your note on \"{note.VerseReference}\"",
//                            CreatedDate = DateTime.UtcNow,
//                            ExpirationDate = DateTime.UtcNow.AddDays(7),
//                            IsRead = false,
//                            NotificationType = "NOTE_LIKED"
//                        };

//                        await notificationDispatcher.SendNotificationsAsync(new[] { notification });
//                    }
//                }
//            }

//            var likeCount = await likeData.GetNoteLikeCount(id);
//            return Results.Ok(new { likeCount, liked = true });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> UnlikeNote(
//        int id,
//        [FromQuery] string username,
//        [FromServices] INoteLikeData likeData)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(username))
//            {
//                return Results.BadRequest("Username is required");
//            }

//            await likeData.UnlikeNote(id, username);
//            var likeCount = await likeData.GetNoteLikeCount(id);
//            return Results.Ok(new { likeCount, liked = false });
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private record CreateNoteRequest(string VerseReference, string Username, string Text, bool IsPublic, string? OriginalReference);
//    private record UpdateNoteRequest(string Text);
//}

