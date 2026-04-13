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

    public async Task InsertVerses(List<Verse> verses, int vodPassageId)
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

        var parameters = verses.Select(verse => new { VerseId = verse.Id, VodPassageId = vodPassageId });

        await conn.ExecuteAsync(sql, parameters);
    }

    public class GetActiveVod
    {
        public int Id { get; set; }
        public string Reference { get; set; } = "";
        public int AdminId { get; set; }
        public int OrderPosition { get; set; }
        public int VerseId { get; set; }
        public string Book { get; set; } = "";
        public int Chapter { get; set; }
        public int VerseNum { get; set; }
        public string Text { get; set; } = "";
        public int Memorized { get; set; }
        public int Saved { get; set; }
        public DateTime ActiveDate { get; set; }
    }

    public async Task<VerseOfDay?> GetActive()
    {
        var sql =
            """
            WITH anchor AS (
                SELECT 
                    p.order_position AS start_position,
                    a.first_position_date
                FROM vod_active a
                JOIN vod_passages p ON a.id = p.id
            ),
            days_elapsed AS (
                SELECT (CURRENT_DATE - anchor.first_position_date) AS elapsed
                FROM anchor
            ),
            ranked_passages AS (
                SELECT 
                    p.id,
                    p.reference,
                    p.admin_id,
                    p.order_position,
                    ROW_NUMBER() OVER (ORDER BY p.order_position) AS row_num
                FROM vod_passages p
            ),
            target AS (
                SELECT rp.*
                FROM ranked_passages rp, days_elapsed
                WHERE rp.row_num = days_elapsed.elapsed + 1  -- +1 because day 0 = first passage
            )
            SELECT 
                t.reference,
                t.admin_id AS AdminId,
                t.order_position AS OrderPosition,
                v.id AS VerseId,
                v.book AS Book,
                v.chapter AS Chapter,
                v.verse_num AS VerseNum,
                v.text AS Text,
                v.memorized_count AS Memorized,
                v.saved_count AS Saved
            FROM target t
            LEFT JOIN vod_passages_verses pv ON t.id = pv.vod_passage_id
            LEFT JOIN verses v ON pv.verse_id = v.id
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
                }).OrderBy(v => v.Reference.Verses.First()).ToList(),
                MostMemorized = g.Max(v => v.Memorized),
                MostSaved = g.Max(v => v.Saved),
                Date = DateTime.UtcNow.Date
            }).FirstOrDefault();

        return grouped;
    }

    public async Task<int> GetDaysUntilLastVod()
    {
        var sql =
            """
            WITH anchor AS (
                SELECT a.first_position_date
                FROM vod_active a
            ),
            days_elapsed AS (
                SELECT (CURRENT_DATE - anchor.first_position_date) AS elapsed
                FROM anchor
            ),
            total AS (
                SELECT COUNT(*) AS total_count FROM vod_passages
            )
            SELECT 
                (total.total_count - days_elapsed.elapsed - 1)::int AS days_remaining
            FROM total, days_elapsed
            """;

        await using var conn = new Npgsql.NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        return await conn.ExecuteScalarAsync<int>(sql);
    }

    public async Task ResetFirstVodDay()
    {
        using var conn = new Npgsql.NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        await conn.ExecuteAsync(
            """
            update vod_active
            set first_position_date = CURRENT_DATE
            """);
    }

    public async Task<List<VerseOfDay>> GetVods(int? page = null, int? pageSize = null)
    {
        if (page is null) page = 1;
        if (pageSize is null) pageSize = 100;

        var offset = (page - 1) * pageSize;

        var sql =
            $"""
                WITH anchor AS (
                    SELECT 
                        a.first_position_date
                    FROM vod_active a
                ),
                ranked_passages AS (
                    SELECT 
                        p.id,
                        p.reference,
                        p.admin_id,
                        p.order_position,
                        ROW_NUMBER() OVER (ORDER BY p.order_position) AS row_num
                    FROM vod_passages p
                )
                SELECT 
                    rp.id AS Id,
                    rp.reference,
                    rp.admin_id AS AdminId,
                    rp.order_position AS OrderPosition,
                    v.id AS VerseId,
                    v.book AS Book,
                    v.chapter AS Chapter,
                    v.verse_num AS VerseNum,
                    v.text AS Text,
                    v.memorized_count AS Memorized,
                    v.saved_count AS Saved,
                    (anchor.first_position_date + (rp.row_num - 1) * INTERVAL '1 day')::date AS ActiveDate
                FROM ranked_passages rp
                CROSS JOIN anchor
                LEFT JOIN vod_passages_verses pv ON rp.id = pv.vod_passage_id
                LEFT JOIN verses v ON pv.verse_id = v.id
                ORDER BY rp.order_position
                OFFSET {offset} LIMIT {pageSize}
                """;

        await using var conn = new Npgsql.NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        var results = await conn.QueryAsync<GetActiveVod>(sql);

        var grouped = results
            .GroupBy(r => new { r.Reference, r.AdminId, r.OrderPosition })
            .Select(g => new VerseOfDay
            {
                Id = g.Max(v => v.Id),
                Reference = g.Key.Reference,
                AdminId = g.Key.AdminId,
                OrderPosition = g.Key.OrderPosition,
                Date = g.Max(v => v.ActiveDate),
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

        DateTime now = DateTime.UtcNow.Date;
        var future = grouped.Where(r => r.Date >= now).ToList();
        var past = grouped.Where(r => r.Date < now).ToList();

        return future.Concat(past).ToList();
    }

    public async Task DeleteVod(int vodPassageId)
    {
        await using var conn = new Npgsql.NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // Find the row_num of the passage being deleted
        var rowNumSql = """
        SELECT row_num FROM (
            SELECT id, ROW_NUMBER() OVER (ORDER BY order_position) AS row_num
            FROM vod_passages
        ) ranked
        WHERE id = @Id
        """;

        var rowNum = await conn.ExecuteScalarAsync<int>(rowNumSql, new { Id = vodPassageId });

        // Find how many days have elapsed (passages already shown, 0-indexed)
        var elapsedSql = """
        SELECT (CURRENT_DATE - first_position_date) FROM vod_active
        """;

        var elapsed = await conn.ExecuteScalarAsync<int>(elapsedSql);

        // Don't delete past or today VOD
        if (rowNum <= elapsed + 1)
        {
            return;
        }

        await using var tx = await conn.BeginTransactionAsync();
        try
        {
            // Delete verse links first (FK constraint)
            await conn.ExecuteAsync(
                "DELETE FROM vod_passages_verses WHERE vod_passage_id = @Id",
                new { Id = vodPassageId }, tx);

            await conn.ExecuteAsync(
                "DELETE FROM vod_passages WHERE id = @Id",
                new { Id = vodPassageId }, tx);

            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}
