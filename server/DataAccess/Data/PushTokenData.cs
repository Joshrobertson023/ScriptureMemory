using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using DataAccess.DataInterfaces;
using DataAccess.Models;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace DataAccess.Data;

public class PushTokenData : IPushTokenData
{
    private readonly IConfiguration _config;
    private readonly string connectionString;

    public PushTokenData(IConfiguration config)
    {
        _config = config;
        connectionString = _config.GetConnectionString("Default");
    }

    public async Task UpsertTokenAsync(string username, string expoPushToken, string platform)
    {
        const string sql = @"
MERGE INTO USER_PUSH_TOKENS target
USING (SELECT :Username AS USERNAME, :Token AS TOKEN FROM DUAL) source
   ON (target.USERNAME = source.USERNAME AND target.TOKEN = source.TOKEN)
 WHEN MATCHED THEN
   UPDATE SET
       PLATFORM = :Platform,
       UPDATED_AT = SYSDATE
 WHEN NOT MATCHED THEN
   INSERT (USERNAME, TOKEN, PLATFORM, UPDATED_AT)
   VALUES (:Username, :Token, :Platform, SYSDATE)";

        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new
        {
            Username = username,
            Token = expoPushToken,
            Platform = platform
        }, commandType: CommandType.Text);
    }

    public async Task RemoveTokenAsync(string username, string expoPushToken)
    {
        const string sql = @"DELETE FROM USER_PUSH_TOKENS WHERE USERNAME = :Username AND TOKEN = :Token";

        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new
        {
            Username = username,
            Token = expoPushToken
        }, commandType: CommandType.Text);
    }

    public async Task RemoveTokensAsync(IEnumerable<string> expoPushTokens)
    {
        var tokens = expoPushTokens?.Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().ToList();
        if (tokens == null || tokens.Count == 0)
        {
            return;
        }

        var parameterNames = tokens
            .Select((_, index) => $":t{index}")
            .ToArray();

        var sql = $@"DELETE FROM USER_PUSH_TOKENS WHERE TOKEN IN ({string.Join(", ", parameterNames)})";

        var parameters = new DynamicParameters();
        for (var i = 0; i < tokens.Count; i++)
        {
            parameters.Add($"t{i}", tokens[i]);
        }

        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, parameters, commandType: CommandType.Text);
    }

    public async Task<IEnumerable<PushToken>> GetTokensForUserAsync(string username)
    {
        const string sql = @"
SELECT USERNAME, TOKEN AS ExpoPushToken, PLATFORM, UPDATED_AT AS UpdatedAt
FROM USER_PUSH_TOKENS
WHERE USERNAME = :Username";

        using IDbConnection conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<PushToken>(sql, new { Username = username }, commandType: CommandType.Text);
        return results.ToList();
    }

    public async Task<IEnumerable<PushToken>> GetTokensForUsersAsync(IEnumerable<string> usernames)
    {
        var usernameList = usernames?
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .Select(u => u.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (usernameList == null || usernameList.Count == 0)
        {
            return Array.Empty<PushToken>();
        }

        var parameterNames = usernameList
            .Select((_, index) => $":u{index}")
            .ToArray();

        var sql = $@"
SELECT USERNAME, TOKEN AS ExpoPushToken, PLATFORM, UPDATED_AT AS UpdatedAt
FROM USER_PUSH_TOKENS
WHERE USERNAME IN ({string.Join(", ", parameterNames)})";

        var parameters = new DynamicParameters();
        for (var i = 0; i < usernameList.Count; i++)
        {
            parameters.Add($"u{i}", usernameList[i]);
        }

        using IDbConnection conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<PushToken>(sql, parameters, commandType: CommandType.Text);
        return results.ToList();
    }

    public async Task<IEnumerable<PushToken>> GetAllTokensAsync()
    {
        const string sql = @"
SELECT USERNAME, TOKEN AS ExpoPushToken, PLATFORM, UPDATED_AT AS UpdatedAt
FROM USER_PUSH_TOKENS";

        using IDbConnection conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<PushToken>(sql, commandType: CommandType.Text);
        return results.ToList();
    }
}


