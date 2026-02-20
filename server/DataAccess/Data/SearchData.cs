using DataAccess.Models;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Dapper;
using DataAccess.DataInterfaces;
using static VerseAppLibrary.Enums;

namespace DataAccess.Data;

public class SearchData : ISearchData
{
    private readonly IConfiguration _config;
    private readonly string connectionString;

    public SearchData(IConfiguration config)
    {
        _config = config;
        connectionString = _config.GetConnectionString("Default")!;
    }

    public async Task TrackSearch(string searchTerm, SearchType searchType)
    {
        var sql = @"
            MERGE INTO SEARCHES s
            USING (SELECT :SearchTerm AS TERM FROM dual) new_search
            ON (UPPER(s.SEARCH_TERM) = UPPER(new_search.TERM))
            WHEN MATCHED THEN
                UPDATE SET 
                    SEARCH_COUNT = SEARCH_COUNT + 1
            WHEN NOT MATCHED THEN
                INSERT (SEARCH_TERM, SEARCH_COUNT, SEARCH_DATE, SEARCH_TYPE)
                VALUES (new_search.TERM, 1, :CurrentDate, :SearchType)
        ";

        await using var conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(
            sql, 
            new 
            { 
                SearchTerm = searchTerm.Trim(),
                CurrentDate = DateTime.UtcNow,
                SearchType = searchType
            });
    }

    public async Task InsertSearch(Search search)
    {
        var sql = @"
            INSERT INTO SEARCHES (SEARCH_TERM, SEARCH_COUNT, SEARCH_DATE, SEARCH_TYPE)
            VALUES (:SearchTerm, :SearchCount, :SearchDate, :SearchType)
        ";
        await using var conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new 
        { 
            SearchTerm = search.SearchTerm.Trim(),
            SearchCount = search.SearchCount,
            SearchDate = search.SearchDate,
            SearchType = search.SearchType
        });
    }

    public async Task<Search> GetSearch(int id)
    {
        var sql = @"
            SELECT ID, SEARCH_TERM AS SearchTerm, SEARCH_DATE AS SearchDate, 
                   SEARCH_TYPE AS SearchType, SEARCH_COUNT AS SearchCount
            FROM SEARCHES
            WHERE ID = :Id
        ";
        await using var conn = new OracleConnection(connectionString);
        var result = await conn.QueryFirstOrDefaultAsync<Search>(sql, new { Id = id }, commandType: CommandType.Text);
        return result;
    }



    public async Task<List<int>> GetTrendingSearchesFromSearches()
    {
        var sql = @"select 
                    id
                    from searches
                    where search_count > (
                        select percentile_cont(0.55) within group (order by search_count)
                        from searches
                    )
                    order by search_date desc
                    fetch first 10 rows only";

        await using var conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<int>(sql);
        return results.ToList();
    }

    public async Task InsertTrendingSearch(int searchId)
    {
        var sql = @"INSERT INTO TRENDING_SEARCHES 
                    (SEARCH_ID)
                    VALUES
                    (:SearchId)";

        await using var conn = new OracleConnection(connectionString);
        await conn.ExecuteAsync(sql, new { Searchid = searchId });
    }

    public async Task<List<Search>> GetTrendingSearches()
    {
        var sql = @"
            select
            s.id,
            s.search_term as SearchTerm,
            s.search_date as SearchDate,
            s.search_type as SearchType,
            s.search_count as SearchCount
            from searches s
            join trending_searches ts on ts.search_id = s.id
            where s.id = 1";

        using IDbConnection conn = new OracleConnection(connectionString);
        var results = await conn.QueryAsync<Search>(sql);
        return results.ToList();
    }
}

