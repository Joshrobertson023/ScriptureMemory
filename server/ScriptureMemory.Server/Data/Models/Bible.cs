using System.ComponentModel;

namespace ScriptureMemory.Server.DataAccess.Models;

public class Bible
{
    [Key] 
    public string Id { get; set; } = string.Empty;
    
    public string Abbreviation { get; set; } = string.Empty;
    
    public string? AbbreviationLocal { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? NameLocal { get; set; }
    
    public string? Copyright { get; set; }
    
    public string Info { get; set; } = string.Empty;

    public bool Active { get; set; } = false; // Is this authorized Bible visible to app users?

    public bool Authorized { get; set; } = false; // Authorized by API.Bible to use
    
    public DateTime? NextScheduledAutoSync { get; set; }
}