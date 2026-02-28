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
//}


