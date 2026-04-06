namespace ScriptureMemory.Server.DataAccess.Models;

public class Session
{
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string RefreshTokenHash { get; set; } = string.Empty;
    public string? PushNotificationToken { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastSeenAt { get; set; }
}
