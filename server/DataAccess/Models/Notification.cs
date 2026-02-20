using System;
using VerseAppLibrary;

namespace DataAccess.Models;

public class Notification
{
    public int Id { get; set; }
    public string? Receiver { get; set; }
    public string? Sender { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpirationDate { get; set; }
    public bool IsRead { get; set; } = false;
    public Enums.NotificationType NotificationType { get; set; }

    public Notification(
        string receiver,
        string sender,
        string message,
        Enums.NotificationType type)
    {
        Receiver = receiver;
        Sender = sender;
        Message = message;
        NotificationType = type;
    }
}