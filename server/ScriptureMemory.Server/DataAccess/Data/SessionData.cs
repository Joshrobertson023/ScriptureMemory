using Dapper;
using Npgsql;
using ScriptureMemory.Server.DataAccess.Models;

namespace ScriptureMemory.Server.DataAccess.Data;

public class SessionData
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;

    public SessionData(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetConnectionString("PostgresConnection")
            ?? throw new InvalidOperationException("Connection string 'PostgresConnection' not found");
    }

    public async Task<Session> CreateSession(int userId, Session session)
    {
        string deviceId = Guid.NewGuid().ToString();
        using var conn = new NpgsqlConnection(_connectionString);
        var results = await conn.ExecuteScalarAsync<int>(
            """
            insert into device_sessions
            (user_id, device_id, device_name, model, refresh_token_hash, push_notification_token, created_at, last_seen_at)
            values
            (@UserId, @DeviceId, @DeviceName, @Model, @RefreshTokenHash, @PushNotificationToken, @CreatedAt, @LastSeenAt)
            returning id
            """, new
            {
                UserId = userId,
                DeviceId = deviceId,
                DeviceName = session.DeviceName,
                Model = session.Model,
                RefreshTokenHash = session.RefreshTokenHash,
                PushNotificationToken = session.PushNotificationToken,
                CreatedAt = session.CreatedAt,
                LastSeenAt = session.LastSeenAt,
            });
        return new Session
        {
            Id = results,
            DeviceId = deviceId,
            DeviceName = session.DeviceName,
            Model = session.Model,
            RefreshTokenHash = session.RefreshTokenHash,
            PushNotificationToken = session.PushNotificationToken,
            CreatedAt = session.CreatedAt,
            LastSeenAt = session.LastSeenAt,
        };
    }

    public async Task<string?> GetDeviceId(string refreshTokenHash)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        string? deviceId = await conn.ExecuteScalarAsync<string?>(
            """
            select device_id from device_sessions
            where refresh_token_hash = @RefreshTokenHash
            """, new 
            { 
                RefreshTokenHash = refreshTokenHash 
            });
        return deviceId;
    }

    public async Task UpdateRefreshToken(int userId, Session session)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(
            """
            update device_sessions
            set refresh_token_hash = @RefreshTokenHash, last_seen_at = @LastSeenAt
            where user_id = @UserId
            """, new
            {
                UserId = userId,
                RefreshTokenHash = session.RefreshTokenHash,
                LastSeenAt = DateTime.UtcNow,
            });
    }

    public async Task LoginSession(Session session)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(
            """
            update device_sessions
            set last_seen_at = @LastSeenAt, refresh_token_hash = @RefreshTokenHash
            where device_id = @DeviceId
            """, new
            {
                DeviceId = session.DeviceId,
                RefreshTokenHash = session.RefreshTokenHash,
                LastSeenAt = DateTime.UtcNow,
            });
    }
}
