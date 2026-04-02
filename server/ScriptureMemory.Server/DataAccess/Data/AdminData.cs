using System.Data;
using System.Linq;
using Dapper;
using DataAccess.Models;

namespace DataAccess.Data;

public class AdminData
{
    private readonly IDbConnection _conn;

    public AdminData([FromKeyedServices("Postgres")] IDbConnection conn)
    {
        _conn = conn;
    }

    public async Task<int> InsertAdmin(Admin admin)
    {
        var sql =
            """
            insert into admins (role, admin_email)
            values (@Role, @AdminEmail)
            returning id
            """;

        return await _conn.QuerySingleAsync<int>(sql, admin);
    }

    public async Task UpdatePassword(int adminId, string password)
    {
        var sql =
            """
            update admins set hashed_password = @Password
            where id = @Id
            """;

        await _conn.ExecuteAsync(sql, new { Id = adminId, Password = password });
    }

    public async Task UpdatePersonalEmail(int adminId, string personalEmail)
    {
        var sql =
            """
            update admins set personal_email = @PersonalEmail
            where id = @Id
            """;
        await _conn.ExecuteAsync(sql, new { Id = adminId, PersonalEmail = personalEmail });
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
        return await _conn.QuerySingleOrDefaultAsync<Admin>(sql, new { Username = username });
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
        return await _conn.QuerySingleOrDefaultAsync<Admin>(sql, new { Id = id });
    }

    public async Task<List<Admin>> GetAllAdmins()
    {
        var sql =
            """
            select id, hashed_password as HashedPassword, role,
            admin_email as AdminEmail, personal_email as PersonalEmail
            from admins
            """;
        return (await _conn.QueryAsync<Admin>(sql)).ToList();
    }
}

