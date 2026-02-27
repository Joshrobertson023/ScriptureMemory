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

namespace DataAccess.Data;

public class CollectionData : ICollectionData
{
    private readonly IConfiguration _config;
    private readonly IPracticeSessionData _practiceSessionData;
    private readonly string connectionString;

    public CollectionData(IConfiguration config, IPracticeSessionData practiceSessionData)
    {
        _config = config;
        _practiceSessionData = practiceSessionData;
        connectionString = _config.GetConnectionString("Default")!;
    }

    /// <summary>
    /// Insert a collection
    /// </summary>
    /// <param name="collection"></param>
    /// <returns>int</returns>
    public async Task<int> InsertCollection(Collection collection)
    {
        var sql = @"INSERT INTO COLLECTIONS
                (USER_ID, ORDER_POSITION, VISIBILITY, IS_FAVORITES, DESCRIPTION, PROGRESS_PERCENT, TITLE, DATE_CREATED, PUBLISHED_ID, AUTHOR_ID)
                VALUES
                (:UserId, :OrderPosition, :Visibility, :IsFavorites, :Description, :ProgressPercent, :Title, :DateCreated, :PublishedId, :AuthorId)
                RETURNING COLLECTION_ID INTO :NewId";

        await using var conn = new OracleConnection(connectionString);
        await conn.OpenAsync();

        var parameters = new DynamicParameters();

        parameters.Add("UserId", collection.UserId);
        parameters.Add("OrderPosition", collection.OrderPosition);
        parameters.Add("Visibility", collection.Visibility);
        parameters.Add("IsFavorites", collection.IsFavorites);
        parameters.Add("Description", collection.Description);
        parameters.Add("ProgressPercent", collection.ProgressPercent);
        parameters.Add("Title", collection.Title);
        parameters.Add("DateCreated", DateTime.UtcNow);

        parameters.Add("NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await conn.ExecuteAsync(sql, parameters);

        return parameters.Get<int>("NewId");
    }

    /// <summary>
    /// Get the next order position for creating a new collection
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>int</returns>
    public async Task<int> GetNextOrderPosition(int userId)
    {
        var sql = @"SELECT NVL(MAX(ORDER_POSITION), 0) + 1 FROM COLLECTIONS WHERE USER_ID = :UserId";

        await using var conn = new OracleConnection(connectionString);
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

        await using var conn = new OracleConnection(connectionString);
        var count = await conn.ExecuteScalarAsync<int>(sql, new { UserId = userId });

        return count > 0;
    }

    /// <summary>
    /// Check whether a user has already saved this collection
    /// </summary>
    /// <param name="collection"></param>
    /// <returns>bool</returns>
    public async Task<bool> SavedFromPublishedExists(Collection collection)
    {
        var sql = @"SELECT COUNT(*) FROM COLLECTIONS 
                    WHERE USER_ID = :UserId 
                    AND PUBLISHED_ID = :PublishedId";
        await using var conn = new OracleConnection(connectionString);
        var count = await conn.ExecuteScalarAsync<int>(sql, new { UserId = collection.UserId, PublishedId = collection.PublishedId });

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
                        c.PUBLISHED_ID AS PublishedId,
                        c.AUTHOR_ID AS AuthorId,
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
                        c.PUBLISHED_ID,
                        c.AUTHOR_ID,
                        c.PROGRESS_PERCENT
                    ORDER BY c.ORDER_POSITION";
        
        await using var conn = new OracleConnection(connectionString);
        var collections = await conn.QueryAsync<Collection>(sql, new { UserId = userId });
        
        return collections.ToList();
    }

    //public async Task<List<Collection>> GetUserSavedCollections(int userId)
    //{
    //    var sql = @"SELECT 
    //                    c.PUBLISHED_ID AS CollectionId,
    //                    c.USER_ID AS UserId,
    //                    c.TITLE,
    //                    c.VISIBILITY,
    //                    c.DATE_CREATED AS DateCreated,
    //                    c.ORDER_POSITION AS OrderPosition,
    //                    c.IS_FAVORITES AS IsFavorites,
    //                    c.DESCRIPTION,
    //                    c.PUBLISHED_ID AS PublishedId,
    //                    c.AUTHOR_ID AS AuthorId,
    //                    c.PROGRESS_PERCENT AS ProgressPercent,
    //                    COUNT(up.COLLECTION_ID) AS NumPassages
    //                FROM COLLECTIONS c
    //                LEFT JOIN USER_PASSAGES up 
    //                    ON up.COLLECTION_ID = c.COLLECTION_ID
    //                WHERE c.USER_ID = :UserId 
    //                  AND c.PUBLISHED_ID IS NULL
    //                GROUP BY 
    //                    c.COLLECTION_ID,
    //                    c.USER_ID,
    //                    c.TITLE,
    //                    c.VISIBILITY,
    //                    c.DATE_CREATED,
    //                    c.ORDER_POSITION,
    //                    c.IS_FAVORITES,
    //                    c.DESCRIPTION,
    //                    c.PUBLISHED_ID,
    //                    c.AUTHOR_ID,
    //                    c.PROGRESS_PERCENT
    //                ORDER BY c.ORDER_POSITION";

    //    await using var conn = new OracleConnection(connectionString);
    //    var collections = await conn.QueryAsync<Collection>(sql, new { UserId = userId });

    //    return collections.ToList();
    //}

    public async Task<Collection> GetUserCollection



    // -- Published Collections -----------------------------------------------------


    /// <summary>
    /// Gets the author id for a published collection
    /// </summary>
    /// <param name="publishedId"></param>
    /// <returns>int</returns>
    public async Task<int> GetAuthorId(int publishedId)
    {
        var sql = @"SELECT AUTHOR_ID FROM PUBLISHED_COLLECTIONS WHERE PUBLISHED_ID = :PublishedId";

        await using var conn = new OracleConnection(connectionString);
        var authorId = await conn.ExecuteScalarAsync<int>(sql, new { PublishedId = publishedId });

        return authorId;
    }

    /// <summary>
    /// Gets the author's username of a published collection
    /// </summary>
    /// <param name="publishedId"></param>
    /// <returns></returns>
    public async Task<string> GetAuthorName(int publishedId)
    {
        var sql = @"SELECT u.USERNAME FROM PUBLISHED_COLLECTIONS c
                    INNER JOIN USERS u ON c.AUTHOR_ID = u.USER_ID
                    WHERE c.PUBLISHED_ID = :PublishedId";

        await using var conn = new OracleConnection(connectionString);
        var authorName = await conn.ExecuteScalarAsync<string>(sql, new { PublishedId = publishedId });

        return authorName ?? "";
    }
}

//    public async Task<int> PublishCollection(Collection collectionBeingPublished, string description)
//    {
//        if (collectionBeingPublished == null)
//        {
//            throw new ArgumentNullException(nameof(collectionBeingPublished), "Collection cannot be null.");
//        }

//        if (string.IsNullOrWhiteSpace(collectionBeingPublished.Title))
//        {
//            throw new ArgumentException("Collection title cannot be null or empty.", nameof(collectionBeingPublished));
//        }

//        if (string.IsNullOrWhiteSpace(collectionBeingPublished.AuthorUsername))
//        {
//            throw new ArgumentException("Collection author username cannot be null or empty.", nameof(collectionBeingPublished));
//        }



//        using IDbConnection conn = new OracleConnection(connectionString);


//        // Handle CollectionId - use NULL if it's 0 or not set
//        int? collectionIdValue = collectionBeingPublished.CollectionId > 0 
//            ? collectionBeingPublished.CollectionId 
//            : (int?)null;

//        await conn.ExecuteAsync(@"INSERT INTO PUBLISHED_COLLECTIONS
//                    (DESCRIPTION, TITLE, VERSE_ORDER, AUTHOR, DATE_PUBLISHED, STATUS, COLLECTION_ID)
//                    VALUES
//                    (:Description, :Title, :VerseOrder, :Author, SYSDATE, 'PENDING', :CollectionId)", new
//        {
//            Description = description,
//            Title = collectionBeingPublished.Title,
//            VerseOrder = collectionBeingPublished.VerseOrder ?? string.Empty,
//            Author = collectionBeingPublished.AuthorUsername,
//            CollectionId = collectionIdValue
//        },
//        commandType: CommandType.Text);



//        var getPublishedIdSql = @"SELECT PUBLISHED_ID FROM PUBLISHED_COLLECTIONS 
//                                  WHERE AUTHOR = :Author 
//                                    AND TITLE = :Title
//                                    AND NVL(VERSE_ORDER, '') = NVL(:VerseOrder, '')
//                                    AND STATUS = 'PENDING'
//                                  ORDER BY DATE_PUBLISHED DESC 
//                                  FETCH FIRST 1 ROWS ONLY";

//        var publishedId = await conn.QueryFirstOrDefaultAsync<int>(
//            getPublishedIdSql, 
//            new 
//            { 
//                Author = collectionBeingPublished.AuthorUsername,
//                Title = collectionBeingPublished.Title,
//                VerseOrder = collectionBeingPublished.VerseOrder ?? string.Empty
//            }, 
//            commandType: CommandType.Text);

//        // Save notes for published collection
//        if (publishedId > 0 && collectionBeingPublished.Notes != null && collectionBeingPublished.Notes.Count > 0)
//        {
//            var insertNoteSql = @"INSERT INTO PUBLISHED_COLLECTION_NOTES (ID, PUBLISHED_ID, TEXT)
//                                 VALUES (:Id, :PublishedId, :Text)";
//            foreach (var note in collectionBeingPublished.Notes)
//            {
//                await conn.ExecuteAsync(insertNoteSql, new
//                {
//                    Id = note.Id,
//                    PublishedId = publishedId,
//                    Text = note.Text
//                }, commandType: CommandType.Text);
//            }
//        }

//        return publishedId;
//    }

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

//    public async Task DeleteCollection(int id, IUserPassageData uvData)
//    {
//        await uvData.DeleteUserVersesByCollection(id);
//        // Delete notes for this collection
//        var deleteNotesSql = @"DELETE FROM COLLECTION_NOTES WHERE COLLECTION_ID = :CollectionId";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(deleteNotesSql, new { CollectionId = id }, commandType: CommandType.Text);
//        var sql = $@"DELETE FROM COLLECTIONS WHERE collection_id = :Id";
//        await conn.ExecuteAsync(sql, new { Id = id }, commandType: CommandType.Text);
//    }

//    public async Task DeleteCollectionsByUsername(string username, IUserPassageData uvData)
//    {
//        if (string.IsNullOrWhiteSpace(username))
//        {
//            return;
//        }

//        var getCollectionsSql = @"SELECT COLLECTION_ID FROM COLLECTIONS WHERE USERNAME = :Username";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var collectionIds = await conn.QueryAsync<int>(getCollectionsSql, new { Username = username }, commandType: CommandType.Text);

//        foreach (var collectionId in collectionIds)
//        {
//            await uvData.DeleteUserVersesByCollection(collectionId);
//            // Delete notes for this collection
//            var deleteNotesSql = @"DELETE FROM COLLECTION_NOTES WHERE COLLECTION_ID = :CollectionId";
//            await conn.ExecuteAsync(deleteNotesSql, new { CollectionId = collectionId }, commandType: CommandType.Text);
//        }

//        var sql = @"DELETE FROM COLLECTIONS WHERE USERNAME = :Username";
//        await conn.ExecuteAsync(sql, new { Username = username }, commandType: CommandType.Text);
//    }

//    public async Task<int> GetMostRecentCollection(string username)
//    {
//        var sql = @"SELECT MAX(COLLECTION_ID) FROM COLLECTIONS WHERE USERNAME = :username";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var id = await conn.QueryFirstOrDefaultAsync<int>(
//            sql, new { username = username }, commandType: CommandType.Text);
//        return id;
//    }

//    public async Task<int> GetMostRecentPublishedCollection(string username)
//    {
//        var sql = @"SELECT MAX(PUBLISHED_ID) FROM PUBLISHED_COLLECTIONS WHERE AUTHOR = :username";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var id = await conn.QueryFirstOrDefaultAsync<int>(
//            sql, new { username = username }, commandType: CommandType.Text);
//        return id;
//    }

//    public async Task<Collection?> GetCollection(Collection collection)
//    {
//        if (collection.UserVerses == null)
//            collection.UserVerses = new List<UserPassage>();

//        var userVerses = collection.UserVerses;
//        List<string> verseReferences = new();

//        foreach (var userVerse in userVerses)
//        {
//            if (string.IsNullOrEmpty(userVerse.ReadableReference)) continue;

//            List<string> individualVerseReferences = ReferenceParse.GetReferencesFromVersesInReference(userVerse.ReadableReference);
//            verseReferences.AddRange(individualVerseReferences);
//        }

//        List<string> uniqueReferences = verseReferences.Distinct().ToList();

//        if (uniqueReferences.Count == 0)
//            return collection;

//        var quotedReferences = uniqueReferences.Select(r => $"'{r.Replace("'", "''")}'");
//        var referencesList = string.Join(",", quotedReferences);

//        Debug.WriteLine($"\n\n\n\nSQL IN CLAUSE: {referencesList}\n\n\n\n");

//        var sql = $@"SELECT * FROM verses WHERE verse_reference in ({referencesList})";

//        using IDbConnection conn = new OracleConnection(connectionString);
//        var result = await conn.QueryAsync<Verse>(
//            sql,
//            commandType: CommandType.Text);

//        var resultVerses = result.ToList();

//        foreach (var uv in collection.UserVerses)
//        {
//            uv.Verses.Clear();
//            List<string> individualVerseReferences = ReferenceParse.GetReferencesFromVersesInReference(uv.ReadableReference);

//            foreach (var verseReference in individualVerseReferences)
//            {
//                var newVerse = resultVerses.FirstOrDefault(v => v.verse_reference == verseReference);
//                if (newVerse != null)
//                {
//                    newVerse.Verse_Number = ReferenceParse.GetVerseNumber(newVerse.verse_reference);
//                    uv.Verses.Add(newVerse);
//                }
//            }
//        }

//        foreach (var uv in collection.UserVerses)
//        {
//            Debug.WriteLine("\n\ncollection.UserVerse: " + uv.ReadableReference);
//            foreach (var v in uv.Verses)
//            {
//                Debug.WriteLine("userVerse.Verse: " + v.verse_reference);
//            }
//        }

//        await PopulateCollectionNotes(collection);

//        return collection;
//    }

//    public async Task<Collection?> GetCollectionById(int collectionId)
//    {
//        var sql = @"
//            SELECT
//                c.COLLECTION_ID AS CollectionId,
//                c.AUTHOR_USERNAME AS AuthorUsername,
//                c.USERNAME AS Username,
//                c.TITLE AS Title,
//                c.VISIBILITY AS Visibility,
//                c.VERSE_ORDER AS VerseOrder,
//                c.DATE_CREATED AS DateCreated
//            FROM COLLECTIONS c
//            WHERE c.COLLECTION_ID = :CollectionId";

//        using IDbConnection conn = new OracleConnection(connectionString);
//        var result = await conn.QueryFirstOrDefaultAsync<Collection>(sql, new { CollectionId = collectionId }, commandType: CommandType.Text);

//        if (result != null)
//        {
//            await PopulateCollectionNotes(result);
//        }

//        return result;
//    }

//    public async Task<Collection?> GetCollectionByPublishedId(int publishedId)
//    {
//        var sql = @"
//            SELECT
//                c.COLLECTION_ID AS CollectionId,
//                c.AUTHOR_USERNAME AS AuthorUsername,
//                c.USERNAME AS Username,
//                c.TITLE AS Title,
//                c.VISIBILITY AS Visibility,
//                c.VERSE_ORDER AS VerseOrder,
//                c.DATE_CREATED AS DateCreated
//            FROM PUBLISHED_COLLECTIONS p
//            INNER JOIN COLLECTIONS c
//                ON p.COLLECTION_ID = c.COLLECTION_ID
//            WHERE p.PUBLISHED_ID = :PublishedId";

//        using IDbConnection conn = new OracleConnection(connectionString);
//        var result = await conn.QueryFirstOrDefaultAsync<Collection>(sql, new { PublishedId = publishedId }, commandType: CommandType.Text);

//        if (result != null)
//        {
//            await PopulateCollectionNotes(result);
//        }

//        return result;
//    }

//    public async Task<IEnumerable<Collection>> GetUserCollectionsWithUserVerses(string username)
//    {
//        var sql = @"
//                    SELECT
//                        c.COLLECTION_ID AS CollectionId,
//                        c.AUTHOR_USERNAME AS AuthorUsername,
//                        c.TITLE,
//                        c.VISIBILITY,
//                        c.VERSE_ORDER AS VerseOrder,
//                        c.DATE_CREATED AS DateCreated,
//                        uv.USER_VERSE_ID AS Id,
//                        uv.USERNAME as Username,
//                        uv.READABLE_REFERENCE AS ReadableReference,
//                        uv.LAST_PRACTICED AS LastPracticed,
//                        uv.PROGRESS_PERCENT AS ProgressPercent,
//                        uv.TIMES_MEMORIZED AS TimesMemorized,
//                        uv.DATE_SAVED AS DateAdded,
//                        uv.IN_PUBLISHED AS INPUBLISHED
//                    FROM COLLECTIONS c
//                    LEFT JOIN USER_VERSES uv
//                        ON c.COLLECTION_ID = uv.COLLECTION_ID
//                        AND uv.USERNAME = :Username
//                        AND uv.IN_PUBLISHED = 0
//                    WHERE c.USERNAME = :Username
//                    ORDER BY c.order_position";

//        using IDbConnection conn = new OracleConnection(connectionString);

//        var collectionDictionary = new Dictionary<int, Collection>();

//        var results = await conn.QueryAsync<Collection, UserPassage, Collection>(
//            sql,
//            (collection, userVerse) =>
//            {
//                if (!collectionDictionary.TryGetValue(collection.CollectionId, out var currentCollection))
//                {
//                    currentCollection = collection;
//                    Debug.WriteLine(collection.Title);
//                    currentCollection.UserVerses = new List<UserPassage>();
//                    collectionDictionary.Add(currentCollection.CollectionId, currentCollection);
//                }

//                if (userVerse != null && userVerse.Id != 0)
//                {
//                    currentCollection.UserVerses.Add(userVerse);
//                }

//                return currentCollection;
//            },
//            param: new { Username = username },
//            splitOn: "Id"
//        );

//        var result = await GetUserVersesForCollection(collectionDictionary);
//        await PopulateCollectionNotes(result);

//        return result;
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


//    private static async Task<IEnumerable<Collection>> GetUserVersesForCollection(Dictionary<int, Collection> collectionDictionary)
//    {
//        Debug.WriteLine("\n\nBefore function: ");
//        foreach (var col in collectionDictionary.Values)
//        {
//            Debug.WriteLine(col.Title);
//        }

//        foreach (var col in collectionDictionary.Values)
//        {
//            if (col.VerseOrder != null && col.VerseOrder.Length > 0)
//            {
//                var orderArray = col.VerseOrder.Split(',', StringSplitOptions.RemoveEmptyEntries)
//                    .Select(r => r.Trim())
//                    .ToArray();

//                if (orderArray.Length > 0)
//                {
//                    var verseMap = new Dictionary<string, UserPassage>(StringComparer.OrdinalIgnoreCase);
//                    foreach (var uv in col.UserVerses)
//                    {
//                        if (!string.IsNullOrWhiteSpace(uv.ReadableReference))
//                        {
//                            var key = uv.ReadableReference.Trim();
//                            if (!verseMap.ContainsKey(key))
//                            {
//                                verseMap[key] = uv;
//                            }
//                        }
//                    }

//                    var ordered = new List<UserPassage>();
//                    var unordered = new List<UserPassage>();

//                    foreach (var ref_ in orderArray)
//                    {
//                        var trimmedRef = ref_.Trim();
//                        if (verseMap.TryGetValue(trimmedRef, out var uv))
//                        {
//                            ordered.Add(uv);
//                            verseMap.Remove(trimmedRef);
//                        }
//                    }

//                    ordered.AddRange(verseMap.Values);

//                    col.UserVerses = ordered;
//                }
//            }

//            col.UserVerses ??= new List<UserPassage>();
//        }

//        Debug.WriteLine("\n\nAfter function: ");
//        foreach (var col in collectionDictionary.Values)
//        {
//            Debug.WriteLine(col.Title);
//        }
//        return collectionDictionary.Values.ToList();
//    }

//    private async Task PopulateCollectionNotes(Collection collection)
//    {
//        if (collection == null || collection.CollectionId == 0)
//        {
//            collection.Notes = new List<CollectionNote>();
//            return;
//        }

//        var sql = @"SELECT ID AS Id, COLLECTION_ID AS CollectionId, TEXT AS Text
//                    FROM COLLECTION_NOTES
//                    WHERE COLLECTION_ID = :CollectionId";

//        using IDbConnection conn = new OracleConnection(connectionString);
//        var notes = await conn.QueryAsync<CollectionNote>(sql, new { CollectionId = collection.CollectionId }, commandType: CommandType.Text);
//        collection.Notes = notes?.ToList() ?? new List<CollectionNote>();
//    }

//    private async Task PopulateCollectionNotes(IEnumerable<Collection> collections)
//    {
//        if (collections == null) return;

//        var collectionList = collections.ToList();
//        if (collectionList.Count == 0) return;

//        var collectionIds = collectionList.Where(c => c.CollectionId > 0).Select(c => c.CollectionId).Distinct().ToList();
//        if (collectionIds.Count == 0)
//        {
//            foreach (var col in collectionList)
//            {
//                col.Notes = new List<CollectionNote>();
//            }
//            return;
//        }

//        using IDbConnection conn = new OracleConnection(connectionString);

//        var inClause = string.Join(",", collectionIds.Select((_, i) => $":id{i}"));
//        var sql = $@"SELECT ID AS Id, COLLECTION_ID AS CollectionId, TEXT AS Text
//                    FROM COLLECTION_NOTES
//                    WHERE COLLECTION_ID IN ({inClause})";

//        var parameters = new DynamicParameters();
//        for (int i = 0; i < collectionIds.Count; i++)
//        {
//            parameters.Add($"id{i}", collectionIds[i]);
//        }

//        var notes = await conn.QueryAsync<CollectionNote>(sql, parameters, commandType: CommandType.Text);

//        var notesByCollection = notes?.GroupBy(n => n.CollectionId).ToDictionary(g => g.Key, g => g.ToList()) ?? new Dictionary<int, List<CollectionNote>>();

//        foreach (var col in collectionList)
//        {
//            if (notesByCollection.TryGetValue(col.CollectionId, out var collectionNotes))
//            {
//                col.Notes = collectionNotes;
//            }
//            else
//            {
//                col.Notes = new List<CollectionNote>();
//            }
//        }
//    }

//    public async Task InsertCollectionNote(CollectionNote note)
//    {
//        if (note == null || string.IsNullOrWhiteSpace(note.Id))
//        {
//            throw new ArgumentException("Note ID is required.", nameof(note));
//        }

//        var sql = @"INSERT INTO COLLECTION_NOTES (ID, COLLECTION_ID, TEXT)
//                    VALUES (:Id, :CollectionId, :Text)";

//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new
//        {
//            Id = note.Id,
//            CollectionId = note.CollectionId,
//            Text = note.Text
//        }, commandType: CommandType.Text);
//    }

//    public async Task UpdateCollectionNote(CollectionNote note)
//    {
//        if (note == null || string.IsNullOrWhiteSpace(note.Id))
//        {
//            throw new ArgumentException("Note ID is required.", nameof(note));
//        }

//        var sql = @"UPDATE COLLECTION_NOTES
//                    SET TEXT = :Text
//                    WHERE ID = :Id AND COLLECTION_ID = :CollectionId";

//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new
//        {
//            Id = note.Id,
//            CollectionId = note.CollectionId,
//            Text = note.Text
//        }, commandType: CommandType.Text);
//    }

//    public async Task DeleteCollectionNote(string noteId, int collectionId)
//    {
//        if (string.IsNullOrWhiteSpace(noteId))
//        {
//            throw new ArgumentException("Note ID is required.", nameof(noteId));
//        }

//        var sql = @"DELETE FROM COLLECTION_NOTES
//                    WHERE ID = :Id AND COLLECTION_ID = :CollectionId";

//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new { Id = noteId, CollectionId = collectionId }, commandType: CommandType.Text);
//    }

//    public async Task DeleteCollectionNotesByCollection(int collectionId)
//    {
//        var sql = @"DELETE FROM COLLECTION_NOTES WHERE COLLECTION_ID = :CollectionId";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new { CollectionId = collectionId }, commandType: CommandType.Text);
//    }

//    private static async Task<IEnumerable<PublishedCollection>> GetUserVersesForPublishedCollection(Dictionary<int, PublishedCollection> collectionDictionary)
//    {
//        foreach (var col in collectionDictionary.Values)
//        {
//            col.UserVerses ??= new List<UserPassage>();

//            if (!string.IsNullOrWhiteSpace(col.VerseOrder) && col.UserVerses.Count > 0)
//            {
//                var orderArray = col.VerseOrder
//                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
//                    .Select(r => r.Trim())
//                    .ToArray();

//                if (orderArray.Length > 0)
//                {
//                    var verseMap = new Dictionary<string, UserPassage>(StringComparer.OrdinalIgnoreCase);
//                    foreach (var uv in col.UserVerses)
//                    {
//                        if (string.IsNullOrWhiteSpace(uv.ReadableReference))
//                        {
//                            continue;
//                        }

//                        var key = uv.ReadableReference.Trim();
//                        if (!verseMap.ContainsKey(key))
//                        {
//                            verseMap.Add(key, uv);
//                        }
//                    }

//                    var ordered = new List<UserPassage>();

//                    foreach (var ref_ in orderArray)
//                    {
//                        var trimmedRef = ref_.Trim();
//                        if (verseMap.TryGetValue(trimmedRef, out var uv))
//                        {
//                            ordered.Add(uv);
//                            verseMap.Remove(trimmedRef);
//                        }
//                    }

//                    ordered.AddRange(verseMap.Values);
//                    col.UserVerses = ordered;
//                }
//            }

//            col.categoryIds ??= new List<int>();
//        }

//        return collectionDictionary.Values.ToList();
//    }

//    private async Task<IEnumerable<PublishedCollection>> QueryPublishedCollectionsAsync(string sql, object parameters)
//    {
//        using IDbConnection conn = new OracleConnection(connectionString);

//        var collectionDictionary = new Dictionary<int, PublishedCollection>();

//        await conn.QueryAsync<PublishedCollection, decimal?, UserPassage, PublishedCollection>(
//            sql,
//            (collection, categoryIdRaw, userVerse) =>
//            {
//                if (!collectionDictionary.TryGetValue(collection.PublishedId, out var currentCollection))
//                {
//                    currentCollection = collection;
//                    currentCollection.UserVerses = new List<UserPassage>();
//                    currentCollection.categoryIds ??= new List<int>();
//                    collectionDictionary.Add(currentCollection.PublishedId, currentCollection);
//                }

//                var categoryId = categoryIdRaw.HasValue ? Convert.ToInt32(categoryIdRaw.Value) : (int?)null;

//                if (categoryId.HasValue)
//                {
//                    currentCollection.categoryIds ??= new List<int>();
//                    if (!currentCollection.categoryIds.Contains(categoryId.Value))
//                    {
//                        currentCollection.categoryIds.Add(categoryId.Value);
//                    }
//                }

//                if (userVerse != null && userVerse.Id != 0)
//                {
//                    userVerse.Verses ??= new List<Verse>();
//                    currentCollection.UserVerses.Add(userVerse);
//                }

//                return currentCollection;
//            },
//            param: parameters,
//            splitOn: "CategoryId,Id",
//            commandType: CommandType.Text);

//        var results = await GetUserVersesForPublishedCollection(collectionDictionary);
//        var resultsList = results.ToList();

//        // Load notes for all published collections
//        await PopulatePublishedCollectionNotes(resultsList);

//        return resultsList;
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

//    public async Task<Collection?> GetFriendCollectionWithVerses(int collectionId, string? viewerUsername)
//    {
//        const string collectionSql = @"
//            SELECT
//                c.COLLECTION_ID AS CollectionId,
//                c.AUTHOR_USERNAME AS AuthorUsername,
//                c.USERNAME AS Username,
//                c.TITLE,
//                c.VISIBILITY,
//                c.VERSE_ORDER AS VerseOrder,
//                c.DATE_CREATED AS DateCreated,
//                uv.USER_VERSE_ID AS Id,
//                uv.USERNAME,
//                uv.READABLE_REFERENCE AS ReadableReference,
//                uv.LAST_PRACTICED AS LastPracticed,
//                uv.PROGRESS_PERCENT AS ProgressPercent,
//                uv.TIMES_MEMORIZED AS TimesMemorized,
//                uv.DATE_SAVED AS DateAdded,
//                uv.IN_PUBLISHED AS INPUBLISHED
//            FROM COLLECTIONS c
//            LEFT JOIN USER_VERSES uv
//                ON c.COLLECTION_ID = uv.COLLECTION_ID
//                AND uv.USERNAME = c.USERNAME
//                AND uv.IN_PUBLISHED = 0
//            WHERE c.COLLECTION_ID = :CollectionId";

//        using IDbConnection conn = new OracleConnection(connectionString);

//        var collectionDictionary = new Dictionary<int, Collection>();

//        await conn.QueryAsync<Collection, UserPassage, Collection>(
//            collectionSql,
//            (collection, userVerse) =>
//            {
//                if (!collectionDictionary.TryGetValue(collection.CollectionId, out var currentCollection))
//                {
//                    currentCollection = collection;
//                    currentCollection.UserVerses = new List<UserPassage>();
//                    collectionDictionary.Add(currentCollection.CollectionId, currentCollection);
//                }

//                if (userVerse != null && userVerse.Id != 0)
//                {
//                    userVerse.Verses ??= new List<Verse>();
//                    currentCollection.UserVerses.Add(userVerse);
//                }

//                return currentCollection;
//            },
//            param: new { CollectionId = collectionId },
//            splitOn: "Id",
//            commandType: CommandType.Text);

//        if (!collectionDictionary.TryGetValue(collectionId, out var collectionResult))
//        {
//            return null;
//        }

//        var visibility = collectionResult.Visibility?.Trim() ?? string.Empty;
//        var ownerUsername = collectionResult.Username;

//        var isOwner = !string.IsNullOrWhiteSpace(viewerUsername) &&
//                      ownerUsername != null &&
//                      ownerUsername.Equals(viewerUsername, StringComparison.OrdinalIgnoreCase);

//        var isPublic = visibility.Equals("public", StringComparison.OrdinalIgnoreCase);
//        var isFriendsOnly = visibility.Equals("friends", StringComparison.OrdinalIgnoreCase);

//        var allowed = isOwner || isPublic;

//        if (!allowed && isFriendsOnly && !string.IsNullOrWhiteSpace(viewerUsername))
//        {
//            const string checkFriendSql = @"SELECT COUNT(*) FROM USER_RELATIONSHIPS 
//                                WHERE ((USERNAME_1 = :Username1 AND USERNAME_2 = :Username2) OR (USERNAME_1 = :Username2 AND USERNAME_2 = :Username1))
//                                AND TYPE = 1";
//            var isFriend = await conn.QueryFirstOrDefaultAsync<int>(
//                checkFriendSql,
//                new { Username1 = ownerUsername, Username2 = viewerUsername },
//                commandType: CommandType.Text);
//            allowed = isFriend > 0;
//        }

//        if (!allowed)
//        {
//            return null;
//        }

//        await GetUserVersesForCollection(collectionDictionary);
//        await PopulateCollectionNotes(collectionResult);

//        return collectionResult;
//    }

//    public async Task<IEnumerable<PublishedCollection>> GetPopularCollections(int top)
//    {
//        var limit = Math.Max(1, top);
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
//            FROM (
//                SELECT *
//                FROM PUBLISHED_COLLECTIONS
//                WHERE STATUS = 'APPROVED'
//                ORDER BY NUM_SAVES DESC, PUBLISHED_ID DESC
//                FETCH FIRST :Limit ROWS ONLY
//            ) p
//            LEFT JOIN COLLECTION_CATEGORIES cc
//                ON p.PUBLISHED_ID = cc.PUBLISHED_ID
//            LEFT JOIN USER_VERSES uv
//                ON p.PUBLISHED_ID = uv.COLLECTION_ID
//                AND uv.USERNAME = p.AUTHOR
//                AND uv.IN_PUBLISHED = 1
//            ORDER BY p.NUM_SAVES DESC, p.PUBLISHED_ID DESC";

//        return await QueryPublishedCollectionsAsync(sql, new { Limit = limit });
//    }

//    public async Task<IEnumerable<PublishedCollection>> GetRecentCollections(int top)
//    {
//        var limit = Math.Max(1, top);
//        const string sql = @"
//                    SELECT
//                        p.PUBLISHED_ID AS PublishedId,
//                        p.DESCRIPTION AS Description,
//                        p.AUTHOR AS Author,
//                        p.TITLE AS Title,
//                        p.NUM_SAVES AS NumSaves,
//                        p.VERSE_ORDER AS VerseOrder,
//                        p.COLLECTION_ID AS CollectionId,
//                        p.DATE_PUBLISHED AS PublishedDate,
//                        p.STATUS AS Status,
//                cc.CATEGORY_ID AS CategoryId,
//                        uv.USER_VERSE_ID AS Id,
//                        uv.USERNAME AS Username,
//                        uv.READABLE_REFERENCE AS ReadableReference,
//                        uv.LAST_PRACTICED AS LastPracticed,
//                        uv.PROGRESS_PERCENT AS ProgressPercent,
//                        uv.TIMES_MEMORIZED AS TimesMemorized,
//                        uv.DATE_SAVED AS DateAdded,
//                        uv.COLLECTION_ID AS CollectionId,
//                        uv.IN_PUBLISHED AS INPUBLISHED
//                    FROM (
//                SELECT *
//                FROM PUBLISHED_COLLECTIONS
//                WHERE STATUS = 'APPROVED'
//                        ORDER BY DATE_PUBLISHED DESC, PUBLISHED_ID DESC
//                        FETCH FIRST :Limit ROWS ONLY
//                    ) p
//            LEFT JOIN COLLECTION_CATEGORIES cc
//                ON p.PUBLISHED_ID = cc.PUBLISHED_ID
//                    LEFT JOIN USER_VERSES uv
//                        ON p.PUBLISHED_ID = uv.COLLECTION_ID
//                        AND uv.USERNAME = p.AUTHOR
//                        AND uv.IN_PUBLISHED = 1
//                    ORDER BY p.DATE_PUBLISHED DESC, p.PUBLISHED_ID DESC";

//        return await QueryPublishedCollectionsAsync(sql, new { Limit = limit });
//    }

//    public async Task<IEnumerable<PublishedCollection>> GetPublishedCollectionsByAuthor(string username)
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
//            WHERE LOWER(p.AUTHOR) = LOWER(:Author)
//                AND p.STATUS = 'APPROVED'
//            ORDER BY p.DATE_PUBLISHED DESC, p.PUBLISHED_ID DESC";

//        return await QueryPublishedCollectionsAsync(sql, new { Author = username });
//    }

//    public async Task<IEnumerable<PublishedCollection>> GetPublishedCollectionsByCategory(int categoryId)
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
//            INNER JOIN COLLECTION_CATEGORIES cc
//                ON p.PUBLISHED_ID = cc.PUBLISHED_ID
//            LEFT JOIN USER_VERSES uv
//                ON p.PUBLISHED_ID = uv.COLLECTION_ID
//                AND uv.USERNAME = p.AUTHOR
//                AND uv.IN_PUBLISHED = 1
//            WHERE cc.CATEGORY_ID = :CategoryId
//                AND p.STATUS = 'APPROVED'
//            ORDER BY p.DATE_PUBLISHED DESC, p.PUBLISHED_ID DESC";

//        return await QueryPublishedCollectionsAsync(sql, new { CategoryId = categoryId });
//    }

//    public async Task<PublishedCollection?> GetPublishedCollectionById(int publishedId)
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
//            WHERE p.PUBLISHED_ID = :PublishedId";

//        var results = await QueryPublishedCollectionsAsync(sql, new { PublishedId = publishedId });
//        var publishedCollection = results.FirstOrDefault();

//        // Load notes for published collection
//        if (publishedCollection != null)
//        {
//            await PopulatePublishedCollectionNotes(publishedCollection);
//        }

//        return publishedCollection;
//    }

//    private async Task PopulatePublishedCollectionNotes(PublishedCollection publishedCollection)
//    {
//        if (publishedCollection == null || publishedCollection.PublishedId <= 0)
//        {
//            publishedCollection.Notes = new List<CollectionNote>();
//            return;
//        }

//        using IDbConnection conn = new OracleConnection(connectionString);
//        var sql = @"SELECT ID AS Id, PUBLISHED_ID AS CollectionId, TEXT AS Text
//                    FROM PUBLISHED_COLLECTION_NOTES
//                    WHERE PUBLISHED_ID = :PublishedId";
//        var notes = await conn.QueryAsync<CollectionNote>(sql, new { PublishedId = publishedCollection.PublishedId }, commandType: CommandType.Text);
//        publishedCollection.Notes = notes?.ToList() ?? new List<CollectionNote>();
//    }

//    private async Task PopulatePublishedCollectionNotes(IEnumerable<PublishedCollection> publishedCollections)
//    {
//        if (publishedCollections == null || !publishedCollections.Any())
//        {
//            return;
//        }

//        var publishedIds = publishedCollections
//            .Where(pc => pc != null && pc.PublishedId > 0)
//            .Select(pc => pc.PublishedId)
//            .Distinct()
//            .ToList();

//        if (publishedIds.Count == 0)
//        {
//            foreach (var pc in publishedCollections)
//            {
//                pc.Notes = new List<CollectionNote>();
//            }
//            return;
//        }

//        using IDbConnection conn = new OracleConnection(connectionString);
//        var parameters = new DynamicParameters();
//        var inClause = string.Join(",", publishedIds.Select((_, i) => $":PublishedId{i}"));
//        for (int i = 0; i < publishedIds.Count; i++)
//        {
//            parameters.Add($"PublishedId{i}", publishedIds[i]);
//        }

//        var sql = $@"SELECT ID AS Id, PUBLISHED_ID AS CollectionId, TEXT AS Text
//                    FROM PUBLISHED_COLLECTION_NOTES
//                    WHERE PUBLISHED_ID IN ({inClause})";
//        var notes = await conn.QueryAsync<CollectionNote>(sql, parameters, commandType: CommandType.Text);
//        var notesByPublishedId = notes?.GroupBy(n => n.CollectionId).ToDictionary(g => g.Key, g => g.ToList()) ?? new Dictionary<int, List<CollectionNote>>();

//        foreach (var pc in publishedCollections)
//        {
//            if (pc == null || pc.PublishedId <= 0)
//            {
//                pc.Notes = new List<CollectionNote>();
//            }
//            else if (notesByPublishedId.TryGetValue(pc.PublishedId, out var collectionNotes))
//            {
//                pc.Notes = collectionNotes;
//            }
//            else
//            {
//                pc.Notes = new List<CollectionNote>();
//            }
//        }
//    }

//    public async Task<IEnumerable<PublishedCollection>> SearchPublishedCollections(string query, IEnumerable<string> verseReferences, int limit)
//    {
//        var hasTerm = !string.IsNullOrWhiteSpace(query.Trim());
//        var lowerTerm = query.Trim().ToLowerInvariant();

//        var verseList = verseReferences?
//            .Where(v => !string.IsNullOrWhiteSpace(v))
//            .Select(v => v.Trim().ToLowerInvariant())
//            .Distinct()
//            .Take(50)
//            .ToList() ?? new List<string>();

//        var parameters = new DynamicParameters();
//        parameters.Add("Limit", Math.Max(1, limit));
//        parameters.Add("HasTerm", hasTerm ? 1 : 0);
//        parameters.Add("TermLower", lowerTerm);

//        string verseScoreExpr = "0";
//        if (verseList.Count > 0)
//        {
//            var parts = new List<string>();
//            for (int i = 0; i < verseList.Count; i++)
//            {
//                var paramName = $"VerseRef{i}";
//                parts.Add($"MAX(CASE WHEN INSTR(LOWER(NVL(p.VERSE_ORDER, '')), :{paramName}) > 0 THEN 1 ELSE 0 END)");
//                parameters.Add(paramName, verseList[i]);
//            }

//            verseScoreExpr = string.Join(" + ", parts);
//        }

//        var sql = $@"
//            WITH scored AS (
//                SELECT
//                    p.PUBLISHED_ID AS PublishedId,
//                    MAX(CASE WHEN :HasTerm = 1 AND INSTR(LOWER(p.TITLE), :TermLower) > 0 THEN 1 ELSE 0 END) AS TitleScore,
//                    {verseScoreExpr} AS VerseScore,
//                    MAX(CASE WHEN :HasTerm = 1 AND cat.NAME IS NOT NULL AND INSTR(LOWER(cat.NAME), :TermLower) > 0 THEN 1 ELSE 0 END) AS CategoryScore,
//                    MAX(CASE WHEN :HasTerm = 1 AND LOWER(p.AUTHOR) = :TermLower THEN 1 ELSE 0 END) AS AuthorExactScore,
//                    MAX(CASE WHEN :HasTerm = 1
//                        AND (
//                            INSTR(LOWER(p.AUTHOR), :TermLower) > 0
//                            OR INSTR(LOWER(NVL(u.FIRSTNAME, '')), :TermLower) > 0
//                            OR INSTR(LOWER(NVL(u.LASTNAME, '')), :TermLower) > 0
//                            OR INSTR(LOWER(TRIM(NVL(u.FIRSTNAME, '') || ' ' || NVL(u.LASTNAME, ''))), :TermLower) > 0
//                        )
//                    THEN 1 ELSE 0 END) AS AuthorPartialScore
//                FROM PUBLISHED_COLLECTIONS p
//                LEFT JOIN USERS u ON u.USERNAME = p.AUTHOR
//                LEFT JOIN COLLECTION_CATEGORIES cc ON p.PUBLISHED_ID = cc.PUBLISHED_ID
//                LEFT JOIN CATEGORIES cat ON cc.CATEGORY_ID = cat.CATEGORY_ID
//                WHERE p.STATUS = 'APPROVED'
//                GROUP BY p.PUBLISHED_ID, p.TITLE, p.AUTHOR, p.VERSE_ORDER
//            )
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
//            FROM scored s
//            JOIN PUBLISHED_COLLECTIONS p ON p.PUBLISHED_ID = s.PublishedId
//            LEFT JOIN COLLECTION_CATEGORIES cc ON p.PUBLISHED_ID = cc.PUBLISHED_ID
//            LEFT JOIN USER_VERSES uv
//                ON p.PUBLISHED_ID = uv.COLLECTION_ID
//                AND uv.USERNAME = p.AUTHOR
//                AND uv.IN_PUBLISHED = 1
//            WHERE (s.TitleScore + s.VerseScore + s.CategoryScore + s.AuthorExactScore + s.AuthorPartialScore) > 0
//            ORDER BY s.TitleScore DESC,
//                     s.VerseScore DESC,
//                     s.CategoryScore DESC,
//                     s.AuthorExactScore DESC,
//                     s.AuthorPartialScore DESC,
//                     p.NUM_SAVES DESC,
//                     p.PUBLISHED_ID DESC
//            FETCH FIRST :Limit ROWS ONLY";

//        return await QueryPublishedCollectionsAsync(sql, parameters);
//    }

//    public async Task UpdateCollectionVerseOrder(int collectionId, string verseOrder)
//    {
//        var sql = @"UPDATE COLLECTIONS SET VERSE_ORDER = :VerseOrder WHERE COLLECTION_ID = :CollectionId";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new
//        {
//            VerseOrder = verseOrder ?? string.Empty,
//            CollectionId = collectionId
//        }, commandType: CommandType.Text);
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

