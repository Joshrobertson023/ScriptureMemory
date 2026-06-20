using System.Data;
using System.Linq;
using System.Text.Json;
using Dapper;
using DataAccess.Models;
using ScriptureMemory.Server.DataAccess.Models;
using ScriptureMemory.Server.Tools;

namespace DataAccess.Data;

public class AdminDataDapper : IAdminData
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;

    public AdminDataDapper(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetConnectionString("PostgresConnection")
            ?? throw new InvalidOperationException("Connection string 'PostgresConnection' not found");
    }

    public async Task<int> InsertAdmin(Admin admin)
    {
        var sql =
            """
            insert into admins (role, admin_email)
            values (@Role, @AdminEmail)
            returning id
            """;

        await using var conn = new Npgsql.NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        var result = await conn.QuerySingleAsync<int>(sql, admin);
        return result;
    }

    public async Task UpdatePassword(int adminId, string password)
    {
        var sql = "update admins set hashed_password = @Password where id = @Id";

        using var conn = new Npgsql.NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        await conn.ExecuteAsync(sql, new { Id = adminId, Password = password });

        await conn.CloseAsync();
    }

    public async Task UpdatePersonalEmail(int adminId, string personalEmail)
    {
        var sql =
            """
            update admins set personal_email = @PersonalEmail
            where id = @Id
            """;

        await using var conn = new Npgsql.NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await conn.ExecuteAsync(sql, new { Id = adminId, PersonalEmail = personalEmail });
    }


    public async Task<Admin?> GetAdminByUsername(string username)
    {
        var sql =
            """
            select id, hashed_password as HashedPassword, role,
            admin_email as AdminEmail, personal_email as PersonalEmai
            from admins
            where admin_email = @Username
            """;

        await using var conn = new Npgsql.NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        var result = await conn.QuerySingleOrDefaultAsync<Admin>(sql, new { Username = username });
        return result;
    }

    public async Task<Admin?> GetAdminById(int id)
    {
        var sql =
            """
            select id, hashed_password as HashedPassword, role,
            admin_email as AdminEmail, personal_email as PersonalEmail
            from admins
            where id = @Id
            """;

        await using var conn = new Npgsql.NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        return await conn.QuerySingleOrDefaultAsync<Admin>(sql, new { Id = id });
    }

    public async Task<List<Admin>> GetAllAdmins()
    {
        var sql =
            """
            select id, hashed_password as HashedPassword, role,
            admin_email as AdminEmail, personal_email as PersonalEmail
            from admins
            """;

        await using var conn = new Npgsql.NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        return (await conn.QueryAsync<Admin>(sql)).ToList();
    }


    // -- Admin Actions ------------------------------------------------------
    public async Task<int> InsertAdminAction(AdminAction action)
    {
        var sql =
            """
            insert into admin_actions
            (admin_id, action_type, timestamp, json_metadata)
            values
            (@AdminId, @ActionType, @Timestamp, @JsonMetadata::jsonb)
            returning id
            """;

        await using var conn = new Npgsql.NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        var result = await conn.QuerySingleAsync<int>(sql, new
        {
            AdminId = action.AdminId,
            ActionType = action.ActionType,
            Timestamp = action.Timestamp,
            JsonMetadata = action.JsonMetadata
        });
        return result;
    }

    public async Task<List<AdminAction>> GetAdminActions(int? page = null, int? pageSize = null)
    {
        if (page is null) page = 1;
        if (pageSize is null) pageSize = 50;

        var offset = (page - 1) * pageSize;

        var sql =
            $"""
            select 
            id, admin_id as AdminId, action_type as ActionType, timestamp, json_metadata as JsonMetadata
            from admin_actions
            order by timestamp desc
            limit @PageSize offset @Offset
            """;

        await using var conn = new Npgsql.NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        var result = await conn.QueryAsync<AdminAction>(sql, new { PageSize = pageSize, Offset = offset });
        return result.ToList();
    }
}

