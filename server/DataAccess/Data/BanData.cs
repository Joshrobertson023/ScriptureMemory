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

namespace DataAccess.Data;

public class BanData : IBanData
{
    private readonly IConfiguration _config;
    private readonly string connectionString;

    public BanData(IConfiguration config)
    {
        _config = config;
        connectionString = _config.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");
    }

    public async Task<Ban?> GetActiveBan(string username)
    {
        var sql = @"
            SELECT BAN_ID AS BanId, USERNAME AS Username, ADMIN_BANNED AS AdminBanned, 
                   REASON AS Reason, BAN_DATE AS BanDate, BAN_EXPIRE_DATE AS BanExpireDate
            FROM BANS
            WHERE USERNAME = :Username
            AND (BAN_EXPIRE_DATE IS NULL OR BAN_EXPIRE_DATE > SYSDATE)
            ORDER BY BAN_DATE DESC
            FETCH FIRST 1 ROWS ONLY";
        
        using IDbConnection conn = new OracleConnection(connectionString);
        var result = await conn.QueryFirstOrDefaultAsync<Ban>(sql, new { Username = username }, commandType: CommandType.Text);
        return result;
    }

    public async Task<List<Ban>> GetAllBans(string username)
    {
        var sql = @"
            SELECT BAN_ID AS BanId, USERNAME AS Username, ADMIN_BANNED AS AdminBanned, 
                   REASON AS Reason, BAN_DATE AS BanDate, BAN_EXPIRE_DATE AS BanExpireDate
            FROM BANS
            WHERE USERNAME = :Username
            ORDER BY BAN_DATE DESC";
        
        using IDbConnection conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<Ban>(sql, new { Username = username }, commandType: CommandType.Text);
        return results.ToList();
    }

    public async Task<Ban> CreateBan(string username, string adminBanned, string? reason, DateTime? banExpireDate)
    {
        // First, set user as banned
        var updateUserSql = @"UPDATE USERS SET BANNED = 1 WHERE USERNAME = :Username";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(updateUserSql, new { Username = username }, commandType: CommandType.Text);

        // Insert ban record - Oracle doesn't support RETURNING in the same way, so we'll get the ID from a sequence or use a different approach
        var insertSql = @"
            INSERT INTO BANS (BAN_ID, USERNAME, ADMIN_BANNED, REASON, BAN_DATE, BAN_EXPIRE_DATE)
            VALUES (BAN_ID_SEQ.NEXTVAL, :Username, :AdminBanned, :Reason, SYSDATE, :BanExpireDate)";
        
        await conn.ExecuteAsync(insertSql, new { Username = username, AdminBanned = adminBanned, Reason = reason, BanExpireDate = banExpireDate }, commandType: CommandType.Text);
        
        // Get the created ban using the sequence's current value
        var getBanSql = @"
            SELECT BAN_ID AS BanId, USERNAME AS Username, ADMIN_BANNED AS AdminBanned, 
                   REASON AS Reason, BAN_DATE AS BanDate, BAN_EXPIRE_DATE AS BanExpireDate
            FROM BANS
            WHERE USERNAME = :Username
            AND ADMIN_BANNED = :AdminBanned
            ORDER BY BAN_DATE DESC
            FETCH FIRST 1 ROWS ONLY";
        
        var ban = await conn.QueryFirstOrDefaultAsync<Ban>(getBanSql, new { Username = username, AdminBanned = adminBanned }, commandType: CommandType.Text);
        return ban ?? throw new Exception("Failed to retrieve created ban");
    }

    public async Task DeleteBan(int banId)
    {
        var sql = @"DELETE FROM BANS WHERE BAN_ID = :BanId";
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { BanId = banId }, commandType: CommandType.Text);
    }

    public async Task<bool> IsUserBanned(string username)
    {
        var activeBan = await GetActiveBan(username);
        return activeBan != null;
    }
}

