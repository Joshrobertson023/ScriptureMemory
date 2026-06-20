using DataAccess.Models;
using Pgvector;
using System.ComponentModel;

namespace ScriptureMemory.Server.DataAccess.Models;

public class VerseContent
{
    [MaxLength(5)]
    public string Version { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string PlainText { get; set; } = string.Empty;
    
    [Column(TypeName = "jsonb")]
    public object? ContentJson { get; set; }
    
    public Vector? Embedding { get; set; }
    
    public DateTime? LastUpdated { get; set; }

    [MaxLength(20)]
    public string VerseId { get; set; } = string.Empty;
    
    public Verse VerseNavigation { get; set; } = null!;

    public string? GetEmbeddingText()
    {
        if (string.IsNullOrEmpty(PlainText))
            return null;
        
        return VerseNavigation.Reference.Book  
               + " "                  
               + VerseNavigation.Reference.Chapter 
               + " " 
               + VerseNavigation.Reference.VerseNumbers.FirstOrDefault()
               + ": " 
               + PlainText;
    }
}