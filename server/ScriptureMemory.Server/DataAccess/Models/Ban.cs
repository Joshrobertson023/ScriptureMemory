using System;

namespace DataAccess.Models;

public sealed class Ban
{
    public int BanId { get; set; }
    public string Username { get; set; }
    public string AdminBanned { get; set; }
    public string? Reason { get; set; }
    public DateTime BanDate { get; set; }
    public DateTime? BanExpireDate { get; set; }

    public Ban(string usernameBanned, string adminUsername)
    {
        Username = usernameBanned;
        AdminBanned = adminUsername;
        BanDate = DateTime.UtcNow;
    }
}































