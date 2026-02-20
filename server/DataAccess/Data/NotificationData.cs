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
    private readonly IConfiguration _config;
    private readonly string connectionString;

    public NotificationData(IConfiguration config)
    {
        _config = config;
        connectionString = _config.GetConnectionString("Default")!;
    }

    public async Task CreateNotification(Notification notification)
    {
        var sql = @"INSERT INTO NOTIFICATIONS
                    (RECEIVER, SENDER, MESSAGE, CREATEDDATE, ISREAD, NOTIFICATIONTYPE, EXPIRATION_DATE)
                    VALUES 
                    (:Receiver, :Sender, :Message, :CreatedDate, 0, :NotificationType, :ExpirationDate)";
        
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new
        {
            Receiver = notification.Receiver,
            Sender = notification.Sender,
            Message = notification.Message,
            CreatedDate = notification.CreatedDate,
            NotificationType = notification.NotificationType,
            ExpirationDate = notification.ExpirationDate
        });
    }

    public async Task<List<Notification>> GetUserNotifications(string username)
    {
        var sql = @"SELECT * FROM NOTIFICATIONS 
                    WHERE USERNAME = :Username 
                    ORDER BY CREATEDDATE DESC";
        
        using IDbConnection conn = new OracleConnection(connectionString);
        var notifications = await conn.QueryAsync<Notification>(
            sql, 
            new { Username = username });
        return notifications.ToList();
    }

    public async Task<List<Notification>> GetTopNotifications(string username, int limit)
    {
        var sql = @"
            SELECT * FROM (
                SELECT * FROM NOTIFICATIONS 
                WHERE USERNAME = :Username 
                ORDER BY CREATEDDATE DESC, ID DESC
            )
            WHERE ROWNUM <= :Limit
        ";

        using IDbConnection conn = new OracleConnection(connectionString);
        var notifications = await conn.QueryAsync<Notification>(sql, new { Username = username, Limit = limit }, commandType: CommandType.Text);
        return notifications.ToList();
    }

    public async Task<List<Notification>> GetNotificationsBefore(string username, int cursorId, int limit)
    {
        var sql = @"
            SELECT * FROM (
                SELECT * FROM NOTIFICATIONS 
                WHERE USERNAME = :Username 
                  AND (ID < :CursorId)
                ORDER BY CREATEDDATE DESC, ID DESC
            )
            WHERE ROWNUM <= :Limit
        ";

        using IDbConnection conn = new OracleConnection(connectionString);
        var notifications = await conn.QueryAsync<Notification>(sql, new
        {
            Username = username,
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
        
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { NotificationId = notificationId }, commandType: CommandType.Text);
    }

    public async Task MarkAllNotificationsAsRead(string username)
    {
        var sql = @"
            UPDATE NOTIFICATIONS 
            SET ISREAD = 1 
            WHERE USERNAME = :Username AND ISREAD = 0
        ";
        
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { Username = username }, commandType: CommandType.Text);
    }

    public async Task ExpireNotification(int notificationId)
    {
        var sql = @"
            UPDATE NOTIFICATIONS 
            SET EXPIRATION_DATE = :CurrentDate 
            WHERE ID = :NotificationId
        ";
        
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(
            sql, 
            new 
            { 
                NotificationId = notificationId,
                CurrentDate = DateTime.UtcNow
            });
    }

    public async Task<int> GetUnreadNotificationCount(string username)
    {
        var sql = @"
            SELECT COUNT(*) FROM NOTIFICATIONS 
            WHERE USERNAME = :Username AND ISREAD = 0
        ";
        
        using IDbConnection conn = new OracleConnection(connectionString);
        var count = await conn.QueryFirstOrDefaultAsync<int>(sql, new { Username = username }, commandType: CommandType.Text);
        return count;
    }

    public async Task SendNotificationToAllUsers(Notification notification)
    {
        var sql = @"
            INSERT INTO NOTIFICATIONS 
            (USERNAME, SENDERUSERNAME, MESSAGE, CREATEDDATE, ISREAD, NOTIFICATIONTYPE)
            SELECT USERNAME, :SenderUsername, :Message, :CreatedDate, 0, :NotificationType
            FROM USERS
        ";
        
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(
            sql, 
            new 
            { 
                SenderUsername = notification.Sender, 
                Message = notification.Message,
                CreatedDate = notification.CreatedDate,
                NotificationType = notification.NotificationType,
            });
    }

    public async Task SendNotificationToAdmins(Notification notification)
    {
        var sql = @"
            INSERT INTO NOTIFICATIONS 
            (USERNAME, SENDERUSERNAME, MESSAGE, CREATEDDATE, ISREAD, NOTIFICATIONTYPE)
            SELECT u.USERNAME, :SenderUsername, :Message, :CreatedDate, 0, :NotificationType
            FROM USERS u
            INNER JOIN ADMINS a ON a.USERNAME = u.USERNAME
        ";
        
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(
            sql, 
            new 
            { 
                SenderUsername = notification.Sender, 
                Message = notification.Message,
                CreatedDate = DateTime.UtcNow,
                NotificationType = notification.NotificationType
            });
    }

    public async Task DeleteNotificationsByUsername(string username)
    {
        
        var sql = @"
            DELETE FROM NOTIFICATIONS 
            WHERE USERNAME = :Username OR SENDERUSERNAME = :Username
        ";
        
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { Username = username }, commandType: CommandType.Text);
    }
}
