using Dapper;
using Npgsql;
using Pgvector;
using ScriptureMemory.Server.Data.Models;

namespace ScriptureMemory.Server.Data.DataAccess.Bible;

/// <summary>
/// Raw-SQL verse queries that don't fit cleanly into EF Core LINQ (pgvector similarity search,
/// multi-verse passage lookups joining Verses to VerseTranslationContents). Simple CRUD/lookups
/// live in <see cref="VerseDataEfCore"/> instead. Not IVerseData -- this class is injected
/// directly wherever raw semantic-search queries are needed (see BibleRepository).
/// </summary>
public class VerseDataDapper
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly ILogger<VerseDataDapper> _logger;

    public VerseDataDapper(
        NpgsqlDataSource dataSource,
        ILogger<VerseDataDapper> logger)
    {
        _dataSource = dataSource;
        _logger = logger;
    }

    private sealed class VerseContentDto
    {
        public string VerseId { get; set; } = string.Empty;
        public int Chapter { get; set; }
        public int[] VerseNumbers { get; set; } = Array.Empty<int>();
        public string BookDisplayName { get; set; } = string.Empty;
        public int MemorizedCount { get; set; }
        public int SavedCount { get; set; }
        public string PlainText { get; set; } = string.Empty;
        public string ContentUsx { get; set; } = string.Empty;
        public DateTime? LastUpdated { get; set; }
        public string Version { get; set; }
        public double Distance { get; set; }
        public Vector? Embedding { get; set; }
    }

    private static Verse MapVerse(VerseContentDto dto)
    {
        var book = new Book(dto.BookDisplayName);
        var verseNum = dto.VerseNumbers.FirstOrDefault();

        var verse = new Verse(book, dto.Chapter, verseNum)
        {
            MemorizedCount = dto.MemorizedCount,
            SavedCount = dto.SavedCount,
            SearchDistance = dto.Distance
        };

        var content = new VerseTranslationContent
        {
            VerseId = verse.Id,
            PlainText = dto.PlainText,
            ContentUsx = dto.ContentUsx,
            LastUpdated = dto.LastUpdated,
            VerseNavigation = verse,
            Version = dto.Version,
            Embedding = dto.Embedding
        };

        verse.TranslationContents = new List<VerseTranslationContent> { content };

        return verse;
    }

    public async Task AddVersionToAllVerses()
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        await connection.ExecuteAsync(
            $"""
             UPDATE "VerseTranslationContents"
             SET "Version" = @newVersion;
             """, new { newVersion = "kjv" });
    }

    public async Task<List<VerseEmbedding>> GetEmbeddingsForVerses(IEnumerable<string> verseIds)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        var verseIdArray = verseIds as string[] ?? verseIds.ToArray();

        var results = await connection.QueryAsync<VerseEmbedding>(
            """
        select "VerseId", "Embedding"
        from "VerseTranslationContents"
        where "VerseId" = any(@verseIdArray)
        """, new { verseIdArray });

        return results.AsList();
    }

    public async Task<List<Verse>> GetVersesFromIds(List<string> verseIds)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        var results = await connection.QueryAsync<VerseContentDto>(
            """
            select
            v."Id" as "VerseId",
            v."Reference_Chapter" as "Chapter",
            v."Reference_VerseNumbers" as "VerseNumbers",
            v."Reference_Book_DisplayName" as "BookDisplayName",
            v."MemorizedCount" as "MemorizedCount",
            v."SavedCount" as "SavedCount",
            vc."PlainText" as "PlainText",
            vc."ContentUsx" as "ContentUsx",
            vc."LastUpdated" as "LastUpdated",
            vc."Version" as "Version",
            vc."Embedding" as "Embedding"
            from "Verses" v
            join "VerseTranslationContents" vc
                on vc."VerseId" = v."Id"
            where v."Id" = any(@verseIds)
            and vc."Version" = 'kjv'
            """, new { verseIds });

        return results.Select(dto => MapVerse(dto)).ToList();
    }

    public async Task<Passage> GetPassage(Reference reference)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        var results = await connection.QueryAsync<VerseContentDto>(
            """
            select
            v."Id" as "VerseId",
            v."Reference_Chapter" as "Chapter",
            v."Reference_VerseNumbers" as "VerseNumbers",
            v."Reference_Book_DisplayName" as "BookDisplayName",
            v."MemorizedCount" as "MemorizedCount",
            v."SavedCount" as "SavedCount",
            vc."PlainText" as "PlainText",
            vc."ContentUsx" as "ContentUsx",
            vc."LastUpdated" as "LastUpdated",
            vc."Version" as "Version",
            vc."Embedding" as "Embedding"
            from "Verses" v
            join "VerseTranslationContents" vc
                on vc."VerseId" = v."Id"
            where v."Id" = any(@verseIds)
            and vc."Version" = 'kjv'
            """, new { verseIds = reference.VerseIds });

        var verses = results
            .Select(dto => MapVerse(dto))
            .OrderBy(v => v.Reference.VerseNumbers!.First())
            .ToList();

        return new Passage
        {
            Reference = reference,
            Verses = verses
        };
    }

    public async Task<Vector> GetEmbedding(string verseId)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        var result = await connection.QuerySingleAsync<Vector>(
            """
            select
            "Embedding"
            from "VerseTranslationContents"
            where "VerseId" = @verseId
            """, new { verseId });

        return result;
    }

    public async Task<List<Verse>> GetKjvContentForSemanticSearch(
        Vector queryEmbedding, 
        double? lastVerseDistance,
        int maxResults = 50)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        try
        {
            var results = await connection.QueryAsync<VerseContentDto>(
                """
                select
                v."Id" as "VerseId",
                v."Reference_Chapter" as "Chapter",
                v."Reference_VerseNumbers" as "VerseNumbers",
                v."Reference_Book_DisplayName" as "BookDisplayName",
                v."MemorizedCount" as "MemorizedCount",
                v."SavedCount" as "SavedCount",
                vc."PlainText" as "PlainText",
                vc."ContentUsx" as "ContentUsx",
                vc."LastUpdated" as "LastUpdated",
                (vc."Embedding" <=> @queryEmbedding::vector) as "Distance"
                from "Verses" v
                join "VerseTranslationContents" vc
                    on vc."VerseId" = v."Id"
                where vc."Version" = 'kjv'
                and vc."Embedding" is not null
                and (
                    @lastVerseDistance::float8 IS NULL
                    OR (vc."Embedding" <=> @queryEmbedding::vector) > @lastVerseDistance::float8
                )
                order by vc."Embedding" <=> @queryEmbedding::vector
                limit @maxResults
                """, new { queryEmbedding, maxResults, lastVerseDistance });

            return results.Select(dto => MapVerse(dto)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("GetKjvContentForSemanticSearch - " + ex.Message);

            return new List<Verse>();
        }
    }

    public async Task CreateVectorIndex()
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        await connection.ExecuteAsync("SET statement_timeout = '30min';");
        await connection.ExecuteAsync("SET maintenance_work_mem = '1GB';");

        await connection.ExecuteAsync(new CommandDefinition(
            """DROP INDEX IF EXISTS "IX_VerseTranslationContents_Embedding_Hnsw";""",
            commandTimeout: 60));

        await connection.ExecuteAsync(new CommandDefinition(
            """
            CREATE INDEX CONCURRENTLY "IX_VerseTranslationContents_Embedding_Hnsw"
            ON "VerseTranslationContents"
            USING hnsw ("Embedding" vector_cosine_ops);
            """,
            commandTimeout: 0));
    }

    public async Task<List<Verse>> GetKjvContentForSemanticSearch(
        IEnumerable<Vector> queryEmbeddings,
        string[] originalVerseIds,
        double? lastVerseDistance,
        int maxResults = 25)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        try
        {
            var results = await connection.QueryAsync<VerseContentDto>(
                """
                with candidates as (
                    select
                        nearest."VerseId",
                        nearest."Chapter",
                        nearest."Version",
                        nearest."VerseNumbers",
                        nearest."BookDisplayName",
                        nearest."MemorizedCount",
                        nearest."SavedCount",
                        nearest."PlainText",
                        nearest."ContentUsx",
                        nearest."LastUpdated",
                        nearest."Distance"
                    from unnest(@queryEmbeddings) as q(embedding)
                    cross join lateral (
                        select
                            v."Id"                         as "VerseId",
                            v."Reference_Chapter"          as "Chapter",
                            v."Reference_VerseNumbers"     as "VerseNumbers",
                            v."Reference_Book_DisplayName" as "BookDisplayName",
                            v."MemorizedCount"             as "MemorizedCount",
                            v."SavedCount"                 as "SavedCount",
                            vc."PlainText"                 as "PlainText",
                            vc."ContentUsx"                as "ContentUsx",
                            vc."LastUpdated"               as "LastUpdated",
                            vc."Version"                   as "Version",
                            vc."Embedding" <=> q.embedding as "Distance"
                        from "VerseTranslationContents" vc
                        join "Verses" v
                            on v."Id" = vc."VerseId"
                        where vc."Version" = 'kjv'
                          and NOT (v."Id" = ANY(@originalVerseIds))
                          and (
                            @lastVerseDistance::float8 IS NULL
                            OR (vc."Embedding" <=> q.embedding) > @lastVerseDistance::float8
                          )
                        order by vc."Embedding" <=> q.embedding
                        limit (@maxResults + 20)
                    ) as nearest
                )
                select
                    "VerseId",
                    "Chapter",
                    "VerseNumbers",
                    "BookDisplayName",
                    "MemorizedCount",
                    "SavedCount",
                    "PlainText",
                    "ContentUsx",
                    "Version",
                    "LastUpdated",
                    min("Distance") as "Distance"
                from candidates
                group by
                    "VerseId", "Chapter", "VerseNumbers", "BookDisplayName",
                    "MemorizedCount", "SavedCount", "PlainText", "ContentUsx", "Version", "LastUpdated"
                order by "Distance", "VerseId"
                limit @maxResults;
                """, new
                {
                    queryEmbeddings = queryEmbeddings.ToArray(), 
                    maxResults,
                    originalVerseIds,
                    lastVerseDistance
                });

            return results.Select(dto => MapVerse(dto)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("GetKjvContentForSemanticSearch - " + ex.Message);

            return new List<Verse>();
        }
    }

    class VerseCardDto
    {
        public string VerseId { get; set; } = string.Empty;
        public string VerseBookDisplayName { get; set; } = string.Empty;
        public int VerseChapter { get; set; }
        public int[]? VerseVerseNumbers { get; set; }
        public string? VerseReadableReference { get; set; }
        public int VerseTotalMemorizedCount { get; set; }
        public int VerseTotalSavedCount { get; set; }

        public int? CrossReferenceVotes { get; set; }
        public string? CrossReferencePassageId { get; set; }
        public string? CRPassageBookDisplayName { get; set; }
        public int? CRPassageChapter { get; set; }
        public int[]? CRPassageVerseNumbers { get; set; }
        public string? CRPassageReadableReference { get; set; }

        public string? CRVerseId { get; set; }
        public string? CRVerseBookDisplayName { get; set; }
        public int? CRVerseChapter { get; set; }
        public int[]? CRVerseVerseNumbers { get; set; }
        public string? CRVerseReadableReference { get; set; }
    }

    public async Task<VerseCardResponse> GetVerseCardResponse(int userId, List<string> verseIds)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();

        var result = (await conn.QueryAsync<VerseCardDto>(
        """
            select
            v."Id" as "VerseId",
            v."Reference_Book_DisplayName" as "VerseBookDisplayName",
            v."Reference_Chapter" as "VerseChapter",
            v."Reference_VerseNumbers" as "VerseVerseNumbers",
            v."Reference_ReadableReference" as "VerseReadableReference",
            v."MemorizedCount" as "VerseTotalMemorizedCount",
            v."SavedCount" as "VerseTotalSavedCount",
            cr."ToPassageId" as "CrossReferencePassageId",
            cr."Votes" as "CrossReferenceVotes",
            p."Reference_Book_DisplayName" as "CRPassageBookDisplayName",
            p."Reference_Chapter" as "CRPassageChapter",
            p."Reference_VerseNumbers" as "CRPassageVerseNumbers",
            p."Reference_ReadableReference" as "CRPassageReadableReference",
            crv."Id" as "CRVerseId",
            crv."Reference_Book_DisplayName" as "CRVerseBookDisplayName",
            crv."Reference_Chapter" as "CRVerseChapter",
            crv."Reference_VerseNumbers" as "CRVerseVerseNumbers",
            crv."Reference_ReadableReference" as "CRVerseReadableReference"
            from "Verses" v
            left join "CrossReferences" cr on cr."FromVerseId" = v."Id"
            left join "Passages" p on p."Id" = cr."ToPassageId"
            left join "PassageVerse" crpv on crpv."PassagesId" = cr."ToPassageId"
            left join "Verses" crv on crv."Id" = crpv."VersesId"
            where v."Id" = any(@verseIds)
        """, new
        {
            verseIds = verseIds.ToArray()
        })).GroupBy(dto => dto.VerseId).ToList();

        return new VerseCardResponse
        {
            TotalSaved = result.Sum(g => g.Max(r => r.VerseTotalSavedCount)),
            TotalMemorized = result.Sum(g => g.Max(r => r.VerseTotalMemorizedCount)),
            CrossReferences = result
                .Select(g => new CrossReferenceResponse
                {
                    FromVerse = new Verse
                    {
                        Reference = new Reference(
                            new Book(g.First().VerseBookDisplayName),
                            g.First().VerseChapter,
                            g.First().VerseVerseNumbers?.ToList() ?? new List<int>())
                        {
                            ReadableReference = g.First().VerseReadableReference
                        }
                    },
                    CrossReferences = g
                        .Where(r => r.CrossReferencePassageId is not null)
                        .GroupBy(r => r.CrossReferencePassageId!)
                        .Select(cr =>
                        {
                            var firstCr = cr.First();

                            return new Passage
                            {
                                Reference = new Reference(
                                    new Book(firstCr.CRPassageBookDisplayName),
                                    firstCr.CRPassageChapter.Value,
                                    firstCr.CRPassageVerseNumbers?.ToList() ?? new List<int>())
                                {
                                    ReadableReference = firstCr.CRPassageReadableReference
                                },
                                Verses = cr
                                    .DistinctBy(r => r.CRVerseId)
                                    .Where(r => r.CRVerseId is not null && r.CRVerseBookDisplayName is not null && r.CRVerseChapter is not null)
                                    .Select(r => new Verse
                                    {
                                        Reference = new Reference(
                                            new Book(r.CRVerseBookDisplayName!),
                                            r.CRVerseChapter!.Value,
                                            r.CRVerseVerseNumbers?.ToList() ?? new List<int>())
                                        {
                                            ReadableReference = r.CRVerseReadableReference
                                        }
                                    })
                                    .ToList()
                            };
                        })
                        .Where(p => p != null)
                        .Cast<Passage>()
                        .Where(p => p.Verses.Count > 0 && !verseIds.Contains(p.Verses.First().Id))
                        .ToList()
                })
                .Where(group => group.CrossReferences.Count > 0)
                .ToList(),
        };
    }
}