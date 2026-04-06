using Dapper;
using DataAccess.Models;
using Microsoft.Data.Sqlite;
using ScriptureMemory.Server.DataAccess.Models;

namespace ScriptureMemory.Server.DataAccess.Data;

public class CrossReferenceData
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;

    public CrossReferenceData(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetConnectionString("PostgresConnection")
            ?? throw new InvalidOperationException("Connection string 'PostgresConnection' not found");
    }

    public List<Passage> GetCrossReferences(int verseId)
    {
        using var conn = new SqliteConnection(_connectionString);

        conn.Query("""
            select 
            """)
    }
}
