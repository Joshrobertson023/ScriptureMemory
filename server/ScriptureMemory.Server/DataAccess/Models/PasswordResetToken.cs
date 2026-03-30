using System;

namespace DataAccess.Models;

public class PasswordResetToken
{
    public string Username { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime Sent { get; set; }
}

