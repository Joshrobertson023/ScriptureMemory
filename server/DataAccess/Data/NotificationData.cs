using Dapper;
using DataAccess.DataInterfaces;
using DataAccess.Models;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccess.Data;

public class NotificationData : INotificationData
{
    private readonly IDbConnection conn;

    public NotificationData(IDbConnection connection)
    {
        conn = connection;
    }

    public async Task CreateNotification(Notification notification)
    {
        var sql = @"INSERT INTO NOTIFICATIONS
                    (RECEIVER_ID, SENDER_ID, MESSAGE, CREATEDDATE, ISREAD, NOTIFICATIONTYPE, EXPIRATION_DATE)
                    VALUES 
                    (:Receiver, :Sender, :Message, :CreatedDate, 0, :NotificationType, :ExpirationDate)";
        
        await conn.ExecuteAsync(sql, new
        {
            Receiver = notification.ReceiverId,
            Sender = notification.SenderId,
            Message = notification.Message,
            CreatedDate = notification.CreatedDate,
            NotificationType = notification.NotificationType,
            ExpirationDate = notification.ExpirationDate
        });
    }

    public async Task<List<Notification>> GetUserNotifications(int userId)
    {
        var sql = @"SELECT ID as Id, 
                           MESSAGE as Message, 
                           CREATEDDATE as CreatedDate, 
                           ISREAD as IsRead, 
                           EXPIRATION_DATE as ExpirationDate,
                           NOTIFICATIONTYPE as NotificationType,
                           SENDER_ID as SenderId,
                           RECEIVER_ID as ReceiverId
                    FROM NOTIFICATIONS 
                    WHERE RECEIVER_ID = :UserId 
                    ORDER BY CREATEDDATE DESC";
        
        var notifications = await conn.QueryAsync<Notification>(
            sql, 
            new { UserId = userId });
        return notifications.ToList();
    }

    public async Task<List<Notification>> GetTopNotifications(int userId, int limit)
    {
        var sql = @"
            SELECT * FROM (
                SELECT ID as Id,
                       MESSAGE as Message,
                       CREATEDDATE as CreatedDate,
                       ISREAD as IsRead,
                       EXPIRATION_DATE as ExpirationDate,
                       NOTIFICATIONTYPE as NotificationType,
                       SENDER_ID as SenderId,
                       RECEIVER_ID as ReceiverId
                FROM NOTIFICATIONS 
                WHERE RECEIVER_ID = :UserId 
                ORDER BY CREATEDDATE DESC, ID DESC
            )
            WHERE ROWNUM <= :Limit
        ";

        var notifications = await conn.QueryAsync<Notification>(
            sql, 
            new 
            { 
                UserId = userId,
                Limit = limit 
            });
        return notifications.ToList();
    }

    public async Task<List<Notification>> GetNotificationsBefore(int userId, int cursorId, int limit)
    {
        var sql = @"
            SELECT * FROM (
                SELECT ID as Id,
                       MESSAGE as Message,
                       CREATEDDATE as CreatedDate,
                       ISREAD as IsRead,
                       EXPIRATION_DATE as ExpirationDate,
                       NOTIFICATIONTYPE as NotificationType,
                       SENDER_ID as SenderId,
                       RECEIVER_ID as ReceiverId
                FROM NOTIFICATIONS 
                WHERE RECEIVER_ID = :UserId 
                  AND (ID < :CursorId)
                ORDER BY CREATEDDATE DESC, ID DESC
            )
            WHERE ROWNUM <= :Limit
        ";

        var notifications = await conn.QueryAsync<Notification>(sql, new
        {
            UserId = userId,
            CursorId = cursorId,
            Limit = limit
        }, commandType: CommandType.Text);
        return notifications.ToList();
    }

    public async Task MarkNotificationAsRead(int notificationId)
    {
        var sql = @"
            UPDATE NOTIFICATIONS 
            SET ISREAD = 1 
            WHERE ID = :NotificationId
        ";
        
        await conn.ExecuteAsync(sql, new { NotificationId = notificationId }, commandType: CommandType.Text);
    }

    public async Task MarkAllNotificationsAsRead(int userId)
    {
        var sql = @"
            UPDATE NOTIFICATIONS 
            SET ISREAD = 1 
            WHERE RECEIVER_ID = :UserId AND ISREAD = 0
        ";
        
        await conn.ExecuteAsync(sql, new { UserId = userId }, commandType: CommandType.Text);
    }

    public async Task ExpireNotification(int notificationId)
    {
        var sql = @"
            UPDATE NOTIFICATIONS 
            SET EXPIRATION_DATE = :CurrentDate 
            WHERE ID = :NotificationId
        ";
        
        await conn.ExecuteAsync(
            sql, 
            new 
            { 
                NotificationId = notificationId,
                CurrentDate = DateTime.UtcNow
            });
    }

    public async Task<int> GetUnreadNotificationCount(int userId)
    {
        var sql = @"
            SELECT COUNT(*) FROM NOTIFICATIONS 
            WHERE RECEIVER_ID = :UserId AND ISREAD = 0
        ";
        
        var count = await conn.QueryFirstOrDefaultAsync<int>(sql, new { UserId = userId }, commandType: CommandType.Text);
        return count;
    }

    public async Task SendNotificationToAllUsers(Notification notification)
    {
        var sql = @"
            INSERT INTO NOTIFICATIONS 
            (SENDER_ID, RECEIVER_ID, MESSAGE, CREATEDDATE, ISREAD, NOTIFICATIONTYPE)
            SELECT :SenderId, u.ID, :Message, :CreatedDate, 0, :NotificationType
            FROM USERS u
        ";
        
        await conn.ExecuteAsync(
            sql, 
            new 
            { 
                SenderId = notification.SenderId, 
                Message = notification.Message,
                CreatedDate = notification.CreatedDate,
                NotificationType = notification.NotificationType,
            });
    }

    public async Task SendNotificationToAdmins(Notification notification)
    {
        var sql = @"
            INSERT INTO NOTIFICATIONS 
            (RECEIVER_ID, SENDER_ID, MESSAGE, CREATEDDATE, ISREAD, NOTIFICATIONTYPE)
            SELECT u.ID, :SenderId, :Message, :CreatedDate, 0, :NotificationType
            FROM USERS u
            WHERE u.ISADMIN = 1
        ";
        
        await conn.ExecuteAsync(
            sql, 
            new 
            { 
                SenderId = notification.SenderId, 
                Message = notification.Message,
                CreatedDate = DateTime.UtcNow,
                NotificationType = notification.NotificationType
            });
    }

    public async Task DeleteNotificationsByUserId(int userId)
    {
        
        var sql = @"
            DELETE FROM NOTIFICATIONS 
            WHERE RECEIVER_ID = :UserId OR SENDER_ID = :UserId
        ";
        
        await conn.ExecuteAsync(sql, new { UserId = userId }, commandType: CommandType.Text);
    }
}
