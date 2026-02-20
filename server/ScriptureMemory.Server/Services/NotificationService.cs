using DataAccess.DataInterfaces;
using DataAccess.Models;
using VerseAppLibrary;

namespace VerseAppNew.Server.Services;

public interface INotificationService
{
    Task SendNotification(Notification notification);
}

public sealed class NotificationService : INotificationService
{
    private readonly INotificationData notificationContext; 

    public NotificationService(INotificationData notificationContext)
    {
        this.notificationContext = notificationContext;
    }

    public async Task SendNotification(Notification notification)
    {
        if (notification.Sender is null)
            notification.Sender = notification.NotificationType == Enums.NotificationType.System 
                ? "System" 
                : throw new ArgumentNullException(notification.Sender);

        if (notification.Receiver is null)
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
