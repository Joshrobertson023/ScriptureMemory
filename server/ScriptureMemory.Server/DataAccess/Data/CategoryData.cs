using Dapper;
using DataAccess.Models;
using Npgsql;
using ScriptureMemory.Server.DataAccess.Models;

namespace ScriptureMemory.Server.DataAccess.Data;

public class CategoryData
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;
    private readonly NpgsqlDataSource _dataSource;

    public CategoryData(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetConnectionString("PostgresConnection")
            ?? throw new InvalidOperationException("Connection string 'PostgresConnection' not found");

        NpgsqlDataSourceBuilder builder = new NpgsqlDataSourceBuilder(_connectionString);
        builder.UseVector();

        _dataSource = builder.Build();
    }

    public async Task<int> CreateCategory(Category newCategory)
    {
        using var conn = _dataSource.OpenConnection();
        var sql = """
            insert into categories (name, description, embedding, created_at)
            values (@Name, @Description, @Embedding, @CreatedAt)
            returning id
            """;
        var id = await conn.ExecuteScalarAsync<int>(sql, new
        {
            Name = newCategory.Name,
            Description = newCategory.Description,
            Embedding = newCategory.Embedding,
            CreatedAt = DateTime.UtcNow
        });
        return id;
    }

    public async Task AssignVerseToCategory(VerseCategory vc)
    {
        using var conn = _dataSource.OpenConnection();
        await conn.ExecuteAsync(
            """
            insert into verse_categories
            (verse_id, category_id, assignment_source, confidence)
            values
            (@VerseId, @CategoryId, @AssignmentSource, @Confidence)
            """, new
            {
                vc.VerseId,
                vc.CategoryId,
                vc.AssignmentSource,
                vc.Confidence
            });
    }
}
