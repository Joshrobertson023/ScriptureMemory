using DataAccess.Data;
using DataAccess.Models;
using ScriptureMemory.Server.Tools;

namespace VerseAppNew.Server.Services;

public sealed class NotificationService
{
    private readonly NotificationData notificationContext; 

    public NotificationService(NotificationData notificationContext)
    {
        this.notificationContext = notificationContext;
    }

    public async Task SendNotification(Notification notification)
    {
        if (notification.SenderId is null)
            notification.SenderId = notification.NotificationType == Enums.NotificationType.System 
                ? Data.NOTIFICATION_SYSTEM_SENDER_ID
                : throw new ArgumentNullException(nameof(notification.SenderId));

        if (notification.ReceiverId is null)
        {
            // Send to all usres
            await notificationContext.SendNotificationToAllUsers(notification);
        }
        else
        { 
            // Send to specific user
            await notificationContext.CreateNotification(notification);
        }
    }
}
