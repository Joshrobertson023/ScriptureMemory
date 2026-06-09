using ScriptureMemory.Server.DataAccess.Models;
using System.ComponentModel;
using static ScriptureMemory.Server.Tools.Enums;

namespace DataAccess.Models;

/// <summary>
/// The end-user who will be using the mobile app
/// </summary>
public class User : Account
{
    [MaxLength(20)]
    public string Username { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string FirstName { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string LastName { get; set; } = string.Empty;
    
    [MaxLength(30)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? ProfileDescription { get; set; }
    
    [MaxLength(100)]
    public string? ProfilePictureUrl { get; set; }
    
    [DefaultValue(0)]
    public int VersesMemorizedCount { get; set; }

    [DefaultValue(0)]
    public int Points { get; set; }
    
    [DefaultValue(0)]
    public byte CollectionsCount { get; set; }
    
    public UserPreferences Preferences { get; set; } = new();
    
    public PaidInfo? Paid { get; set; } // User's payment information

    public List<Collection> Collections { get; set; } = new();
}
