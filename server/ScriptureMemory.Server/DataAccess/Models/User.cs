using ScriptureMemory.Server.DataAccess.Models;
using static ScriptureMemory.Server.Tools.Enums;

namespace DataAccess.Models;

public class User : Account
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? DateRegistered { get; set; }
    public UserPreferences Preferences { get; set; } = new();
    public string? ProfileDescription { get; set; }
    public string? PushNotificationToken { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public int VersesMemorizedCount { get; set; } = 0;

    public int Points { get; set; } = 0;

    public Paid? Paid { get; set; }
    public byte CollectionsCount { get; set; } = 0;
}
