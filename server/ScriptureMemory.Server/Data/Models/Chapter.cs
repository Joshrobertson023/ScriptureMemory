namespace ScriptureMemory.Server.DataAccess.Models;

public class Chapter
{
    public string Id { get; set; } = string.Empty;
    
    public string Version { get; set; } = string.Empty;
    
    public string Book { get; set; } = string.Empty;
    
    public int ChapterNum { get; set; }
    
    [Column(TypeName = "jsonb")]
    public object ContentJson { get; set; } = new();
}