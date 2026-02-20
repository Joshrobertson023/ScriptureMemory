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
//public class PracticeSessionData : IPracticeSessionData
//{
//    private readonly IDBAccess _db;
//    private readonly IConfiguration _config;
//    private readonly string connectionString;

//    public PracticeSessionData(IDBAccess db, IConfiguration config)
//    {
//        _db = db;
//        _config = config;
//        connectionString = _config.GetConnectionString("Default");
//    }

//    public async Task<int> InsertPracticeSession(PracticeSession session)
//    {
//        var sql = @"
//            INSERT INTO USER_PRACTICE_SESSIONS (
//                USERNAME, 
//                USER_VERSE_ID, 
//                READABLE_REFERENCE, 
//                PRACTICE_STYLE, 
//                ACCURACY_PERCENT, 
//                STAGE_COUNT, 
//                STAGE_ACCURACIES, 
//                PRACTICE_DATE, 
//                CREATED_DATE
//            )
//            VALUES (
//                :Username,
//                :UserVerseId,
//                :ReadableReference,
//                :PracticeStyle,
//                :AccuracyPercent,
//                :StageCount,
//                :StageAccuracies,
//                :PracticeDate,
//                :CreatedDate
//            )
//            RETURNING SESSION_ID INTO :SessionId
//        ";

//        using IDbConnection conn = new OracleConnection(connectionString);
//        var parameters = new DynamicParameters();
//        parameters.Add("Username", session.Username);
//        parameters.Add("UserVerseId", session.UserVerseId);
//        parameters.Add("ReadableReference", session.ReadableReference);
//        parameters.Add("PracticeStyle", session.PracticeStyle);
//        parameters.Add("AccuracyPercent", session.AccuracyPercent);
//        parameters.Add("StageCount", session.StageCount);
//        parameters.Add("StageAccuracies", session.StageAccuracies);
//        parameters.Add("PracticeDate", session.PracticeDate);
//        parameters.Add("CreatedDate", session.CreatedDate);
//        parameters.Add("SessionId", dbType: DbType.Int32, direction: ParameterDirection.Output);

//        await conn.ExecuteAsync(sql, parameters, commandType: CommandType.Text);
        
//        return parameters.Get<int>("SessionId");
//    }

//    public async Task<List<PracticeSession>> GetPracticeSessionsByVerse(string username, string readableReference, int limit = 5)
//    {
//        var sql = @"
//            SELECT 
//                SESSION_ID AS SessionId,
//                USERNAME AS Username,
//                USER_VERSE_ID AS UserVerseId,
//                READABLE_REFERENCE AS ReadableReference,
//                PRACTICE_STYLE AS PracticeStyle,
//                ACCURACY_PERCENT AS AccuracyPercent,
//                STAGE_COUNT AS StageCount,
//                STAGE_ACCURACIES AS StageAccuracies,
//                PRACTICE_DATE AS PracticeDate,
//                CREATED_DATE AS CreatedDate
//            FROM USER_PRACTICE_SESSIONS
//            WHERE USERNAME = :Username 
//            AND READABLE_REFERENCE = :ReadableReference
//            ORDER BY PRACTICE_DATE DESC, CREATED_DATE DESC
//            FETCH FIRST :Limit ROWS ONLY
//        ";

//        using IDbConnection conn = new OracleConnection(connectionString);
//        var sessions = await conn.QueryAsync<PracticeSession>(sql, new 
//        { 
//            Username = username,
//            ReadableReference = readableReference,
//            Limit = limit
//        }, commandType: CommandType.Text);

//        return sessions.ToList();
//    }

//    public async Task<List<PracticeSession>> GetPracticeSessionsByUserVerseId(string username, int userVerseId, int limit = 5)
//    {
//        var sql = @"
//            SELECT 
//                SESSION_ID AS SessionId,
//                USERNAME AS Username,
//                USER_VERSE_ID AS UserVerseId,
//                READABLE_REFERENCE AS ReadableReference,
//                PRACTICE_STYLE AS PracticeStyle,
//                ACCURACY_PERCENT AS AccuracyPercent,
//                STAGE_COUNT AS StageCount,
//                STAGE_ACCURACIES AS StageAccuracies,
//                PRACTICE_DATE AS PracticeDate,
//                CREATED_DATE AS CreatedDate
//            FROM USER_PRACTICE_SESSIONS
//            WHERE USERNAME = :Username 
//            AND USER_VERSE_ID = :UserVerseId
//            ORDER BY PRACTICE_DATE DESC, CREATED_DATE DESC
//            FETCH FIRST :Limit ROWS ONLY
//        ";

//        using IDbConnection conn = new OracleConnection(connectionString);
//        var sessions = await conn.QueryAsync<PracticeSession>(sql, new 
//        { 
//            Username = username,
//            UserVerseId = userVerseId,
//            Limit = limit
//        }, commandType: CommandType.Text);

//        return sessions.ToList();
//    }
//}





















