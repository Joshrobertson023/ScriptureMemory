//using DataAccess.DBAccess;
//using DataAccess.Models;
//using Microsoft.Extensions.Configuration;
//using System.Data;
//using Dapper;
//using Oracle.ManagedDataAccess.Client;
//using DataAccess.DataInterfaces;

//namespace DataAccess.Data;

//public class HighlightData : IHighlightData
//{
//    private readonly IDBAccess _db;
//    private readonly IConfiguration _config;
//    private readonly string connectionString;

//    public HighlightData(IDBAccess db, IConfiguration config)
//    {
//        _db = db;
//        _config = config;
//        connectionString = _config.GetConnectionString("Default");
//    }

//    public async Task InsertHighlight(Highlight highlight)
//    {
//        var sql = @"INSERT INTO HIGHLIGHTS (USERNAME, VERSEREFERENCE, CREATEDDATE)
//                    VALUES (:Username, :VerseReference, SYSDATE)";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new
//        {
//            Username = highlight.Username,
//            VerseReference = highlight.VerseReference
//        }, commandType: CommandType.Text);
//    }

//    public async Task DeleteHighlight(string username, string verseReference)
//    {
//        var sql = @"DELETE FROM HIGHLIGHTS 
//                    WHERE USERNAME = :Username AND VERSEREFERENCE = :VerseReference";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new
//        {
//            Username = username,
//            VerseReference = verseReference
//        }, commandType: CommandType.Text);
//    }

//    public async Task<List<Highlight>> GetHighlightsByUsername(string username)
//    {
//        var sql = @"SELECT ID AS Id, USERNAME AS Username, VERSEREFERENCE AS VerseReference, 
//                           CREATEDDATE AS CreatedDate
//                    FROM HIGHLIGHTS
//                    WHERE USERNAME = :Username
//                    ORDER BY CREATEDDATE DESC";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var results = await conn.QueryAsync<Highlight>(sql, new { Username = username }, commandType: CommandType.Text);
//        return results.ToList();
//    }

//    public async Task<List<Highlight>> GetHighlightsByChapter(string username, string book, int chapter)
//    {
//        var sql = @"SELECT ID AS Id, USERNAME AS Username, VERSEREFERENCE AS VerseReference, 
//                           CREATEDDATE AS CreatedDate
//                    FROM HIGHLIGHTS
//                    WHERE USERNAME = :Username 
//                    AND VERSEREFERENCE LIKE :Pattern
//                    ORDER BY CREATEDDATE DESC";
//        var pattern = $"{book} {chapter}:%";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var results = await conn.QueryAsync<Highlight>(sql, new { Username = username, Pattern = pattern }, commandType: CommandType.Text);
//        return results.ToList();
//    }

//    public async Task<bool> IsHighlighted(string username, string verseReference)
//    {
//        var sql = @"SELECT COUNT(*) FROM HIGHLIGHTS 
//                    WHERE USERNAME = :Username AND VERSEREFERENCE = :VerseReference";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var count = await conn.QuerySingleAsync<int>(sql, new
//        {
//            Username = username,
//            VerseReference = verseReference
//        }, commandType: CommandType.Text);
//        return count > 0;
//    }
//}

