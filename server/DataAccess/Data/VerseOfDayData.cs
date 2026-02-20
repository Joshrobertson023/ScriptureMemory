//using DataAccess.Models;
//using Dapper;
//using Microsoft.Extensions.Configuration;
//using Oracle.ManagedDataAccess.Client;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Threading.Tasks;
//using DataAccess.DataInterfaces;

//namespace DataAccess.Data;

//public class VerseOfDayData : IVerseOfDayData
//{
//    private readonly IConfiguration _config;
//    private readonly string connectionString;

//    public VerseOfDayData(IConfiguration config)
//    {
//        _config = config;
//        connectionString = _config.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");
//    }

//    public async Task<VerseOfDayInfo> GetLastUsedVerseOfDayData()
//    {
//        var sql = @"SELECT LAST_USED_VOD_ID AS LastUsedVodId, LAST_ROTATED_VOD_UTC AS LastUsedVodUtc FROM VERSE_OF_DAY_INFO";

//        using IDbConnection conn = new OracleConnection(connectionString);
//        var data = await conn.QueryAsync<VerseOfDayInfo>(sql, commandType: CommandType.Text);
//        return data.FirstOrDefault();
//    }

//    public async Task<VerseOfDay> GetNextPassageInSequence(int lastUsedId)
//    {
//        var getLastUsedSequenceSql = @"SELECT SEQUENCE FROM VERSE_OF_DAY WHERE ID = :LastUsedId;";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var lastUsedSequenceResults = await conn.QueryAsync(
//            getLastUsedSequenceSql,
//            new { LastUsedId = lastUsedId },
//            commandType: CommandType.Text);

//        int lastUsedSequence = lastUsedSequenceResults.FirstOrDefault();

//        var getNextVerseInSequenceSql = @"
//                SELECT ID, READABLEREFERENCE FROM VERSE_OF_DAY
//                WHERE SEQUENCE > :LastSquence
//                ORDER BY SEQUENCE ASC FETCH FIRST 1 ROWS ONLY";
//        using IDbConnection conn2 = new OracleConnection(connectionString);
//        var nextVerseInSequence = await conn2.QueryAsync<VerseOfDay>(
//            getNextVerseInSequenceSql,
//            new { LastSequence = lastUsedSequence },
//            commandType: CommandType.Text);

//        return nextVerseInSequence.FirstOrDefault();
//    }

//    public async Task SetTodayVod(VerseOfDayInfo info)
//    {
//        var sql = @"UPDATE VERSE_OF_DAY_INFO 
//                    SET LAST_USED_VOD_ID = :newId, LAST_ROTATED_VOD_UTC = :newDate";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql,
//            new { newId = info.LastUsedVodId, newDate = info.LastUsedVodUtc },
//            commandType: CommandType.Text);
//    }

//    public async Task<VerseOfDay?> GetCurrentVerseOfDay()
//    {
//        var info = await GetLastUsedVerseOfDayData();
//        if (info == null || info.LastUsedVodId == 0)
//        {
//            return null;
//        }

//        var sql = @"SELECT ID, READABLEREFERENCE FROM VERSE_OF_DAY WHERE ID = :Id";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var result = await conn.QueryFirstOrDefaultAsync<VerseOfDay>(sql, 
//            new { Id = info.LastUsedVodId }, 
//            commandType: CommandType.Text);
//        return result;
//    }

//    public async Task CreateVerseOfDay(VerseOfDay verseOfDay)
//    {
//        using IDbConnection conn = new OracleConnection(connectionString);
        
//        var getMaxSequenceSql = @"SELECT NVL(MAX(SEQUENCE), 0) FROM VERSE_OF_DAY";
//        var maxSequenceResult = await conn.QueryFirstOrDefaultAsync<int>(getMaxSequenceSql, commandType: CommandType.Text);
//        int nextSequence = maxSequenceResult + 1;

//        var insertSql = @"INSERT INTO VERSE_OF_DAY (READABLEREFERENCE, SEQUENCE) 
//                          VALUES (:ReadableReference, :Sequence)";
//        await conn.ExecuteAsync(insertSql,
//            new { ReadableReference = verseOfDay.ReadableReference, Sequence = nextSequence },
//            commandType: CommandType.Text);
//    }

//    public async Task DeleteVerseOfDay(int id)
//    {
//        var sql = @"DELETE FROM VERSE_OF_DAY WHERE ID = :Id";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new { Id = id }, commandType: CommandType.Text);
//    }

//    public async Task<List<VerseOfDay>> GetUpcomingVerseOfDay()
//    {
//        var sql = @"SELECT ID, READABLEREFERENCE, SEQUENCE FROM VERSE_OF_DAY ORDER BY SEQUENCE ASC";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var results = await conn.QueryAsync<VerseOfDay>(sql, commandType: CommandType.Text);
//        return results.ToList();
//    }

//    public async Task ResetQueueToBeginning()
//    {
//        using IDbConnection conn = new OracleConnection(connectionString);
        
//        var getFirstVerseSql = @"
//            SELECT ID, READABLEREFERENCE, SEQUENCE 
//            FROM VERSE_OF_DAY 
//            ORDER BY SEQUENCE ASC 
//            FETCH FIRST 1 ROWS ONLY";
        
//        var firstVerse = await conn.QueryFirstOrDefaultAsync<VerseOfDay>(getFirstVerseSql, commandType: CommandType.Text);
        
//        if (firstVerse == null)
//        {
//            throw new InvalidOperationException("No verses found in the queue to reset to.");
//        }

//        var updateSql = @"UPDATE VERSE_OF_DAY_INFO 
//                         SET LAST_USED_VOD_ID = :newId, LAST_ROTATED_VOD_UTC = :newDate";
        
//        await conn.ExecuteAsync(updateSql,
//            new { newId = firstVerse.Id, newDate = DateTime.UtcNow },
//            commandType: CommandType.Text);
//    }
//}