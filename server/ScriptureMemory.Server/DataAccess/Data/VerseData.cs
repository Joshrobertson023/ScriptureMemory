using Dapper;
using DataAccess.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ScriptureMemory.Server.Tools;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Npgsql;
using ScriptureMemory.Server.DataAccess.Models;
using Pgvector;

namespace DataAccess.Data;

public class VerseData
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;
    private readonly NpgsqlDataSource _dataSource;

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

    public VerseData(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetConnectionString("PostgresConnection")
            ?? throw new InvalidOperationException("Connection string 'PostgresConnection' not found");
        
        NpgsqlDataSourceBuilder builder = new NpgsqlDataSourceBuilder(_connectionString);
        builder.UseVector();

        _dataSource = builder.Build();
    }

    public async Task<List<Verse>> GetAllVerses(int offset, int nextFetch)
    {
        var sql = $@"SELECT * FROM VERSES OFFSET :offset ROWS FETCH NEXT :nextFetch ROWS ONLY";
        using var conn = _dataSource.OpenConnection();
        var results = await conn.QueryAsync<Verse>(sql, new { offset = offset, nextFetch = nextFetch });

        return results.ToList();
    }

    public async Task<List<Verse>> GetAllVerses()
    {
        var sql = $@"{selectSql} FROM VERSES";
        using var conn = _dataSource.OpenConnection();
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
        using var conn = _dataSource.OpenConnection();
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
        public string Book { get; set; }
        public int Chapter { get; set; }
        public string Text { get; set; }
        public int UsersMemorizedCount { get; set; }
        public int UsersSavedCount { get; set; }
        public int VerseNum { get; set; }
        public Vector? Embedding { get; set; }
        public double? Distance { get; set; }
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

        using var conn = _dataSource.OpenConnection();
        await conn.OpenAsync();

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

        using var conn = _dataSource.OpenConnection();
        await conn.OpenAsync();
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
            memorized_count as UsersMemorizedCount,
            saved_count as UsersSavedCount,
            verse_num as VerseNum
            from verses
            where book = @Book
            and chapter = @Chapter
            """;

        using var conn = _dataSource.OpenConnection();
        await conn.OpenAsync();

        var results = await conn.QueryAsync<GetVerseDto>(sql, new
        {
            Book = book,
            Chapter = chapter
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
            UsersMemorizedCount = r.UsersMemorizedCount
        }).ToList();
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

        using var conn = _dataSource.OpenConnection();

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
        using var conn = _dataSource.OpenConnection();
        await conn.OpenAsync();
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

    public async Task<List<Verse>> GetVersesSemanticSearch(Vector queryEmbedding)
    {
        using var conn = _dataSource.OpenConnection();
        var results = await conn.QueryAsync<GetVerseDto>(
            """
            select
            id as Id,
            book as Book,
            chapter as Chapter,
            text as Text,
            memorized_count as UsersMemorizedCount,
            saved_count as UsersSavedCount,
            verse_num as VerseNum,
            embedding <-> @queryEmbedding as distance
            from verses
            order by embedding <-> @queryEmbedding
            limit 25
            """, new
            {
                queryEmbedding
            });
        return results.Select(v => new Verse
        {
            Id = v.Id,
            Reference = new Reference
            {
                Book = v.Book,
                Chapter = v.Chapter,
                Verses = new List<int> { v.VerseNum }
            },
            Text = v.Text,
            UsersSavedCount = v.UsersSavedCount,
            UsersMemorizedCount = v.UsersMemorizedCount
        }).ToList();
    }

    public async Task UpdateUsersSavedVerse(int id)
    {
        var sql = @"UPDATE VERSES SET saved_count = saved_count + 1
                     WHERE id = @Id";
        using var conn = _dataSource.OpenConnection();
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    public async Task UpdateUsersMemorizedVerse(int id)
    {
        var sql = @"UPDATE VERSES SET memorized_count = memorized_count + 1
                     WHERE id = @Id";
        using var conn = _dataSource.OpenConnection();
        await conn.ExecuteAsync(sql, new { Id = id });
    }
}