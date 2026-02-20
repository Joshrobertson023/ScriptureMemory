using DataAccess.Models;

namespace DataAccess.DataInterfaces;

public interface INotificationData
{
    Task CreateNotification(Notification notification);
    Task<List<Notification>> GetUserNotifications(string username);
    Task<List<Notification>> GetTopNotifications(string username, int limit);
    Task<List<Notification>> GetNotificationsBefore(string username, int cursorId, int limit);
    Task MarkNotificationAsRead(int notificationId);
    Task MarkAllNotificationsAsRead(string username);
    Task ExpireNotification(int notificationId);
    Task<int> GetUnreadNotificationCount(string username);
    Task SendNotificationToAllUsers(Notification notification);
    Task SendNotificationToAdmins(Notification notification);
    Task DeleteNotificationsByUsername(string username);
}
