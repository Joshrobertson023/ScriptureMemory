using System;

namespace DataAccess.Models;

public class PushToken
{
    public string Username { get; set; } = string.Empty;
    public string ExpoPushToken { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}



