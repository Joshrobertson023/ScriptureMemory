using DataAccess.Models;

namespace DataAccess.DataInterfaces;

public interface IHighlightData
{
    Task InsertHighlight(Highlight highlight);
    Task DeleteHighlight(string username, string verseReference);
    Task<List<Highlight>> GetHighlightsByUsername(string username);
    Task<List<Highlight>> GetHighlightsByChapter(string username, string book, int chapter);
    Task<bool> IsHighlighted(string username, string verseReference);
}

