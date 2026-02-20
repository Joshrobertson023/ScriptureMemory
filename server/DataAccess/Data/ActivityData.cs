using DataAccess.Models;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DataAccess.DataInterfaces;

namespace DataAccess.Data;

public class ActivityData : IActivityData
{
    private readonly IConfiguration _config;
    private readonly string connectionString;

    public ActivityData(IConfiguration config)
    {
        _config = config;
        connectionString = _config.GetConnectionString("Default")!;
    }

    public async Task CreateActivity(Activity activity)
    {
        var sql = @"
            INSERT INTO ACTIVITY (TEXT, DATE_CREATED, USERNAME)
            VALUES (:Text, SYSDATE, :Username)
        ";
        
        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new
        {
            Text = activity.Text,
            Username = activity.Username
        }, commandType: CommandType.Text);
    }

    public async Task<List<Activity>> GetUserActivity(string username, int limit = 10)
    {
        var sql = @"
            SELECT * FROM (
                SELECT ID, TEXT, DATE_CREATED AS DateCreated, USERNAME AS Username
                FROM ACTIVITY
                WHERE USERNAME = :Username
                ORDER BY DATE_CREATED DESC
            )
            WHERE ROWNUM <= :Limit
        ";
        
        using IDbConnection conn = new OracleConnection(connectionString);
        var activities = await conn.QueryAsync<Activity>(sql, new { Username = username, Limit = limit }, commandType: CommandType.Text);
        return activities.ToList();
    }

    public async Task<List<Activity>> GetFriendsActivity(string username, int limit = 10)
    {
        var activities = await GetFriendActivityInternal(username, limit: limit, filterFriendsOnly: true);
        return activities;
    }

    public async Task<List<Activity>> GetFriendActivity(string username, string viewerUsername, int limit = 10)
    {
        var activities = await GetFriendActivityInternal(username, limit, filterFriendsOnly: false, viewerUsername);
        return activities;
    }

    private async Task<List<Activity>> GetFriendActivityInternal(string username, int limit, bool filterFriendsOnly, string? viewerUsername = null)
    {
        using IDbConnection conn = new OracleConnection(connectionString);

        if (filterFriendsOnly)
        {
            var sql = @"
                SELECT * FROM (
                    SELECT a.ID, a.TEXT, a.DATE_CREATED AS DateCreated, a.USERNAME AS Username
                    FROM ACTIVITY a
                    INNER JOIN USER_RELATIONSHIPS r ON (
                        (r.USERNAME_1 = :Username AND r.USERNAME_2 = a.USERNAME) OR
                        (r.USERNAME_2 = :Username AND r.USERNAME_1 = a.USERNAME)
                    )
                    WHERE r.TYPE = 1
                    ORDER BY a.DATE_CREATED DESC
                )
                WHERE ROWNUM <= :Limit
            ";

            var activities = await conn.QueryAsync<Activity>(sql, new { Username = username, Limit = limit }, commandType: CommandType.Text);
            return activities.ToList();
        }

        if (viewerUsername == null)
        {
            return new List<Activity>();
        }

        var checkFriendSql = @"SELECT COUNT(*) FROM USER_RELATIONSHIPS 
                                WHERE ((USERNAME_1 = :Username1 AND USERNAME_2 = :Username2) OR (USERNAME_1 = :Username2 AND USERNAME_2 = :Username1))
                                AND TYPE = 1";

        var isFriend = await conn.QueryFirstOrDefaultAsync<int>(checkFriendSql, new { Username1 = username, Username2 = viewerUsername }, commandType: CommandType.Text);

        if (isFriend <= 0)
        {
            return new List<Activity>();
        }

        var sqlWithFriendCheck = @"
            SELECT * FROM (
                SELECT a.ID, a.TEXT, a.DATE_CREATED AS DateCreated, a.USERNAME AS Username
                FROM ACTIVITY a
                WHERE a.USERNAME = :Username
                ORDER BY a.DATE_CREATED DESC
            )
            WHERE ROWNUM <= :Limit";

        var activitiesForUser = await conn.QueryAsync<Activity>(sqlWithFriendCheck, new { Username = username, Limit = limit }, commandType: CommandType.Text);
        return activitiesForUser.ToList();
    }
}





