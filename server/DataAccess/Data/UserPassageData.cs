using DataAccess.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Dapper;
using ScriptureMemoryLibrary;
using Oracle.ManagedDataAccess.Client;
using DataAccess.DataInterfaces;

namespace DataAccess.Data;

public class UserPassageData : IUserPassageData
{
    private readonly IDbConnection conn;

    public UserPassageData(IDbConnection connection)
    {
        conn = connection;
    }

    public async Task<int> InsertUserPassage(UserPassage newPassage)
    {
        var sql = @"INSERT INTO USER_PASSAGES
                    (REFERENCE, DUE_DATE, LAST_PRACTICED, TIMES_MEMORIZED, DATE_SAVED, 
                     COLLECTION_ID, ORDER_POSITION, NOTIFY_MEMORIZED, USER_ID, PROGRESS_PERCENT)
                    VALUES
                    (:Reference, :DueDate, :LastPracticed, :TimesMemorized, :DateSaved, 
                     :CollectionId, :OrderPosition, :NotifyMemorized, :UserId, :ProgressPercent)
                    RETURNING ID INTO :NewId";

        var parameters = new DynamicParameters();

        parameters.Add("Reference", newPassage.Reference.ReadableReference);
        parameters.Add("DueDate", newPassage.DueDate);
        parameters.Add("LastPracticed", newPassage.LastPracticed);
        parameters.Add("TimesMemorized", newPassage.TimesMemorized);
        parameters.Add("DateSaved", newPassage.DateAdded);
        parameters.Add("CollectionId", newPassage.CollectionId);
        parameters.Add("OrderPosition", newPassage.OrderPosition);
        parameters.Add("NotifyMemorized", newPassage.NotifyMemorized ? 1 : 0);
        parameters.Add("UserId", newPassage.UserId);
        parameters.Add("ProgressPercent", newPassage.ProgressPercent);

        parameters.Add(
            "NewId",
            dbType: DbType.Int32,
            direction: ParameterDirection.Output
        );

        await conn.ExecuteAsync(sql, parameters);

        var newId = parameters.Get<int>("NewId");

        return newId;
    }

    public async Task<string> GetPassageTextFromListOfReferences(List<string> references)
    {
        var sql = @"SELECT TEXT FROM VERSES WHERE VERSE_REFERENCE IN (:References)";
        // If you get an error for this, remove the paranthesis

        var result = await conn.QueryAsync<string>(sql, new { References = references });
        
        return string.Join(" ", result);
    }

    // DTO used for fetching a row of user passages
    private class UserPassageRow
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CollectionId { get; set; }
        public string ReadableReference { get; set; } = string.Empty;
        public int OrderPosition { get; set; }
        public DateTime DateAdded { get; set; }
        public float ProgressPercent { get; set; }
        public int TimesMemorized { get; set; }
        public DateTime? LastPracticed { get; set; }
        public DateTime? DueDate { get; set; }
        public bool NotifyMemorized { get; set; }
        public List<VerseRow> Verses { get; set; } = new();
    }

    private class VerseRow
    {
        public int VerseId { get; set; }
        public string ReadableReference { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public int UsersSavedCount { get; set; }
        public int UsersMemorizedCount { get; set; }
    }

    /// <summary>
    /// Gets a list of user passages populated with its verses for a collection
    /// </summary>
    /// <param name="collectionId"></param>
    /// <returns>List<UserPassage>></returns>
    public async Task<List<UserPassage>> GetUserPassagesPopulatedForCollection(int collectionId)
    {
        var sql = @"SELECT 
                up.ID,
                up.USER_ID AS UserId,
                up.COLLECTION_ID AS CollectionId,
                up.REFERENCE AS ReadableReference,
                up.ORDER_POSITION AS OrderPosition,
                up.DATE_SAVED AS DateAdded,
                up.progress_percent as ProgressPercent,
                up.times_memorized as TimesMemorized,
                up.last_practiced as LastPracticed,
                up.due_date as DueDate,
                up.notify_memorized as NotifyMemorized,

                v.verse_id as VerseId,
                v.verse_reference as ReadableReference,
                v.verse_text as Text,
                v.users_saved_verse as UsersSavedCount, 
                v.users_memorized as UsersMemorizedCount

                FROM USER_PASSAGES up 
                JOIN verses v on up.reference = v.verse_reference
                WHERE up.COLLECTION_ID = :CollectionId
                ORDER BY up.ORDER_POSITION, v.verse_id";

        var lookup = new Dictionary<int, UserPassage>();

        await conn.QueryAsync<UserPassageRow, VerseRow, UserPassage>(
            sql,
            (row, verseRow) =>
            {
                if (!lookup.TryGetValue(row.Id, out var passage))
                {
                    passage = new UserPassage(
                        row.UserId,
                        row.CollectionId,
                        row.ReadableReference)
                    {
                        Id = row.Id,
                        OrderPosition = row.OrderPosition,
                        DateAdded = row.DateAdded,
                        ProgressPercent = row.ProgressPercent,
                        TimesMemorized = row.TimesMemorized,
                        LastPracticed = row.LastPracticed,
                        DueDate = row.DueDate
                    };

                    lookup.Add(row.Id, passage);
                }

                var verse = new Verse(
                    new DataAccess.Models.Reference(verseRow.ReadableReference),
                    verseRow.Text)
                {
                    Id = verseRow.VerseId,
                    UsersSavedCount = verseRow.UsersSavedCount,
                    UsersMemorizedCount = verseRow.UsersMemorizedCount
                };

                passage.Verses.Add(verse);

                return passage;
            },
            new { CollectionId = collectionId },
            splitOn: "VerseId"
        );

        return lookup.Values.ToList();
    }
}

//    public async Task<List<UserPassage>> GetMemorized(string username)
//    {
//        var sql = @"SELECT USER_VERSE_ID AS Id, USERNAME AS Username, COLLECTION_ID AS CollectionId, 
//                           READABLE_REFERENCE AS ReadableReference, LAST_PRACTICED AS LastPracticed,
//                           DATE_SAVED AS DateAdded, PROGRESS_PERCENT AS ProgressPercent, 
//                           TIMES_MEMORIZED AS TimesMemorized, DUE_DATE AS DUEDATE
//                    FROM (
//                        SELECT * FROM USER_VERSES
//                        WHERE USERNAME = :Username
//                        ORDER BY LAST_PRACTICED DESC
//                    ) uv
//                    WHERE uv.PROGRESS_PERCENT = 100";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var results = await conn.QueryAsync<UserPassage>(sql, new { Username = username }, commandType: CommandType.Text);
//        var memorizedList = results.ToList();

//        return memorizedList;
//    }

//    public async Task<List<UserPassage>> GetInProgress(string username)
//    {
//        var sql = @"SELECT USER_VERSE_ID AS Id, USERNAME AS Username, COLLECTION_ID AS CollectionId, 
//                           READABLE_REFERENCE AS ReadableReference, LAST_PRACTICED AS LastPracticed,
//                           DATE_SAVED AS DateAdded, PROGRESS_PERCENT AS ProgressPercent, 
//                           TIMES_MEMORIZED AS TimesMemorized, DUE_DATE AS DUEDATE
//                    FROM (
//                        SELECT * FROM USER_VERSES
//                        WHERE USERNAME = :Username
//                        ORDER BY LAST_PRACTICED DESC
//                    ) uv
//                    WHERE uv.PROGRESS_PERCENT < 100
//                        AND uv.PROGRESS_PERCENT > 0";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var results = await conn.QueryAsync<UserPassage>(
//            sql,
//            new { Username = username },
//            commandType: CommandType.Text);
//        var inProgressList = results.ToList();

//        return inProgressList;
//    }

//    public async Task<List<UserPassage>> GetNotStarted(string username)
//    {
//        var sql = @"SELECT USER_VERSE_ID AS Id, USERNAME AS Username, COLLECTION_ID AS CollectionId, 
//                           READABLE_REFERENCE AS ReadableReference, LAST_PRACTICED AS LastPracticed,
//                           DATE_SAVED AS DateAdded, PROGRESS_PERCENT AS ProgressPercent, 
//                           TIMES_MEMORIZED AS TimesMemorized, DUE_DATE AS DUEDATE
//                    FROM (
//                        SELECT * FROM USER_VERSES
//                        WHERE USERNAME = :Username
//                        ORDER BY LAST_PRACTICED DESC
//                    ) uv
//                    WHERE uv.PROGRESS_PERCENT IS NULL
//                        or uv.PROGRESS_PERCENT = 0";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var results = await conn.QueryAsync<UserPassage>(
//            sql,
//            new { Username = username },
//            commandType: CommandType.Text);
//        var notStartedList = results.ToList();

//        return notStartedList;
//    }

//    public async Task InsertUserVerse(UserPassage uv)
//    {
//        if (uv == null)
//        {
//            throw new ArgumentNullException(nameof(uv));
//        }

//        if (string.IsNullOrWhiteSpace(uv.ReadableReference))
//        {
//            throw new ArgumentException("ReadableReference cannot be null or empty.", nameof(uv));
//        }

//        const int maxPassagesPerCollection = 30;

//        using IDbConnection conn = new OracleConnection(connectionString);

//        var duplicateExists = await UserVerseExistsInCollection(uv.CollectionId, uv.ReadableReference);
//        if (duplicateExists)
//        {
//            throw new InvalidOperationException($"A verse with reference '{uv.ReadableReference}' already exists in this collection.");
//        }

//        var existingCount = await conn.ExecuteScalarAsync<int>(
//            @"SELECT COUNT(*) FROM USER_VERSES WHERE COLLECTION_ID = :CollectionId AND IN_PUBLISHED = 0",
//            new { CollectionId = uv.CollectionId },
//            commandType: CommandType.Text);

//        if (existingCount >= maxPassagesPerCollection)
//        {
//            throw new InvalidOperationException("Passage limit reached for this collection.");
//        }

//        var sql = @"INSERT INTO USER_VERSES 
//                    (USERNAME, READABLE_REFERENCE, COLLECTION_ID, DATE_SAVED)
//                     VALUES
//                    (:Username, :ReadableReference, :CollectionId, SYSDATE)";
//        await conn.ExecuteAsync(sql, new
//        {
//            Username = uv.Username,
//            ReadableReference = uv.ReadableReference,
//            CollectionId = uv.CollectionId
//        }, commandType: CommandType.Text);
//    }

//    public async Task<bool> UserVerseExistsInCollection(int collectionId, string readableReference)
//    {
//        if (string.IsNullOrWhiteSpace(readableReference))
//        {
//            return false;
//        }

//        using IDbConnection conn = new OracleConnection(connectionString);
//        var count = await conn.ExecuteScalarAsync<int>(
//            @"SELECT COUNT(*) FROM USER_VERSES 
//              WHERE COLLECTION_ID = :CollectionId 
//                AND IN_PUBLISHED = 0 
//                AND UPPER(TRIM(READABLE_REFERENCE)) = UPPER(TRIM(:ReadableReference))",
//            new { CollectionId = collectionId, ReadableReference = readableReference },
//            commandType: CommandType.Text);

//        return count > 0 ? true : false;
//    }

//    public async Task InsertUserVersesToNewCollection(List<UserPassage> userVerses, int newCollectionId)
//    {
//        if (userVerses == null)
//        {
//            throw new ArgumentNullException(nameof(userVerses));
//        }

//        const int maxPassagesPerCollection = 30;

//        using IDbConnection conn = new OracleConnection(connectionString);

//        var existingCount = await conn.ExecuteScalarAsync<int>(
//                @"SELECT COUNT(*) FROM USER_VERSES WHERE COLLECTION_ID = :CollectionId AND IN_PUBLISHED = 0",
//                new { CollectionId = newCollectionId },
//                commandType: CommandType.Text
//            );

//        var distinctReferences = new List<string>();
//        var seenReferences = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

//        foreach (var uv in userVerses)
//        {
//            if (string.IsNullOrWhiteSpace(uv.ReadableReference))
//                continue;

//            var trimmedRef = uv.ReadableReference.Trim();
//            if (!seenReferences.Contains(trimmedRef))
//            {
//                var exists = await UserVerseExistsInCollection(newCollectionId, trimmedRef);
//                if (!exists)
//                {
//                    distinctReferences.Add(trimmedRef);
//                    seenReferences.Add(trimmedRef);
//                }
//            }
//        }

//        if (existingCount + distinctReferences.Count > maxPassagesPerCollection)
//        {
//            throw new InvalidOperationException("Passage limit reached for this collection.");
//        }

//        if (distinctReferences.Count == 0)
//        {
//            // No new verses to add
//            return;
//        }

//        var sql = @"INSERT INTO USER_VERSES (USERNAME, READABLE_REFERENCE, COLLECTION_ID, DATE_SAVED, IN_PUBLISHED) 
//                             VALUES (:username, :reference, :collectionId, SYSDATE, 0)";
//        foreach (var reference in distinctReferences)
//        {
//            var userVerse = userVerses.First(uv => string.Equals(uv.ReadableReference?.Trim(), reference, StringComparison.OrdinalIgnoreCase));
//            await conn.ExecuteAsync(sql, new
//            {
//                username = userVerse.Username,
//                reference,
//                collectionId = newCollectionId,
//            }, commandType: CommandType.Text);
//        }

//        List<string> newVerseReferences = new();
//        foreach (var uv in userVerses)
//        {
//            if (uv.Verses != null && uv.Verses.Count > 0)
//            {
//                foreach (var v in uv.Verses)
//                {
//                    if (!newVerseReferences.Contains(v.verse_reference))
//                        newVerseReferences.Add(v.verse_reference);
//                }
//            }
//            else if (!string.IsNullOrWhiteSpace(uv.ReadableReference))
//            {
//                var individualRefs = ReferenceParse.GetReferencesFromVersesInReference(uv.ReadableReference);
//                foreach (var ref_ in individualRefs)
//                {
//                    if (!newVerseReferences.Contains(ref_))
//                        newVerseReferences.Add(ref_);
//                }
//            }
//        }

//        if (newVerseReferences.Count > 0)
//        {
//            string references = string.Join(",", newVerseReferences.Select(r => $"'{r}'"));
//            sql = $@"UPDATE VERSES SET USERS_SAVED_VERSE = USERS_SAVED_VERSE + 1 WHERE VERSE_REFERENCE IN ({references})";
//            await conn.ExecuteAsync(sql, new { }, commandType: CommandType.Text);
//        }
//    }

//    public async Task AddUserVersesToNewlyPublishedCollection(List<UserPassage> userVerses, int publishedId)
//    {
//        var sql = @"INSERT INTO USER_VERSES (USERNAME, READABLE_REFERENCE, COLLECTION_ID, IN_PUBLISHED) 
//                             VALUES (:username, :reference, :collectionId, 1)";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        foreach (var uv in userVerses)
//        {
//            await conn.ExecuteAsync(sql, new
//            {
//                username = uv.Username,
//                reference = uv.ReadableReference,
//                collectionId = publishedId
//            },
//            commandType: CommandType.Text);
//        }
//    }

//    public async Task<IEnumerable<UserPassage>> GetUserVersesByCollection(int collectionId)
//    {
//        var sql = @"SELECT USER_VERSE_ID AS Id, USERNAME AS Username, COLLECTION_ID AS CollectionId, 
//                           READABLE_REFERENCE AS ReadableReference, LAST_PRACTICED AS LastPracticed,
//                           DATE_SAVED AS DateAdded, PROGRESS_PERCENT AS ProgressPercent, 
//                           TIMES_MEMORIZED AS TimesMemorized, IN_PUBLISHED as InPublished, DUE_DATE AS DUEDATE
//                    FROM USER_VERSES WHERE COLLECTION_ID = :CollectionId";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var userVerses = await conn.QueryAsync<UserPassage>(sql, 
//            new { CollectionId = collectionId },
//            commandType: CommandType.Text);
//        return userVerses;
//    }

//    public async Task<IEnumerable<UserPassage>> GetUserVersesByUsername(string username)
//    {
//        var sql = @"SELECT USER_VERSE_ID AS Id, USERNAME AS Username, COLLECTION_ID AS CollectionId, 
//                           READABLE_REFERENCE AS ReadableReference, LAST_PRACTICED AS LastPracticed,
//                           DATE_SAVED AS DateAdded, PROGRESS_PERCENT AS ProgressPercent, 
//                           TIMES_MEMORIZED AS TimesMemorized, IN_PUBLISHED AS InPublished, DUE_DATE AS DUEDATE
//                    FROM USER_VERSES WHERE USERNAME = :Username ORDER BY PROGRESS_PERCENT DESC";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var userVerses = await conn.QueryAsync<UserPassage>(sql,
//            new { Username = username },
//            commandType: CommandType.Text);
//        return userVerses;
//    }

//    public async Task<UserPassage?> GetUserVerse(int id)
//    {
//        var sql = @"SELECT USER_VERSE_ID AS Id, USERNAME AS Username, COLLECTION_ID AS CollectionId, 
//                           READABLE_REFERENCE AS ReadableReference, LAST_PRACTICED AS LastPracticed,
//                           DATE_SAVED AS DateAdded, PROGRESS_PERCENT AS ProgressPercent, 
//                           TIMES_MEMORIZED AS TimesMemorized, IN_PUBLISHED AS InPublished, DUE_DATE AS DUEDATE
//                    FROM USER_VERSES WHERE USER_VERSE_ID = :Id";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var userVerses = await conn.QueryAsync<UserPassage>(sql,
//            new { Id = id }, commandType: CommandType.Text);
//        var userVerse = userVerses.FirstOrDefault();

//        return userVerse;
//    }

//    public async Task<List<UserPassage>> GetUserVersesByIds(List<int> ids)
//    {
//        if (ids == null || ids.Count == 0)
//        {
//            return new List<UserPassage>();
//        }

//        var inClause = string.Join(",", ids.Select((_, i) => $":Id{i}"));
//        var sql = $@"SELECT USER_VERSE_ID AS Id, USERNAME AS Username, COLLECTION_ID AS CollectionId, 
//                           READABLE_REFERENCE AS ReadableReference, LAST_PRACTICED AS LastPracticed,
//                           DATE_SAVED AS DateAdded, PROGRESS_PERCENT AS ProgressPercent, 
//                           TIMES_MEMORIZED AS TimesMemorized, IN_PUBLISHED AS InPublished, DUE_DATE AS DUEDATE
//                    FROM USER_VERSES WHERE USER_VERSE_ID IN ({inClause})";

//        var parameters = new DynamicParameters();
//        for (int i = 0; i < ids.Count; i++)
//        {
//            parameters.Add($"Id{i}", ids[i]);
//        }

//        using IDbConnection conn = new OracleConnection(connectionString);
//        var userVerses = await conn.QueryAsync<UserPassage>(sql, parameters, commandType: CommandType.Text);
//        var userVersesList = userVerses.ToList();

//        return userVersesList;
//    }

//    public async Task UpdateUserVerse(UserPassage uv)
//    {
//        var existing = await GetUserVerse(uv.Id);
//        if (existing == null)
//        {
//            throw new ArgumentException($"UserVerse with Id {uv.Id} not found.");
//        }

//        var readableReference = !string.IsNullOrWhiteSpace(uv.ReadableReference) ? uv.ReadableReference : existing.ReadableReference;

//        if (string.IsNullOrWhiteSpace(readableReference))
//        {
//            throw new ArgumentException("ReadableReference cannot be null or empty.");
//        }

//        var sql = @"UPDATE USER_VERSES
//                     SET 
//                     READABLE_REFERENCE = :ReadableReference,
//                     LAST_PRACTICED = :LastPracticed,
//                     PROGRESS_PERCENT = :ProgressPercent,
//                     TIMES_MEMORIZED = :TimesMemorized,
//                     COLLECTION_ID = :CollectionId,
//                     DUE_DATE = :DueDate
//                     WHERE
//                     USER_VERSE_ID = :Id";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new
//        {
//            ReadableReference = readableReference,
//            LastPracticed = uv.LastPracticed != default ? uv.LastPracticed : existing.LastPracticed,
//            ProgressPercent = uv.ProgressPercent,
//            TimesMemorized = uv.TimesMemorized,
//            CollectionId = uv.CollectionId,
//            DueDate = uv.DueDate,
//            Id = uv.Id
//        }, commandType: CommandType.Text);
//    }

//    public async Task DeleteUserVerse(UserPassage uv)
//    {
//        var sql = @"DELETE FROM USER_VERSES WHERE USER_VERSE_ID = :Id";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new { Id = uv.Id }, commandType: CommandType.Text);
//    }

//    public async Task DeleteUserVersesByCollection(int collectionId)
//    {
//        var sql = @"DELETE FROM USER_VERSES WHERE COLLECTION_ID = :CollectionId";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new { CollectionId = collectionId }, commandType: CommandType.Text);
//    }

//    public async Task DeleteUserVersesByUsername(string username)
//    {
//        var sql = @"DELETE FROM USER_VERSES WHERE USERNAME = :Username";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new { Username = username }, commandType: CommandType.Text);
//    }

//    public async Task<List<UserPassage>> GetRecentPractice(string username, int limit = 3)
//    {
//        var sql = @"
//            SELECT * FROM (
//                SELECT USER_VERSE_ID AS Id, USERNAME AS Username, COLLECTION_ID AS CollectionId, 
//                       READABLE_REFERENCE AS ReadableReference, LAST_PRACTICED AS LastPracticed,
//                       DATE_SAVED AS DateAdded, PROGRESS_PERCENT AS ProgressPercent, 
//                       TIMES_MEMORIZED AS TimesMemorized, DUE_DATE AS DUEDATE
//                FROM USER_VERSES
//                WHERE USERNAME = :Username
//                  AND LAST_PRACTICED IS NOT NULL
//                ORDER BY LAST_PRACTICED DESC
//            )
//            WHERE ROWNUM <= :Limit
//        ";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var results = await conn.QueryAsync<UserPassage>(sql, new { Username = username, Limit = limit }, commandType: CommandType.Text);
//        return results.ToList();
//    }

//    public async Task<List<UserPassage>> GetOverdueVerses(string username)
//    {
//        var sql = @"
//            SELECT USER_VERSE_ID AS Id, USERNAME AS Username, COLLECTION_ID AS CollectionId, 
//                   READABLE_REFERENCE AS ReadableReference, LAST_PRACTICED AS LastPracticed,
//                   DATE_SAVED AS DateAdded, PROGRESS_PERCENT AS ProgressPercent, 
//                   TIMES_MEMORIZED AS TimesMemorized, DUE_DATE AS DUEDATE
//            FROM USER_VERSES
//            WHERE USERNAME = :Username
//              AND LAST_PRACTICED IS NOT NULL
//              AND TIMES_MEMORIZED > 0
//            ORDER BY LAST_PRACTICED ASC
//        ";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var results = await conn.QueryAsync<UserPassage>(sql, new 
//        { 
//            Username = username
//        }, commandType: CommandType.Text);

//        var allPracticedVerses = results.ToList();
//        var today = DateTime.Today;

//        var overdueList = new List<UserPassage>();
//        foreach (var uv in allPracticedVerses)
//        {
//            if (uv.DueDate.HasValue && uv.DueDate.Value.Date <= today)
//            {
//                overdueList.Add(uv);
//            }
//        }

//        overdueList = overdueList.OrderBy(uv => uv.DueDate).ToList();

//        return overdueList;
//    }

//    public async Task DeleteUserVersesNotInOrder(int collectionId, IEnumerable<string> readableReferences)
//    {
//        if (collectionId <= 0)
//        {
//            return;
//        }

//        var trimmedReferences = readableReferences?
//            .Select(r => r?.Trim())
//            .Where(r => !string.IsNullOrWhiteSpace(r))
//            .Distinct(StringComparer.OrdinalIgnoreCase)
//            .ToList() ?? new List<string>();

//        using IDbConnection conn = new OracleConnection(connectionString);

//        if (trimmedReferences.Count == 0)
//        {
//            var deleteAllSql = @"DELETE FROM USER_VERSES WHERE COLLECTION_ID = :CollectionId AND IN_PUBLISHED = 0";
//            await conn.ExecuteAsync(deleteAllSql, new { CollectionId = collectionId }, commandType: CommandType.Text);
//            return;
//        }

//        var parameters = new DynamicParameters();
//        parameters.Add("CollectionId", collectionId);

//        for (var i = 0; i < trimmedReferences.Count; i++)
//        {
//            parameters.Add($"Ref{i}", trimmedReferences[i]);
//        }

//        var parameterList = string.Join(", ", trimmedReferences.Select((_, idx) => $":Ref{idx}"));

//        var deleteSql = $@"
//            DELETE FROM USER_VERSES
//            WHERE COLLECTION_ID = :CollectionId
//              AND IN_PUBLISHED = 0
//              AND READABLE_REFERENCE NOT IN ({parameterList})";

//        await conn.ExecuteAsync(deleteSql, parameters, commandType: CommandType.Text);
//    }
//}
