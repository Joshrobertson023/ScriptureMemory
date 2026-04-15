using Dapper;
using DataAccess.Models;
using Npgsql;
using ScriptureMemory.Server.DataAccess.Models;
using ScriptureMemory.Server.Tools;

namespace ScriptureMemory.Server.DataAccess.Data;

public class CategoryData
{
    private readonly NpgsqlDataSource _dataSource;

    public CategoryData(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
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

    public async Task UnassignVerseToCategory(int verseId, int categoryId)
    {
        using var conn = _dataSource.OpenConnection();
        await conn.ExecuteAsync(
            """
            delete from verse_categories
            where verse_id = @VerseId and category_id = @CategoryId
            """, new
            {
                VerseId = verseId,
                CategoryId = categoryId
            });
    }

    public async Task<List<Category>> GetCategories()
    {
        using var conn = _dataSource.OpenConnection();
        var results = await conn.QueryAsync<Category>(
            """
            select id, name, description
            from categories
            order by created_at desc
            """);
        return results.ToList();
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
    }

    public async Task<List<Verse>> GetVersesInCategory(int categoryId)
    {
        using var conn = _dataSource.OpenConnection();
        var results = await conn.QueryAsync<GetVerseDto>(
            """
            select v.id, v.book, v.chapter, v.verse_num as VerseNum, v.text, v.memorized_count as UsersMemorizedCount, v.saved_count as UsersSavedCount, v.saved_count as UsersSavedCount
            from verses v
            inner join verse_categories vc on vc.verse_id = v.id
            where vc.category_id = @CategoryId
            """, new { CategoryId = categoryId });
        return results.Select(dto => new Verse
        {
            Id = dto.Id,
            Reference = ReferenceParser.Parse(dto.Book, dto.Chapter, new List<int> { dto.VerseNum }),
            Text = dto.Text,
            UsersMemorizedCount = dto.UsersMemorizedCount,
            UsersSavedCount = dto.UsersSavedCount,
        }).ToList();
    }

    public async Task BulkAssignVersesToCategory(List<VerseCategory> assignments)
    {
        using var conn = _dataSource.OpenConnection();
        await conn.ExecuteAsync(
            """
        INSERT INTO verse_categories (verse_id, category_id, assignment_source, confidence)
        VALUES (@VerseId, @CategoryId, @AssignmentSource, @Confidence)
        ON CONFLICT (verse_id, category_id) DO UPDATE
            SET confidence = EXCLUDED.confidence,
                assignment_source = EXCLUDED.assignment_source
        """, assignments);
    }

    public async Task DeleteCategory(int categoryId)
    {
        using var conn = _dataSource.OpenConnection();
        using var transaction = await conn.BeginTransactionAsync();
        await conn.ExecuteAsync(
            """
            delete from verse_categories
            where category_id = @CategoryId
            """, new { CategoryId = categoryId }, transaction);
        await conn.ExecuteAsync(
            """
            delete from categories
            where id = @CategoryId
            """, new { CategoryId = categoryId }, transaction);
        await transaction.CommitAsync();
    }
}
