using System.ComponentModel;

namespace ScriptureMemory.Server.DataAccess.Models;

public class Bible
{
    public string? Id { get; set; }
    
    [Key]
    public string Version { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    public string? Copyright { get; set; }
    
    public string Info { get; set; } = string.Empty;
    
    public DateTime? LastSynced { get; set; }
}