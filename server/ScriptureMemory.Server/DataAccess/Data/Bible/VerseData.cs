using Dapper;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ScriptureMemory.Server.Tools;
using ScriptureMemory.Server.DataAccess.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ScriptureMemory.Server.DataAccess.Models;
using Pgvector;
using static ScriptureMemory.Server.Tools.Enums;

namespace DataAccess.Data;

public class VerseData
{
    private readonly IRequestDbConnection _requestDbConnection;

    private string selectSql = """
        select
        id as Id,
        book as Book,
        chapter as Chapter,
        text as Text,
        memorized_count as UsersMemorizedCount,
        saved_count as UsersSavedCount,
        verse_num as VerseNum,
        embedding as Embedding
        """;

    public VerseData(IRequestDbConnection requestDbConnection)
    {
        _requestDbConnection = requestDbConnection;
    }

    public async Task<List<Verse>> GetAllVerses(int offset, int nextFetch)
    {
        var sql = $@"SELECT * FROM VERSES OFFSET :offset ROWS FETCH NEXT :nextFetch ROWS ONLY";
        var conn = _requestDbConnection.Connection;
        var results = await conn.QueryAsync<Verse>(sql, new { offset = offset, nextFetch = nextFetch });

        return results.ToList();
    }

    public async Task<List<Verse>> GetAllVerses()
    {
        var sql = $@"{selectSql} FROM VERSES";
        var conn = _requestDbConnection.Connection;
        var results = await conn.QueryAsync<GetVerseDto>(sql);

        return results.Select(r => new Verse
        {
            Id = r.Id,
            Reference = new Reference
            {
                Book = r.Book,
                Chapter = r.Chapter,
                Verses = new List<int> { r.VerseNum },
            },
            Text = r.Text,
            UsersSavedCount = r.UsersSavedCount,
            UsersMemorizedCount = r.UsersMemorizedCount,
            Embedding = r.Embedding
        }).ToList();
    }

    public async Task InsertEmbedding(Verse verse)
    {
        var conn = _requestDbConnection.Connection;
        await conn.ExecuteAsync(
            """
            update verses 
            set embedding = @Embedding 
            where book = @Book 
            and chapter = @Chapter
            and verse_num = @VerseNum
            """, new
            {
                Embedding = verse.Embedding,
                Book = verse.Reference.Book,
                Chapter = verse.Reference.Chapter,
                VerseNum = verse.Reference.Verses.First()
            });
    }

    public class GetVerseDto
    {
        public int Id { get; set; }
        public string Book { get; set; } = string.Empty;
        public int Chapter { get; set; }
        public string Text { get; set; } = string.Empty;
        public int UsersMemorizedCount { get; set; }
        public int UsersSavedCount { get; set; }
        public int VerseNum { get; set; }
        public Vector? Embedding { get; set; }
        public double? Distance { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public async Task<IEnumerable<(Verse Verse, float Similarity)>> GetVersesBySimilarity(
        Vector embedding,
        float minSimilarity,
        int limit = 100)
    {
        var conn = _requestDbConnection.Connection;
        var results = await conn.QueryAsync<GetVerseDto>(
            """
        SELECT
            id AS Id,
            book AS Book,
            chapter AS Chapter,
            text AS Text,
            memorized_count AS UsersMemorizedCount,
            saved_count AS UsersSavedCount,
            verse_num AS VerseNum,
            (1 - (embedding <=> @embedding))::float4 AS Distance
        FROM verses
        WHERE (1 - (embedding <=> @embedding)) >= @minSimilarity
        ORDER BY embedding <=> @embedding
        LIMIT @limit
        """, new { embedding, minSimilarity, limit });

        return results.Select(r => (
            Verse: new Verse
            {
                Id = r.Id,
                Reference = new Reference
                {
                    Book = r.Book,
                    Chapter = r.Chapter,
                    Verses = new List<int> { r.VerseNum }
                },
                Text = r.Text,
                UsersSavedCount = r.UsersSavedCount,
                UsersMemorizedCount = r.UsersMemorizedCount,
                Embedding = r.Embedding
            },
            Similarity: (float)(r.Distance ?? 0)
        ));
    }

    public async Task<List<Verse>> GetVerses(string book, int chapter, List<int> verseNums)
    {
        var sql =
            """
            select
            id as Id,
            book as Book,
            chapter as Chapter,
            text as Text,
            memorized_count as UsersMemorizedCount,
            saved_count as UsersSavedCount,
            verse_num as VerseNum,
            embedding as Embedding
            from verses
            where book = @Book
            and chapter = @Chapter
            and verse_num = any(@VerseNums)
            """;

        var conn = _requestDbConnection.Connection;

        var results = await conn.QueryAsync<GetVerseDto>(sql, new
        {
            Book = book,
            Chapter = chapter,
            VerseNums = verseNums.ToArray()
        });

        return results.Select(r => new Verse
        {
            Id = r.Id,
            Reference = new Reference
            {
                Book = r.Book,
                Chapter = r.Chapter,
                Verses = new List<int> { r.VerseNum },
            },
            Text = r.Text,
            UsersSavedCount = r.UsersSavedCount,
            UsersMemorizedCount = r.UsersMemorizedCount,
            Embedding = r.Embedding
        }).ToList();
    }

    public async Task<Verse?> GetVerses(string book, int chapter, int verseNum)
    {
        var sql =
            """
            select
            id as Id,
            book as Book,
            chapter as Chapter,
            text as Text,
            memorized_count as UsersMemorizedCount,
            saved_count as UsersSavedCount,
            verse_num as VerseNum
            from verses
            where book = @Book
            and chapter = @Chapter
            and verse_num = @VerseNum
            """;

        var conn = _requestDbConnection.Connection;
        var result = await conn.QueryFirstOrDefaultAsync<GetVerseDto>(sql, new
        {
            Book = book,
            Chapter = chapter,
            VerseNum = verseNum
        });
        return result is not null
            ? new Verse
        {
            Id = result.Id,
            Reference = new Reference
            {
                Book = result.Book,
                Chapter = result.Chapter,
                Verses = new List<int> { result.VerseNum },
            },
            Text = result.Text,
            UsersSavedCount = result.UsersSavedCount,
            UsersMemorizedCount = result.UsersMemorizedCount
        }
        : null;
    }

    public async Task<List<Verse>> GetChapterVerses(string book, int chapter)
    {
        var sql =
            """
            select
            id as Id,
            book as Book,
            chapter as Chapter,
            text as Text,
            verse_num as VerseNum
            from verses
            where book = @Book
            and chapter = @Chapter
            """;

        var conn = _requestDbConnection.Connection;

        var results = await conn.QueryAsync<GetVerseDto>(sql, new
        {
            Book = book,
            Chapter = chapter
        });

        return results.Select(r => new Verse
        {
            Id = r.Id,
            Reference = ReferenceParser.Parse($"{r.Book} {r.Chapter}:{r.VerseNum}"),
            Text = r.Text,
            UsersSavedCount = r.UsersSavedCount,
            UsersMemorizedCount = r.UsersMemorizedCount
        }).OrderBy(v => v.Reference.Verses.First()).ToList();
    }

    public async Task<Verse?> GetVerseById(int id)
    {
        var sql =
            """
            select
            id as Id,
            book as Book,
            chapter as Chapter,
            text as Text,
            memorized_count as UsersMemorizedCount,
            saved_count as UsersSavedCount,
            verse_num as VerseNum
            from verses
            where id = @Id
            """;

        var conn = _requestDbConnection.Connection;

        var verses = await conn.QueryAsync<GetVerseDto>(sql, new { Id = id });

        return verses.FirstOrDefault() is not null
            ? new Verse
        {
            Id = verses.First().Id,
            Reference = new Reference
            {
                Book = verses.First().Book,
                Chapter = verses.First().Chapter,
                Verses = new List<int> { verses.First().VerseNum },
            },
            Text = verses.First().Text,
            UsersSavedCount = verses.First().UsersSavedCount,
            UsersMemorizedCount = verses.First().UsersMemorizedCount
        }
        : null;
    }

    public class GetPassageDto
    {

    }

    public async Task<Passage> GetPassage(Reference reference)
    {
        var conn = _requestDbConnection.Connection;
        var results = await conn.QueryAsync<GetVerseDto>(
            """
            select
            id as Id,
            book as Book,
            chapter as Chapter,
            text as Text,
            memorized_count as UsersMemorizedCount,
            saved_count as UsersSavedCount,
            verse_num as VerseNum
            from verses
            where book = @Book
            and chapter = @Chapter
            and verse_num = any(@VerseNums)
            """, new
            {
                Book = reference.Book,
                Chapter = reference.Chapter,
                VerseNums = reference.Verses
            });
        return new Passage
        {
            Reference = reference,
            Verses = results.Select(v => new Verse
            {
                Id = v.Id,
                Reference = new Reference
                {
                    Book = reference.Book,
                    Chapter = reference.Chapter,
                    Verses = new List<int> { v.VerseNum }
                },
                Text = v.Text,
                UsersSavedCount = v.UsersSavedCount,
                UsersMemorizedCount = v.UsersMemorizedCount
            }).ToList(),
        };
    }

    public async Task<List<Verse>> GetVersesSemanticSearch(Vector queryEmbedding, Passage? similarPassage = null)
    {
        int maxResults = 50;
        var conn = _requestDbConnection.Connection;
        var results = await conn.QueryAsync<GetVerseDto>(
            $"""
            select
            v.id as Id,
            v.book as Book,
            v.chapter as Chapter,
            v.text as Text,
            v.memorized_count as UsersMemorizedCount,
            v.saved_count as UsersSavedCount,
            v.verse_num as VerseNum,
            v.embedding <-> @queryEmbedding as distance,
            c.id as CategoryId,
            c.name as CategoryName
            from verses v 
            left join verse_categories vc on v.id = vc.verse_id
            left join categories c on vc.category_id = c.id
            order by v.embedding <-> @queryEmbedding
            limit {maxResults}
            """, new
            {
                queryEmbedding
            });
        var all = results
            .GroupBy(v => v.Id)
            .Select(g => new Verse
            {
                Id = g.First().Id,
                Reference = new Reference
                {
                    Book = g.First().Book,
                    Chapter = g.First().Chapter,
                    Verses = new List<int> { g.First().VerseNum }
                },
                Text = g.First().Text,
                Distance = g.First().Distance,
                UsersSavedCount = g.First().UsersSavedCount,
                UsersMemorizedCount = g.First().UsersMemorizedCount,
                Categories = g.Where(v => v.CategoryId != 0).Select(v => new Category
                {
                    Id = v.CategoryId,
                    Name = v.CategoryName
                }).ToList()
        }).ToList();

        if (similarPassage is not null)
        {
            return all;//.Where(v => v.Id != similarPassage.Verses.Any(v => v.Id).ToList();
        }
        else
        {
            return all;
        }
    }

    public async Task<List<Verse>> GetVersesSemanticSearch(List<Vector> queryEmbeddings)
    {
        var conn = _requestDbConnection.Connection;
        var results = await conn.QueryAsync<GetVerseDto>(
            """
            SELECT
                v.id,
                v.book,
                v.chapter,
                v.text,
                v.memorized_count as UsersMemorizedCount,
                v.saved_count as UsersSavedCount,
                v.verse_num as VerseNum,
                MIN(v.embedding <-> q.embedding) AS distance,
                c.id AS CategoryId,
                c.name AS CategoryName
            FROM verses v
            left join verse_categories vc on v.id = vc.verse_id
            left join categories c on vc.category_id = c.id
            CROSS JOIN UNNEST(@queryEmbeddings) AS q(embedding)
            GROUP BY v.id, c.id
            ORDER BY distance
            LIMIT 25;
            """, new
            {
                queryEmbeddings
            });
        return results
            .GroupBy(v => v.Id)
            .Select(g => new Verse
        {
            Id = g.First().Id,
            Reference = ReferenceParser.Parse($"{g.First().Book} {g.First().Chapter}:{g.First().VerseNum}") 
                ?? throw new Exception(),
            Text = g.First().Text,
            UsersSavedCount = g.First().UsersSavedCount,
            UsersMemorizedCount = g.First().UsersMemorizedCount,
            Categories = g
                .Where(c => c.CategoryId != 0)
                .Select(c => new Category
                {
                    Id = c.CategoryId,
                    Name = c.CategoryName,
                }).ToList()
        }).ToList();
    }

    public async Task UpdateUsersSavedVerse(int id)
    {
        var sql = @"UPDATE VERSES SET saved_count = saved_count + 1
                     WHERE id = @Id";
        var conn = _requestDbConnection.Connection;
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    public async Task UpdateUsersMemorizedVerse(int id)
    {
        var sql = @"UPDATE VERSES SET memorized_count = memorized_count + 1
                     WHERE id = @Id";
        var conn = _requestDbConnection.Connection;
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    class VerseCardDto
    {
        public int VerseId { get; set; }
        public string VerseBook { get; set; } = string.Empty;
        public int VerseChapter { get; set; }
        public int VerseNum { get; set; }
        public int VerseTotalMemorizedCount { get; set; }
        public int VerseTotalSavedCount { get; set; }
        public Vector? VerseEmbedding { get; set; }

        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public int? CrossReferenceVotes { get; set; }
        public string? CrossReferenceReference { get; set; }
        public string? CRText { get; set; }
        public int? CRVerseId { get; set; }
        public string? CRBook { get; set; }
        public int? CRChapter { get; set; }
        public int? CRVerseNum { get; set; }
        public int? CRMemorized { get; set; }
        public int? CRSaved { get; set; }

        public int? CollectionId { get; set; }
        public string? CollectionTitle { get; set; }
        public CollectionVisibility? CollectionVisibility { get; set; }
    }

    public async Task<VerseCardResponse> GetVerseCardResponse(int userId, List<int> verseIds)
    {
        var conn = _requestDbConnection.Connection;

        var results = await conn.QueryAsync<VerseCardDto>(
            """
            select
            v.id as VerseId,
            v.book as VerseBook,
            v.chapter as VerseChapter,
            v.verse_num as VerseNum,
            v.memorized_count as VerseTotalMemorizedCount,
            v.saved_count as VerseTotalSavedCount,
            c.id as CategoryId,
            c.name as CategoryName,
            crp.reference as CrossReferenceReference,
            cr.votes as CrossReferenceVotes,
            crv.text as CRText,
            crv.id as CRVerseId,
            crv.book as CRBook,
            crv.chapter as CRChapter,
            crv.verse_num as CRVerseNum,
            crv.memorized_count as CRMemorized,
            crv.saved_count as CRSaved,
            col.id as CollectionId,
            col.title as CollectionTitle,
            col.visibility as CollectionVisibility
            from verses v
            left join verse_categories vc on vc.verse_id = v.id
            left join categories c on c.id = vc.category_id
            left join cross_references cr on cr.from_verse_id = v.id
            left join cross_reference_passages_verses crpv on crpv.passage_id = cr.to_passage_id
            left join cross_reference_passages crp on crp.id = crpv.passage_id
            left join verses crv on crv.id = crpv.verse_id
            left join passages_verses pv on pv.verse_id = v.id
            left join collection_passages cp on cp.id = pv.passage_id
            left join collections col on col.id = cp.collection_id
                and col.is_deleted = false
                and col.user_id = @userId
            where v.id = any(@verseIds)
            """, new
            {
                verseIds = verseIds.ToArray(),
                userId
            });

        var allGroups = results.GroupBy(dto => dto.VerseId).ToList();
        if (!allGroups.Any())
            return new VerseCardResponse { };

        return new VerseCardResponse
        {
            TotalSaved = allGroups.Sum(g => g.Max(r => r.VerseTotalSavedCount)),
            TotalMemorized = allGroups.Sum(g => g.Max(r => r.VerseTotalMemorizedCount)),
            NumPracticed = 0,
            NextDue = DateTime.UtcNow,
            CrossReferences = allGroups
                .Select(g => new CrossReferenceResponse
                {
                    FromVerse = new Verse
                    {
                        Id = g.First().VerseId,
                        Reference = ReferenceParser.Parse(
                            g.First().VerseBook,
                            g.First().VerseChapter,
                            new List<int> { g.First().VerseNum }),
                        Text = string.Empty,
                        ReadableReference = ReferenceParser.Parse(
                            g.First().VerseBook,
                            g.First().VerseChapter,
                            new List<int> { g.First().VerseNum })?.ReadableReference
                    },
                    CrossReferences = g
                        .Where(r => r.CrossReferenceReference is not null)
                        .GroupBy(r => r.CrossReferenceReference!)
                        .Select(crGroup =>
                        {
                            var reference = ReferenceParser.Parse(crGroup.Key);
                            if (reference == null)
                                return null;

                            return new Passage
                            {
                                Reference = reference,
                                Verses = crGroup
                                    .DistinctBy(r => r.CRVerseId)
                                    .Where(r => r.CRVerseId.HasValue && r.CRBook != null && r.CRChapter.HasValue && r.CRVerseNum.HasValue)
                                    .Select(r => new Verse
                                    {
                                        Id = r.CRVerseId ?? 0,
                                        Reference = ReferenceParser.Parse(r.CRBook ?? "", r.CRChapter ?? 0, new List<int> { r.CRVerseNum ?? 0 }),
                                        Text = r.CRText ?? "",
                                        ReadableReference = $"{r.CRBook} {r.CRChapter}:{r.CRVerseNum}"
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
