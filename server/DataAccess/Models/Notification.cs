using System;
using ScriptureMemoryLibrary;

namespace DataAccess.Models;

public class Notification
{
    public int Id { get; set; }
    public int? SenderId { get; set; }
    public int? ReceiverId { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpirationDate { get; set; }
    public bool IsRead { get; set; } = false;
    public Enums.NotificationType NotificationType { get; set; }

    public Notification(
        int receiver,
        int sender,
        string message,
        Enums.NotificationType type)
    {
        ReceiverId = receiver;
        SenderId = sender;
        Message = message;
        NotificationType = type;
    }

    public Notification() { }
}
