using System.ComponentModel;

namespace ScriptureMemory.Server.DataAccess.Models;

public class Bible
{
    [MaxLength(100)]
    public string? Id { get; set; }
    
    [Key]
    [MaxLength(5)]
    public string Version { get; set; } = string.Empty;
    
    [MaxLength(30)]
    public string VersionFull { get; set; } = string.Empty;
    
    [MaxLength(300)]
    public string? Copyright { get; set; }
    
    [MaxLength(300)]
    public string Source { get; set; } = string.Empty;
    
    public DateTime? LastSynced { get; set; }
}