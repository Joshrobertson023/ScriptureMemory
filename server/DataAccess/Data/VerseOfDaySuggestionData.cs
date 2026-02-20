using DataAccess.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.DataInterfaces;

namespace DataAccess.Data;

public class VerseOfDaySuggestionData : IVerseOfDaySuggestionData
{
    private readonly IConfiguration _config;
    private readonly string connectionString;

    public VerseOfDaySuggestionData(IConfiguration config)
    {
        _config = config;
        connectionString = _config.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");
    }

    public async Task<List<VerseOfDaySuggestion>> GetAllSuggestions()
    {
        const string sql = @"
            SELECT ID,
                   READABLE_REFERENCE AS ReadableReference,
                   SUGGESTER_USERNAME AS SuggesterUsername,
                   CREATED_DATE AS CreatedDate,
                   STATUS
            FROM VERSE_OF_DAY_SUGGESTIONS
            ORDER BY CREATED_DATE DESC";

        using IDbConnection conn = new OracleConnection(connectionString);
        var records = await conn.QueryAsync<VerseOfDaySuggestionRecord>(sql, commandType: CommandType.Text);
        return records.Select(MapToModel).Where(s => s != null).ToList();
    }

    public async Task<VerseOfDaySuggestion?> GetSuggestion(int id)
    {
        const string sql = @"
            SELECT ID,
                   READABLE_REFERENCE AS ReadableReference,
                   SUGGESTER_USERNAME AS SuggesterUsername,
                   CREATED_DATE AS CreatedDate,
                   STATUS
            FROM VERSE_OF_DAY_SUGGESTIONS
            WHERE ID = :Id";

        using IDbConnection conn = new OracleConnection(connectionString);
        var record = await conn.QueryFirstOrDefaultAsync<VerseOfDaySuggestionRecord>(sql, new { Id = id }, commandType: CommandType.Text);
        return MapToModel(record);
    }

    public async Task CreateSuggestion(VerseOfDaySuggestion suggestion)
    {
        const string sql = @"
            INSERT INTO VERSE_OF_DAY_SUGGESTIONS (READABLE_REFERENCE, SUGGESTER_USERNAME, CREATED_DATE, STATUS)
            VALUES (:ReadableReference, :SuggesterUsername, :CreatedDate, :Status)";

        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new
        {
            ReadableReference = suggestion.ReadableReference,
            SuggesterUsername = suggestion.SuggesterUsername,
            CreatedDate = DateTime.UtcNow,
            Status = "PENDING"
        }, commandType: CommandType.Text);
    }

    public async Task DeleteSuggestion(int id)
    {
        const string sql = @"DELETE FROM VERSE_OF_DAY_SUGGESTIONS WHERE ID = :Id";

        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { Id = id }, commandType: CommandType.Text);
    }

    public async Task ApproveSuggestion(int id)
    {
        const string sql = @"
            UPDATE VERSE_OF_DAY_SUGGESTIONS
            SET STATUS = 'APPROVED'
            WHERE ID = :Id";

        using IDbConnection conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { Id = id }, commandType: CommandType.Text);
    }

    private static VerseOfDaySuggestion? MapToModel(VerseOfDaySuggestionRecord? record)
    {
        if (record == null)
        {
            return null;
        }

        return new VerseOfDaySuggestion
        {
            Id = record.Id,
            ReadableReference = record.ReadableReference ?? string.Empty,
            SuggesterUsername = record.SuggesterUsername ?? string.Empty,
            CreatedDate = record.CreatedDate,
            Status = record.Status ?? "PENDING"
        };
    }

    private sealed class VerseOfDaySuggestionRecord
    {
        public int Id { get; set; }
        public string? ReadableReference { get; set; }
        public string? SuggesterUsername { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Status { get; set; }
    }
}



