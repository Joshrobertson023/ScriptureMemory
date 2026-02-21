using DataAccess.Models;

namespace DataAccess.DataInterfaces;

public interface INotificationData
{
    Task CreateNotification(Notification notification);
    Task<List<Notification>> GetUserNotifications(int userId);
    Task<List<Notification>> GetTopNotifications(int userId, int limit);
    Task<List<Notification>> GetNotificationsBefore(int userId, int cursorId, int limit);
    Task MarkNotificationAsRead(int notificationId);
    Task MarkAllNotificationsAsRead(int userId);
    Task ExpireNotification(int notificationId);
    Task<int> GetUnreadNotificationCount(int userId);
    Task SendNotificationToAllUsers(Notification notification);
    Task SendNotificationToAdmins(Notification notification);
    Task DeleteNotificationsByUserId(int userId);
}
