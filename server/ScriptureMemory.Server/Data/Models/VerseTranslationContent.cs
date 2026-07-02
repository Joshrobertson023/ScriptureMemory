using DataAccess.Models;
using Pgvector;
using System.ComponentModel;

namespace ScriptureMemory.Server.DataAccess.Models;

public class VerseTranslationContent
{
    public string Version { get; set; } = string.Empty;
    
    public string PlainText { get; set; } = string.Empty;
    
    [Column(TypeName = "text")]
    public string ContentUsx { get; set; } = string.Empty; // USX format (Unified Scripture XML)
    
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