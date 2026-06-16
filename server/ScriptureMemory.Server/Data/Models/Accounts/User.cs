using ScriptureMemory.Server.DataAccess.Models;
using System.ComponentModel;
using static ScriptureMemory.Server.Tools.Enums;

namespace DataAccess.Models;

/// <summary>
/// The end-user who will be using the mobile app
/// </summary>
public class User : Account
{
    public string Username { get; set; } = string.Empty;
    
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string? ProfileDescription { get; set; }
    
    public string? ProfilePictureUrl { get; set; }

    public int VersesMemorizedCount { get; set; } = 0;

    public int Points { get; set; } = 0;

    public int CollectionsCount { get; set; } = 0;
    
    public UserPreferences Preferences { get; set; } = new();

    public List<Collection> Collections { get; set; } = new();
}
