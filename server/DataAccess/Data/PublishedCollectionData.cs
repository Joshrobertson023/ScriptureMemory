//using Dapper;
//using DataAccess.DataInterfaces;
//using DataAccess.DBAccess;
//using DataAccess.Models;
//using Microsoft.Extensions.Configuration;
//using Oracle.ManagedDataAccess.Client;
//using System.Data;

//namespace DataAccess.Data;

//public class PublishedCollectionData : IPublishedCollectionData
//{
//    private readonly IDBAccess _db;
//    private readonly IConfiguration _config;
//    private readonly string connectionString;

//    public PublishedCollectionData(IDBAccess db, IConfiguration config)
//    {
//        _db = db;
//        _config = config;
//        connectionString = _config.GetConnectionString("Default");
//    }

//    public async Task Publish(int collectionId, string? description)
//    {
//        const string sql = @"INSERT INTO PUBLISHED_COLLECTIONS (COLLECTION_ID, DESCRIPTION) VALUES (:collectionId, :description)";
//        await _db.SaveData<dynamic, dynamic>(sql, new { collectionId, description }, "Default");

        
//        const string update = @"UPDATE COLLECTIONS SET IS_PUBLISHED = 1 WHERE COLLECTION_ID = :collectionId";
//        await _db.SaveData<dynamic, dynamic>(update, new { collectionId }, "Default");
//    }

//    public async Task Unpublish(int collectionId)
//    {
//        const string sql = @"DELETE FROM PUBLISHED_COLLECTIONS WHERE COLLECTION_ID = :collectionId";
//        await _db.SaveData<dynamic, dynamic>(sql, new { collectionId }, "Default");
//    }

//    public async Task<PublishedCollection?> Get(int collectionId)
//    {
//        const string sql = @"SELECT COLLECTION_ID as Collection_Id, DESCRIPTION, PUBLISHED_AT as Published_At FROM PUBLISHED_COLLECTIONS WHERE COLLECTION_ID = :collectionId";
//        var rows = await _db.GetData<PublishedCollection, dynamic>(sql, new { collectionId }, "Default");
//        return rows.FirstOrDefault();
//    }

//    public async Task<List<PublishedCollection>> GetPublishedInCategory(int categoryId)
//    {
//        var sql = @"SELECT a.PUBLISHEDID,
//                           a.DESCRIPTION,
//                           a.NUMSAVES,
//                           a.TITLE,
//                           a.VERSEORDER,
//                           a.AUTHOR,
//                           a.PUBLISHEDDATE,
//                           b.NAME AS CATEGORYNAME
//                        FROM (
//                        SELECT p.PUBLISHED_ID AS PUBLISHEDID,
//                                p.DESCRIPTION,
//                                p.NUM_SAVES AS NUMSAVES,
//                                p.TITLE,
//                                p.VERSE_ORDER AS VERSEORDER,
//                                p.AUTHOR,
//                                p.PUBLISHED_DATE AS PUBLISHEDDATE,
//                                c.CATEGORY_ID AS CATEGORYID
//                        FROM PUBLISHED_COLLECTIONS p JOIN COLLECTION_CATEGORIES c ON p.PUBLISHED_ID = c.PUBLISHED_ID
//                        WHERE CATEGORY_ID = :CategoryId
//                    ) a JOIN CATEGORIES b ON a.CATEGORYID = b.CATEGORY_ID";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var results = await conn.QueryAsync<PublishedCollection>(sql, new { CategoryId = categoryId }, commandType: CommandType.Text);
//        return results.ToList();
//    }
//}


