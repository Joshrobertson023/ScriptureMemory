using ScriptureMemory.Server.Data.Models;

namespace ScriptureMemory.Server.DataAccess.Models;

public class Chapter
{
    public Reference Reference { get; set; }
    
    public string Id { get; set; } = string.Empty;
    
    public string Version { get; set; } = string.Empty;
    
    public Book Book { get; set; }
    
    public int ChapterNum { get; set; }
    
    [Column(TypeName = "text")]
    public string ContentUsx { get; set; } = string.Empty; // Unified Scripture XML
    
    public DateTime? LastUpdated { get; set; }

    public string GetId()
    {
        return Book.Abbreviation + '.' + ChapterNum;
    }
}