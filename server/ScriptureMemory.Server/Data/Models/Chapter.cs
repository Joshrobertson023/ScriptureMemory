namespace ScriptureMemory.Server.DataAccess.Models;

public class Chapter
{
    [Key]
    [MaxLength(50)]
    public string Id { get; set; } = string.Empty;
    
    [MaxLength(5)]
    public string Version { get; set; } = string.Empty;
    
    [MaxLength(30)] 
    public string Book { get; set; } = string.Empty;
    
    [Column("Chapter")]
    public int ChapterNum { get; set; }
    
    public object ContentJson { get; set; } = new();
}