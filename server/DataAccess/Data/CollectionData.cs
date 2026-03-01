using Dapper;
using DataAccess.DataInterfaces;
using DataAccess.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ScriptureMemoryLibrary;
using DataAccess.Requests;
using static ScriptureMemoryLibrary.Enums;

namespace DataAccess.Data;

public class CollectionData : ICollectionData
{
    private readonly IDbConnection conn;

    public CollectionData(IDbConnection connection)
    {
        conn = connection;
    }

    /// <summary>
    /// Insert a collection
    /// </summary>
    /// <param name="collection"></param>
    /// <returns>int</returns>
    public async Task<int> InsertCollection(Collection collection)
    {
        var sql = @"INSERT INTO COLLECTIONS
                (USER_ID, ORDER_POSITION, VISIBILITY, IS_FAVORITES, DESCRIPTION, PROGRESS_PERCENT, TITLE, DATE_CREATED)
                VALUES
                (:UserId, :OrderPosition, :Visibility, :IsFavorites, :Description, :ProgressPercent, :Title, :DateCreated)
                RETURNING COLLECTION_ID INTO :NewId";

        var parameters = new DynamicParameters();

        parameters.Add("UserId", collection.UserId);
        parameters.Add("OrderPosition", collection.OrderPosition);
        parameters.Add("Visibility", collection.Visibility);
        parameters.Add("IsFavorites", collection.IsFavorites == true ? 1 : 0);
        parameters.Add("Description", collection.Description);
        parameters.Add("ProgressPercent", collection.ProgressPercent);
        parameters.Add("Title", collection.Title);
        parameters.Add("DateCreated", DateTime.UtcNow);

        parameters.Add("NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await conn.ExecuteAsync(sql, parameters);

        return parameters.Get<int>("NewId");
    }

    public async Task SaveCollection(SaveCollectionRequest request)
    {
        var sql = @"INSERT INTO SAVED_COLLECTIONS
                    (USER_ID, PUBLISHED_ID, DATE_SAVED, ORDER_POSITION, VISIBILITY)
                    VALUES
                    (:UserId, :PublishedId, :DateSaved, :OrderPosition, :Visibility)";

        await conn.ExecuteAsync(sql, new
        {
            UserId = request.UserId,
            PublishedId = request.PublishedId,
            DateSaved = request.DateSaved,
            OrderPosition = request.OrderPosition,
            Visibility = CollectionVisibility.Private,
        });
    }

    /// <summary>
    /// Get the next order position for creating a new collection
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>int</returns>
    public async Task<int> GetNextOrderPosition(int userId) // TODO: Join with SAVED_COLLECTIONS
    {
        var sql = @"SELECT NVL(MAX(ORDER_POSITION), 0) + 1 FROM COLLECTIONS WHERE USER_ID = :UserId";

        var nextPosition = await conn.ExecuteScalarAsync<int>(sql, new { UserId = userId });

        return nextPosition;
    }

    /// <summary>
    /// Check whether a favorites collection already exists for a user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>bool</returns>
    public async Task<bool> FavoritesExists(int userId)
    {
        var sql = @"SELECT COUNT(*) FROM COLLECTIONS WHERE USER_ID = :UserId AND IS_FAVORITES = 1";

        var count = await conn.ExecuteScalarAsync<int>(sql, new { UserId = userId });

        return count > 0;
    }

    /// <summary>
    /// Check whether a user has already saved this collection
    /// </summary>
    /// <param name="collection"></param>
    /// <returns>bool</returns>
    public async Task<bool> SavedFromPublishedExists(int publishedId, int userId)
    {
        var sql = @"SELECT COUNT(*) FROM SAVED_COLLECTIONS 
                    WHERE USER_ID = :UserId 
                    AND PUBLISHED_ID = :PublishedId";
        var count = await conn.ExecuteScalarAsync<int>(sql, new { UserId = userId, PublishedId = publishedId });

        return count > 0;
    }

    /// <summary>
    /// Gets a list of empty user collections
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<Collection>> GetUserCreatedCollections(int userId)
    {
        var sql = @"SELECT 
                        c.COLLECTION_ID AS CollectionId,
                        c.USER_ID AS UserId,
                        c.TITLE,
                        c.VISIBILITY,
                        c.DATE_CREATED AS DateCreated,
                        c.ORDER_POSITION AS OrderPosition,
                        c.IS_FAVORITES AS IsFavorites,
                        c.DESCRIPTION,
                        c.PROGRESS_PERCENT AS ProgressPercent,
                        COUNT(up.COLLECTION_ID) AS NumPassages
                    FROM COLLECTIONS c
                    LEFT JOIN USER_PASSAGES up 
                        ON up.COLLECTION_ID = c.COLLECTION_ID
                    WHERE c.USER_ID = :UserId 
                      AND c.PUBLISHED_ID IS NULL
                    GROUP BY 
                        c.COLLECTION_ID,
                        c.USER_ID,
                        c.TITLE,
                        c.VISIBILITY,
                        c.DATE_CREATED,
                        c.ORDER_POSITION,
                        c.IS_FAVORITES,
                        c.DESCRIPTION,
                        c.PROGRESS_PERCENT
                    ORDER BY c.ORDER_POSITION";
        
        var collections = await conn.QueryAsync<Collection>(sql, new { UserId = userId });
        
        return collections.ToList();
    }

    public async Task<List<Collection>> GetUserSavedCollections(int userId)
    {
        var sql = @"SELECT 
                        c.COLLECTION_ID      AS CollectionId,
                        c.USER_ID            AS UserId,
                        c.TITLE,
                        c.DATE_CREATED       AS DateCreated,
                        c.IS_FAVORITES       AS IsFavorites,
                        c.DESCRIPTION,
                        c.PUBLISHED_ID       AS PublishedId,
                        c.AUTHOR_ID          AS AuthorId,
                        c.PROGRESS_PERCENT   AS ProgressPercent,
                        sc.VISIBILITY,
                        sc.ORDER_POSITION    AS OrderPosition,
                        sc.DATE_SAVED        AS DateSaved,
                        u.USERNAME           AS Author,
                        COUNT(up.COLLECTION_ID) AS NumPassages
                    FROM COLLECTIONS c
                    INNER JOIN SAVED_COLLECTIONS sc
                        ON  sc.PUBLISHED_ID = c.PUBLISHED_ID
                        AND sc.USER_ID      = c.USER_ID
                    INNER JOIN PUBLISHED_COLLECTIONS pc
                        ON  pc.PUBLISHED_ID = c.PUBLISHED_ID
                    LEFT JOIN USERS u
                        ON  u.ID = pc.AUTHOR_ID
                    LEFT JOIN USER_PASSAGES up
                        ON  up.COLLECTION_ID = c.COLLECTION_ID
                    WHERE c.USER_ID = :UserId
                      AND c.PUBLISHED_ID IS NOT NULL
                    GROUP BY
                        c.COLLECTION_ID,
                        c.USER_ID,
                        c.TITLE,
                        c.DATE_CREATED,
                        c.IS_FAVORITES,
                        c.DESCRIPTION,
                        c.PUBLISHED_ID,
                        c.AUTHOR_ID,
                        c.PROGRESS_PERCENT,
                        sc.VISIBILITY,
                        sc.ORDER_POSITION,
                        sc.DATE_SAVED,
                        u.USERNAME
                    ORDER BY sc.ORDER_POSITION";

        var collections = await conn.QueryAsync<Collection>(sql, new { UserId = userId });

        return collections.ToList();
    }



    // -- Collection Notes -----------------------------------------------------



    public async Task<int> InsertCollectionNote(CollectionNote note)
    {
        var sql = @"INSERT INTO COLLECTION_NOTES
                    (COLLECTION_ID, TEXT, ORDER_POSITION)
                    VALUES
                    (:CollectionId, :Text, :OrderPosition)
                    RETURNING ID INTO :NewId";

        var parameters = new DynamicParameters();

        parameters.Add("CollectionId", note.CollectionId);
        parameters.Add("Text", note.Text);
        parameters.Add("OrderPosition", note.OrderPosition);

        parameters.Add("NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await conn.ExecuteAsync(sql, parameters);

        return parameters.Get<int>("NewId");
    }

    public async Task<List<CollectionNote>> GetCollectionNotes(int collectionId)
    {
        var sql = @"SELECT ID AS Id, COLLECTION_ID AS CollectionId, TEXT AS Text, ORDER_POSITION AS OrderPosition
                    FROM COLLECTION_NOTES
                    WHERE COLLECTION_ID = :CollectionId";

        var notes = await conn.QueryAsync<CollectionNote>(sql, new { CollectionId = collectionId });
        
        return notes?.ToList() ?? new List<CollectionNote>();
    }
}

//    public async Task AssignNewPublishedCollectionCategories(int latestId, List<int> categoryIds)
//    {
//        var sql = @"INSERT INTO COLLECTION_CATEGORIES
//                    (CATEGORY_ID, PUBLISHED_ID)
//                    VALUES 
//                    (:CategoryId, :PublishedId)";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        foreach (var cat in categoryIds)
//        {
//            await conn.ExecuteAsync(sql, new
//            {
//                CategoryId = cat,
//                PublishedId = latestId
//            },
//            commandType: CommandType.Text);
//        }
//    }

//    public async Task UpdateCollection(Collection collection)
//    {
//        var sql = @"UPDATE COLLECTIONS SET
//                     TITLE = :Title,
//                     VISIBILITY = :Visibility,
//                     VERSE_ORDER = :VerseOrder
//                     WHERE COLLECTION_ID = :CollectionId";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new
//        {
//            Title = collection.Title,
//            Visibility = collection.Visibility,
//            VerseOrder = collection.VerseOrder,
//            CollectionId = collection.CollectionId
//        }, commandType: CommandType.Text);
//    }

//    public async Task SetUserSavedCollection(int id)
//    {
//        var sql = $@"UPDATE COLLECTIONS SET NUM_SAVES = NUM_SAVES + 1 WHERE COLLECTION_ID = :collectionId";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new { collectionId = id }, commandType: CommandType.Text);
//    }

//    public async Task IncrementPublishedCollectionSaves(int publishedId)
//    {
//        const string sql = @"UPDATE PUBLISHED_COLLECTIONS
//                             SET NUM_SAVES = NUM_SAVES + 1
//                             WHERE PUBLISHED_ID = :PublishedId";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new { PublishedId = publishedId }, commandType: CommandType.Text);
//    }

//    public async Task<IEnumerable<Collection>> GetUserPublicCollections(string username)
//    {
//        Debug.WriteLine("\n\nGetting public collections");
//        var sql = @"
//            SELECT
//                c.COLLECTION_ID AS CollectionId,
//                c.AUTHOR_USERNAME AS AuthorUsername,
//                c.USERNAME AS Username,
//                c.TITLE AS Title,
//                c.VISIBILITY AS Visibility,
//                c.VERSE_ORDER AS VerseOrder,
//                c.DATE_CREATED AS DateCreated,
//                COUNT(DISTINCT uv.USER_VERSE_ID) AS NumVerses
//            FROM COLLECTIONS c
//            LEFT JOIN USER_VERSES uv ON c.COLLECTION_ID = uv.COLLECTION_ID
//            WHERE c.USERNAME = :Username
//                AND c.VISIBILITY = 'public'
//            GROUP BY c.COLLECTION_ID, c.AUTHOR_USERNAME, c.USERNAME, c.TITLE, c.VISIBILITY, c.VERSE_ORDER, c.DATE_CREATED
//            ORDER BY c.COLLECTION_ID DESC";

//        using IDbConnection conn = new OracleConnection(connectionString);
//        var collections = await conn.QueryAsync<Collection>(sql, new { Username = username }, commandType: CommandType.Text);
//        var collectionList = collections.ToList();
//        await PopulateCollectionNotes(collectionList);
//        return collectionList;
//    }

//    public async Task<IEnumerable<Collection>> GetUserFriendCollections(string username, string viewerUsername)
//    {
//        Debug.WriteLine("\n\nGetting friends collections");
//        var checkFriendSql = @"SELECT COUNT(*) FROM USER_RELATIONSHIPS 
//                                WHERE ((USERNAME_1 = :Username1 AND USERNAME_2 = :Username2) OR (USERNAME_1 = :Username2 AND USERNAME_2 = :Username1))
//                                AND TYPE = 1";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var isFriend = await conn.QueryFirstOrDefaultAsync<int>(checkFriendSql, new { Username1 = username, Username2 = viewerUsername }, commandType: CommandType.Text);

//        var allCollections = await GetUserCollectionsWithUserVerses(username);

//        var allowedVisibilities = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
//        {
//            "public"
//        };

//        if (isFriend > 0)
//        {
//            allowedVisibilities.Add("friends");
//        }

//        var filtered = allCollections
//            .Where(collection => !string.IsNullOrWhiteSpace(collection.Visibility) &&
//                                 allowedVisibilities.Contains(collection.Visibility.Trim()))
//            .ToList();

//        return filtered;
//    }

//    public async Task<IEnumerable<PublishedCollection>> GetPendingCollections()
//    {
//        const string sql = @"
//            SELECT
//                p.PUBLISHED_ID AS PublishedId,
//                p.DESCRIPTION AS Description,
//                p.AUTHOR AS Author,
//                p.TITLE AS Title,
//                p.NUM_SAVES AS NumSaves,
//                p.VERSE_ORDER AS VerseOrder,
//                p.COLLECTION_ID AS CollectionId,
//                p.DATE_PUBLISHED AS PublishedDate,
//                p.STATUS AS Status,
//                cc.CATEGORY_ID AS CategoryId,
//                uv.USER_VERSE_ID AS Id,
//                uv.USERNAME AS Username,
//                uv.READABLE_REFERENCE AS ReadableReference,
//                uv.LAST_PRACTICED AS LastPracticed,
//                uv.PROGRESS_PERCENT AS ProgressPercent,
//                uv.TIMES_MEMORIZED AS TimesMemorized,
//                uv.DATE_SAVED AS DateAdded,
//                uv.COLLECTION_ID AS CollectionId,
//                uv.IN_PUBLISHED AS INPUBLISHED
//            FROM PUBLISHED_COLLECTIONS p
//            LEFT JOIN COLLECTION_CATEGORIES cc
//                ON p.PUBLISHED_ID = cc.PUBLISHED_ID
//            LEFT JOIN USER_VERSES uv
//                ON p.PUBLISHED_ID = uv.COLLECTION_ID
//                AND uv.USERNAME = p.AUTHOR
//                AND uv.IN_PUBLISHED = 1
//            WHERE p.STATUS = 'PENDING'
//            ORDER BY p.DATE_PUBLISHED ASC, p.PUBLISHED_ID ASC";

//        return await QueryPublishedCollectionsAsync(sql, new { });
//    }

//    public async Task ApprovePublishedCollection(int publishedId)
//    {
//        const string sql = @"
//            UPDATE PUBLISHED_COLLECTIONS
//            SET STATUS = 'APPROVED',
//                DATE_PUBLISHED = SYSDATE
//            WHERE PUBLISHED_ID = :PublishedId";

//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new { PublishedId = publishedId }, commandType: CommandType.Text);
//    }

//    public async Task RejectPublishedCollection(int publishedId)
//    {
//        const string sql = @"
//            UPDATE PUBLISHED_COLLECTIONS
//            SET STATUS = 'REJECTED'
//            WHERE PUBLISHED_ID = :PublishedId";

//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new { PublishedId = publishedId }, commandType: CommandType.Text);
//    }
//}

