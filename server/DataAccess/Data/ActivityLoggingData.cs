using Dapper;
using DataAccess.DataInterfaces;
using DataAccess.Models;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerseAppLibrary;

namespace DataAccess.Data;

public class PagedLogs<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public sealed class ActivityLoggingData : IActivityLoggingData
{
    private readonly IConfiguration _config;
    private readonly string connectionString;

    private const string selectClause = @"ID, USERNAME, ACTION_TYPE as ActionType, ENTITY_TYPE as EntityType, 
                                        ENTITY_ID as EntityId, CONTEXT_DESCRIPTION as ContextDescription,
                                        METADATA_JSON as JsonMetadata, SEVERITY_LEVEL as SeverityLevel,
                                        IS_ADMIN_ACTION as IsAdminAction, CREATED_AT as CreatedAt";

    public ActivityLoggingData(IConfiguration config)
    {
        _config = config;
        connectionString = _config.GetConnectionString("Default")!;
    }

    public async Task Create(ActivityLog log)
    {
        var sql = @"INSERT INTO ACTIVITY_LOGS
                    (USERNAME, ACTION_TYPE, ENTITY_TYPE, ENTITY_ID, CONTEXT_DESCRIPTION, 
                     METADATA_JSON, SEVERITY_LEVEL, IS_ADMIN_ACTION, CREATED_AT)
                    VALUES
                    (:Username, :ActionType, :EntityType, :EntityId, :ContextDescription,
                     :MetadataJson, :SeverityLevel, :IsAdminAction, :CreatedAt)";

        await using var conn = new OracleConnection(connectionString);
        await conn.OpenAsync();

        await conn.ExecuteAsync(sql,
            new
            {
                Username = log.Username,
                ActionType = log.ActionType,
                EntityType = log.EntityType,
                EntityId = log.EntityId,
                ContextDescription = log.ContextDescription,
                MetadataJson = log.JsonMetadata,
                SeverityLevel = log.SeverityLevel,
                IsAdminAction = Convert.ToInt(log.IsAdminAction),
                CreatedAt = log.CreatedAt
            });
    }

    public async Task<ActivityLog?> GetById(int id)
    {
        var sql = $@"SELECT
                    {selectClause}
                    FROM ACTIVITY_LOGS WHERE ID = :Id";

        await using var conn = new OracleConnection(connectionString);
        return await conn.QuerySingleOrDefaultAsync<ActivityLog>(
            sql,
            new { Id = id });
    }

    public async Task<PagedLogs<ActivityLog>> GetByUser(string username, int page = 1, int pageSize = 50)
    {
        var sql = $@"SELECT {selectClause} FROM ACTIVITY_LOGS
                     WHERE USERNAME = :Username
                     ORDER BY CREATED_AT DESC
                     OFFSET :Offset ROWS FETCH NEXT :PageSize ROWS ONLY";

        await using var conn = new OracleConnection(connectionString);
        return new PagedLogs<ActivityLog>
        {
            Items = (await conn.QueryAsync<ActivityLog>(
                sql,
                new
                {
                    Username = username,
                    Offset = (page - 1) * pageSize,
                    PageSize = pageSize
                })).AsList(),
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedLogs<ActivityLog>> GetByEntity(Enums.EntityType entityType, int entityId, int page = 1, int pageSize = 50)
    {
        var sql = $@"SELECT {selectClause} FROM ACTIVITY_LOGS
                     WHERE ENTITY_TYPE = :EntityType AND ENTITY_ID = :EntityId
                     ORDER BY CREATED_AT DESC
                     OFFSET :Offset ROWS FETCH NEXT :PageSize ROWS ONLY";

        await using var conn = new OracleConnection(connectionString);
        return new PagedLogs<ActivityLog>
        {
            Items = (await conn.QueryAsync<ActivityLog>(
                sql,
                new
                {
                    EntityType = entityType,
                    EntityId = entityId,
                    Offset = (page - 1) * pageSize,
                    PageSize = pageSize
                })).AsList(),
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedLogs<ActivityLog>> GetByActionType(Enums.ActionType actionType, int page = 1, int pageSize = 50)
    {
        var sql = $@"SELECT {selectClause} FROM ACTIVITY_LOGS
                     WHERE ACTION_TYPE = :ActionType
                     ORDER BY CREATED_AT DESC
                     OFFSET :Offset ROWS FETCH NEXT :PageSize ROWS ONLY";

        await using var conn = new OracleConnection(connectionString);
        return new PagedLogs<ActivityLog>
        {
            Items = (await conn.QueryAsync<ActivityLog>(
                sql,
                new
                {
                    ActionType = actionType,
                    Offset = (page - 1) * pageSize,
                    PageSize = pageSize
                })).AsList(),
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedLogs<ActivityLog>> GetByDateRange(DateTime from, DateTime to, int page = 1, int pageSize = 50)
    {
        var sql = $@"SELECT {selectClause} FROM ACTIVITY_LOGS
                     WHERE CREATED_AT >= :From AND CREATED_AT <= :To
                     ORDER BY CREATED_AT DESC
                     OFFSET :Offset ROWS FETCH NEXT :PageSize ROWS ONLY";

        await using var conn = new OracleConnection(connectionString);
        return new PagedLogs<ActivityLog>
        {
            Items = (await conn.QueryAsync<ActivityLog>(
                sql,
                new
                {
                    From = from,
                    To = to,
                    Offset = (page - 1) * pageSize,
                    PageSize = pageSize
                })).AsList(),
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedLogs<ActivityLog>> GetAdminActions(int page = 1, int pageSize = 50)
    {
        var sql = $@"SELECT {selectClause} FROM ACTIVITY_LOGS
                     WHERE IS_ADMIN_ACTION = 1
                     ORDER BY CREATED_AT DESC
                     OFFSET :Offset ROWS FETCH NEXT :PageSize ROWS ONLY";

        await using var conn = new OracleConnection(connectionString);
        return new PagedLogs<ActivityLog>
        {
            Items = (await conn.QueryAsync<ActivityLog>(
                sql,
                new
                {
                    Offset = (page - 1) * pageSize,
                    PageSize = pageSize
                })).AsList(),
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<int> DeleteOlderThan(DateTime cutoff)
    {
        var sql = "DELETE FROM ACTIVITY_LOGS WHERE CREATED_AT < :Cutoff";

        await using var conn = new OracleConnection(connectionString);
        return await conn.ExecuteAsync(sql, new { Cutoff = cutoff });
    }

    public async Task<int> DeleteLogsForUser(string username)
    {
        var sql = "DELETE FROM ACTIVITY_LOGS WHERE USERNAME = :Username";

        await using var conn = new OracleConnection(connectionString);
        return await conn.ExecuteAsync(sql, new { Username = username });
    }
}
