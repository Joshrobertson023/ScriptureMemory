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

    public async Task<int> CreateSession(int userId, Session session)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        int sessionId = await conn.ExecuteScalarAsync<int>(
            """
            insert into device_sessions
            (user_id, device_id, device_name, platform, refresh_token_hash, push_notification_token, created_at, last_seen_at)
            values
            (@UserId, @DeviceId, @DeviceName, @Platform, @RefreshTokenHash, @PushNotificationToken, @CreatedAt, @LastSeenAt)
            returning id
            """, new
            {
                UserId = userId,
                DeviceId = session.DeviceId,
                DeviceName = session.DeviceName,
                Platform = session.Platform,
                RefreshTokenHash = session.RefreshTokenHash,
                PushNotificationToken = session.PushNotificationToken,
                CreatedAt = DateTime.UtcNow,
                LastSeenAt = DateTime.UtcNow,
            });
        return sessionId;
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
            where user_id = @UserId and device_id = @DeviceId
            """, new
            {
                UserId = userId,
                DeviceId = session.DeviceId,
                RefreshTokenHash = session.RefreshTokenHash,
                LastSeenAt = DateTime.UtcNow,
            });
    }
}
