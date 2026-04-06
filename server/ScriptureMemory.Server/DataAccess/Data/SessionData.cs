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
            (user_id, device_id, device_name, platform, refresh_token_hash, created_at, last_seen_at)
            values
            (@UserId, @DeviceId, @DeviceName, @Platform, @RefreshTokenHash, @CreatedAt, @LastSeenAt)
            returning id
            """, new
            {
                UserId = userId,
                DeviceId = session.DeviceId,
                DeviceName = session.DeviceName,
                Platform = session.Platform,
                RefreshTokenHash = session.RefreshTokenHash,
                CreatedAt = DateTime.UtcNow,
                LastSeenAt = DateTime.UtcNow,
            });
        return sessionId;
    }
}
