using System.ComponentModel;

namespace ScriptureMemory.Server.DataAccess.Models;

public class Session
{
    [Key]
    public int Id { get; set; }

    public int? UserId { get; set; }
    public Account? AccountNavigation { get; set; } = null!;
    
    [MaxLength(50)]
    public string DeviceId { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string DeviceName { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Model { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? RefreshTokenHash { get; set; } // Token that allows automatic logging in
    
    [MaxLength(100)]
    public string? PushNotificationToken { get; set; } // Mobile push notification token
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime LastSeenAt { get; set; } = DateTime.UtcNow;
}
