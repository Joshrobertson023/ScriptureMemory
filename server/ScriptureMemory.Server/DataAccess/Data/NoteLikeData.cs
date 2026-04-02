using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace DataAccess.Data;

public class NoteLikeData
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;

    public NoteLikeData(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetConnectionString("PostgresConnection")
            ?? throw new InvalidOperationException("Connection string 'PostgresConnection' not found");
    }

    public async Task LikeNote(int noteId, string username)
    {
        const string sql = @"
            INSERT INTO NOTE_LIKES (""NOTE_ID"", ""USERNAME"", ""CREATED_DATE"")
            VALUES (:NoteId, :Username, SYSDATE)";

        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteAsync(sql, new { NoteId = noteId, Username = username }, commandType: CommandType.Text);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            // User already liked this note, ignore
        }
    }

    public async Task UnlikeNote(int noteId, string username)
    {
        const string sql = @"DELETE FROM NOTE_LIKES WHERE ""NOTE_ID"" = :NoteId AND ""USERNAME"" = :Username";

        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { NoteId = noteId, Username = username }, commandType: CommandType.Text);
    }

    public async Task<bool> HasUserLikedNote(int noteId, string username)
    {
        const string sql = @"
            SELECT COUNT(*) 
            FROM NOTE_LIKES 
            WHERE ""NOTE_ID"" = :NoteId AND ""USERNAME"" = :Username";

        using var conn = new NpgsqlConnection(_connectionString);
        var count = await conn.QueryFirstOrDefaultAsync<int>(sql, new { NoteId = noteId, Username = username }, commandType: CommandType.Text);
        return count > 0;
    }

    public async Task<int> GetNoteLikeCount(int noteId)
    {
        const string sql = @"
            SELECT COUNT(*) 
            FROM NOTE_LIKES 
            WHERE ""NOTE_ID"" = :NoteId";

        using var conn = new NpgsqlConnection(_connectionString);
        return await conn.QueryFirstOrDefaultAsync<int>(sql, new { NoteId = noteId }, commandType: CommandType.Text);
    }

    public async Task<Dictionary<int, bool>> GetUserLikesForNotes(List<int> noteIds, string username)
    {
        if (noteIds == null || noteIds.Count == 0)
            return new Dictionary<int, bool>();

        var sql = @"
            SELECT ""NOTE_ID"" AS NoteId
            FROM NOTE_LIKES
            WHERE ""USERNAME"" = :Username
            AND ""NOTE_ID"" IN (";
        
        var parameters = new DynamicParameters();
        parameters.Add(":Username", username);
        
        for (int i = 0; i < noteIds.Count; i++)
        {
            if (i > 0) sql += ",";
            sql += $":NoteId{i}";
            parameters.Add($":NoteId{i}", noteIds[i]);
        }
        
        sql += ")";
        
        using var conn = new NpgsqlConnection(_connectionString);
        var likedNoteIds = await conn.QueryAsync<int>(sql, parameters, commandType: CommandType.Text);
        var likedSet = new HashSet<int>(likedNoteIds);
        
        return noteIds.ToDictionary(id => id, id => likedSet.Contains(id));
    }

    public async Task<Dictionary<int, int>> GetLikeCountsForNotes(List<int> noteIds)
    {
        if (noteIds == null || noteIds.Count == 0)
            return new Dictionary<int, int>();

        var sql = @"
            SELECT ""NOTE_ID"" AS NoteId, COUNT(*) AS LikeCount
            FROM NOTE_LIKES
            WHERE ""NOTE_ID"" IN (";
        
        var parameters = new DynamicParameters();
        
        for (int i = 0; i < noteIds.Count; i++)
        {
            if (i > 0) sql += ",";
            sql += $":NoteId{i}";
            parameters.Add($":NoteId{i}", noteIds[i]);
        }
        
        sql += @")
            GROUP BY ""NOTE_ID""";
        
        using var conn = new NpgsqlConnection(_connectionString);
        var results = await conn.QueryAsync<(int NoteId, int LikeCount)>(sql, parameters, commandType: CommandType.Text);
        
        var counts = results.ToDictionary(r => r.NoteId, r => r.LikeCount);
        
        // Ensure all note IDs are in the dictionary (with 0 likes if not found)
        return noteIds.ToDictionary(id => id, id => counts.GetValueOrDefault(id, 0));
    }
}































