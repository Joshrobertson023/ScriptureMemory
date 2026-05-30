using System.ComponentModel;

namespace ScriptureMemory.Server.DataAccess.Models;

public class Bible
{
    [MaxLength(100)]
    public string Id { get; set; } = string.Empty;
    
    [Key]
    [MaxLength(5)]
    public string Version { get; set; } = string.Empty;
    
    [MaxLength(30)]
    public string VersionFull { get; set; } = string.Empty;
    
    public string Copyright { get; set; } = string.Empty;
    
    public string Source { get; set; } = string.Empty;
    
    [DefaultValue("CURRENT_DATE")]
    public DateTime LastSynced { get; set; }
}