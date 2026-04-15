using DataAccess.Data;
using DataAccess.Models;
using Pgvector;
using ScriptureMemory.Server.DataAccess.Data;
using ScriptureMemory.Server.DataAccess.Models;

public class CategoryService
{
    private readonly VerseData _verseData;
    private readonly CategoryData _categoryData;

    private const float DefaultSimilarityThreshold = 0.35f;
    private const int NumToAssign = 500;

    public CategoryService(VerseData verseData, CategoryData categoryData)
    {
        _verseData = verseData;
        _categoryData = categoryData;
    }

    public async Task AssignVersesToCategory(
        Category category,
        string source = "ai",
        float threshold = DefaultSimilarityThreshold)
    {
        var matches = await _verseData.GetVersesBySimilarity(category.Embedding, threshold, NumToAssign);

        var assignments = matches.Select(m => new VerseCategory
        {
            VerseId = m.Verse.Id,
            CategoryId = category.Id,
            AssignmentSource = source,
            Confidence = m.Similarity
        });

        await _categoryData.BulkAssignVersesToCategory(assignments.ToList());
    }

    public async Task AssignVerseToCategory(Verse verse, Category category)
    {
        float similarity = CosineSimilarity(verse.Embedding, category.Embedding);

        await _categoryData.AssignVerseToCategory(new VerseCategory
        {
            VerseId = verse.Id,
            CategoryId = category.Id,
            AssignmentSource = "admin",
            Confidence = similarity
        });
    }

    private static float CosineSimilarity(Vector a, Vector b)
    {
        var va = a.ToArray();
        var vb = b.ToArray();

        float dot = 0f, magA = 0f, magB = 0f;
        for (int i = 0; i < va.Length; i++)
        {
            dot += va[i] * vb[i];
            magA += va[i] * va[i];
            magB += vb[i] * vb[i];
        }

        return dot / (MathF.Sqrt(magA) * MathF.Sqrt(magB));
    }
}