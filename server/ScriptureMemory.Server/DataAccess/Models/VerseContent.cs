using DataAccess.Models;
using Pgvector;
using System.ComponentModel;

namespace ScriptureMemory.Server.DataAccess.Models;

public class VerseContent
{
    [MaxLength(5)]
    public string Version { get; set; } = string.Empty;
    
    [MaxLength(700)]
    public string PlainText { get; set; } = string.Empty;
    
    public object? ContentJson { get; set; }
    
    public Vector? Embedding { get; set; }
    
    [DefaultValue("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")]
    public DateTime LastUpdated { get; set; }

    public Verse VerseNavigation { get; set; } = null!;

    public string? GetEmbeddingText()
    {
        if (string.IsNullOrEmpty(PlainText) 
            || string.IsNullOrEmpty(Verse.Reference.Book + Chapter + VerseNum))
            return null;
        
        return Book + " " + Chapter + " " + VerseNum + ": " + PlainText;
    }
}