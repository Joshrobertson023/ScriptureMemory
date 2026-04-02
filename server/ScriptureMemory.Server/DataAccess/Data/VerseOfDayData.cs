using DataAccess.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Data;

public class VerseOfDayData
{
    private readonly IConfiguration _config;
    private readonly string connectionString;

    public VerseOfDayData(IConfiguration config)
    {
        _config = config;
        connectionString = _config.GetConnectionString("PostgresConnection") 
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");
    }

    public async Task<int> InsertPassage(string reference, int adminId)
    {
        var sql =
            """
            insert into vod_passages
            (reference, admin_id, order_position)
            values
            (@Reference, @AdminId, (select coalesce(max(order_position), 0) + 1 from vod_passages))
            returning id
            """;

        await using var conn = new Npgsql.NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        var result = await conn.ExecuteScalarAsync<int>(sql, new { Reference = reference, AdminId = adminId });
        return result;
    }

    public async Task InsertVerses(List<int> verseIds, int vodPassageId)
    {
        var sql = 
            """
            insert into vod_passages_verses 
            (verse_id, vod_passage_id) 
            values 
            (@VerseId, @VodPassageId) 
            """;

        await using var conn = new Npgsql.NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        var parameters = verseIds.Select(id => new { VerseId = id, VodPassageId = vodPassageId });

        await conn.ExecuteAsync(sql, parameters);
    }

    public record GetActiveVod(
        string Reference,
        int AdminId,
        int OrderPosition,
        int VerseId,
        string Book,
        int Chapter,
        int VerseNum,
        string Text,
        int Memorized,
        int Saved
    );

    public async Task<VerseOfDay?> GetActive()
    {
        var sql =
            """
            select 
            p.reference,
            p.admin_id as AdminId,
            p.order_position as OrderPosition,
            v.id as VerseId,
            v.book as Book,
            v.chapter as Chapter,
            v.verse_num as VerseNum,
            v.text as Text,
            v.memorized_count as Memorized,
            v.saved_count as Saved
            from vod_passages p
            left join vod_passages_verses pv on p.id = pv.vod_passage_id
            left join vod_active a on p.id = a.id
            left join verses v on pv.verse_id = v.id
            """;

        await using var conn = new Npgsql.NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        var results = await conn.QueryAsync<GetActiveVod>(sql);
        var grouped = results.GroupBy(r => new { r.Reference, r.AdminId, r.OrderPosition })
            .Select(g => new VerseOfDay
            {
                Reference = g.Key.Reference,
                AdminId = g.Key.AdminId,
                OrderPosition = g.Key.OrderPosition,
                Verses = g.Select(v => new Verse
                {
                    Id = v.VerseId,
                    Reference = new Reference
                    {
                        Book = v.Book,
                        Chapter = v.Chapter,
                        Verses = new List<int> { v.VerseNum },
                        ReadableReference = $"{v.Book} {v.Chapter}:{v.VerseNum}"
                    },
                    Text = v.Text,
                }).ToList(),
                MostMemorized = g.Max(v => v.Memorized),
                MostSaved = g.Max(v => v.Saved)
            }).FirstOrDefault();

        return grouped;
    }

    public async Task<List<VerseOfDay>> GetVods(int? page = null, int? pageSize = null)
    {
        if (page is null) page = 1;
        if (pageSize is null) pageSize = 10;

        var offset = (page - 1) * pageSize;

        var sql =
            $"""
                select 
                p.reference,
                p.admin_id as AdminId,
                p.order_position as OrderPosition,
                v.id as VerseId,
                v.book as Book,
                v.chapter as Chapter,
                v.verse_num as VerseNum,
                v.text as Text,
                v.memorized_count as Memorized,
                v.saved_count as Saved
                from vod_passages p
                left join vod_passages_verses pv on p.id = pv.vod_passage_id
                left join vod_active a on p.id = a.id
                left join verses v on pv.verse_id = v.id
                order by p.order_position
                offset {offset} limit {pageSize}
                """;

        await using var conn = new Npgsql.NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        var results = await conn.QueryAsync<GetActiveVod>(sql);

        var grouped = results
            .GroupBy(r => new { r.Reference, r.AdminId, r.OrderPosition })
            .Select(g => new VerseOfDay
            {
                Reference = g.Key.Reference,
                AdminId = g.Key.AdminId,
                OrderPosition = g.Key.OrderPosition,
                Verses = g.Select(v => new Verse
                {
                    Id = v.VerseId,
                    Reference = new Reference
                    {
                        Book = v.Book,
                        Chapter = v.Chapter,
                        Verses = new List<int> { v.VerseNum },
                        ReadableReference = $"{v.Book} {v.Chapter}:{v.VerseNum}"
                    },
                    Text = v.Text
                }).ToList(),
                MostMemorized = g.Max(v => v.Memorized),
                MostSaved = g.Max(v => v.Saved)
            })
            .ToList();

        return grouped;
    }
}
