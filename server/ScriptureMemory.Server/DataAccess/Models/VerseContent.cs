using DataAccess.Models;
using Pgvector;

namespace ScriptureMemory.Server.DataAccess.Models;

public class VerseContent : Verse
{
    [MaxLength(5)]
    public string Version { get; set; } = string.Empty;
    public string PlainText { get; set; } = string.Empty;
    public object? ContentJson { get; set; }
    public Vector? Embedding { get; set; }
    public DateTime LastUpdated { get; set; }

    public string? GetEmbeddingText()
    {
        if (string.IsNullOrEmpty(PlainText) 
            || string.IsNullOrEmpty(Book + Chapter + VerseNum))
            return null;
        
        return Book + " " + Chapter + " " + VerseNum + ": " + PlainText;
    }
}