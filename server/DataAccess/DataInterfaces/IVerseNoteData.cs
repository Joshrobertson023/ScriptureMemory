using DataAccess.Models;

namespace DataAccess.DataInterfaces;

public interface IVerseNoteData
{
    Task<VerseNote> InsertNote(VerseNote note);
    Task<VerseNote> UpdateNote(VerseNote note);
    Task DeleteNote(int id);
    Task<VerseNote?> GetNoteById(int id);
    Task<List<VerseNote>> GetNotesByVerseReference(string verseReference, string? username = null);
    Task<List<VerseNote>> GetPublicNotesByVerseReference(string verseReference);
    Task<List<VerseNote>> GetNotesByUsername(string username);
    Task<List<VerseNote>> GetUnapprovedNotes();
    Task UpdateNoteApproval(int id, bool approved);
    Task<List<string>> GetVersesWithPrivateNotes(string username, string book, int chapter);
    Task<List<string>> GetVersesWithPublicNotes(string book, int chapter);
    Task<List<VerseNote>> GetAllNotesByChapter(string book, int chapter, string? username = null, bool isPublic = true);
}

