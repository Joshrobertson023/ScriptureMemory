//using DataAccess.DBAccess;
//using DataAccess.Models;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Threading.Tasks;
//using Dapper;
//using Oracle.ManagedDataAccess.Client;
//using DataAccess.DataInterfaces;

//namespace DataAccess.Data;
//public class PracticeLogData : IPracticeLogData
//{
//    private readonly IDBAccess _db;
//    private readonly IConfiguration _config;
//    private readonly string connectionString;

//    public PracticeLogData(IDBAccess db, IConfiguration config)
//    {
//        _db = db;
//        _config = config;
//        connectionString = _config.GetConnectionString("Default");
//    }

//    public async Task<int> RecordPractice(string username)
//    {
        
//        var today = DateTime.UtcNow.ToUniversalTime().Date;
//        var sql = @"
//            SELECT COUNT(*) 
//            FROM USER_PRACTICE_LOG 
//            WHERE USERNAME = :Username 
//            AND TRUNC(PRACTICE_DATE) = TRUNC(:PracticeDate)
//        ";
        
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var count = await conn.QueryFirstOrDefaultAsync<int>(sql, new 
//        { 
//            Username = username,
//            PracticeDate = today
//        }, commandType: CommandType.Text);

        
//        if (count > 0)
//        {
//            return await GetCurrentStreakLength(username);
//        }

        
//        var insertSql = @"
//            INSERT INTO USER_PRACTICE_LOG (USERNAME, PRACTICE_DATE, LAST_UPDATED)
//            VALUES (:Username, :PracticeDate, :LastUpdated)
//        ";
        
//        await conn.ExecuteAsync(insertSql, new 
//        { 
//            Username = username,
//            PracticeDate = today,
//            LastUpdated = DateTime.UtcNow.ToUniversalTime()
//        }, commandType: CommandType.Text);

//        return await GetCurrentStreakLength(username);
//    }

//    public async Task<int> GetCurrentStreakLength(string username)
//    {
//        var sql = @"
//            WITH practice_dates AS (
//                SELECT DISTINCT TRUNC(PRACTICE_DATE) as practice_date
//                FROM USER_PRACTICE_LOG
//                WHERE USERNAME = :Username
//            ),
//            streak_groups AS (
//                SELECT 
//                    practice_date,
//                    practice_date - ROW_NUMBER() OVER (ORDER BY practice_date) as grp,
//                    ROW_NUMBER() OVER (ORDER BY practice_date DESC) as rn
//                FROM practice_dates
//            ),
//            recent_streaks AS (
//                SELECT 
//                    grp,
//                    COUNT(*) as streak_length,
//                    MAX(CASE WHEN rn = 1 THEN practice_date END) as most_recent_date
//                FROM streak_groups
//                WHERE practice_date >= TRUNC(SYSDATE) - 100
//                GROUP BY grp
//            )
//            SELECT COALESCE(MAX(streak_length), 0) as streak
//            FROM recent_streaks
//            WHERE most_recent_date >= TRUNC(SYSDATE) - 1
//        ";
        
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var streak = await conn.QueryFirstOrDefaultAsync<int>(sql, new 
//        { 
//            Username = username
//        }, commandType: CommandType.Text);

//        return streak;
//    }

//    public async Task<List<DateTime>> GetPracticeHistory(string username)
//    {
//        var sql = @"
//            SELECT DISTINCT TRUNC(PRACTICE_DATE) as practice_date
//            FROM USER_PRACTICE_LOG
//            WHERE USERNAME = :Username
//            ORDER BY TRUNC(PRACTICE_DATE) DESC
//        ";
        
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var dates = await conn.QueryAsync<DateTime>(sql, new 
//        { 
//            Username = username
//        }, commandType: CommandType.Text);

//        return dates.ToList();
//    }
//}

