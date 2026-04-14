using DataAccess.Data;
using DataAccess.Models;
using Pgvector;
using ScriptureMemory.Server.DataAccess.Data;
using ScriptureMemory.Server.DataAccess.Models;

namespace ScriptureMemory.Server.Services;

public class CategoryService
{

    private readonly VerseData _verseData;
    private readonly CategoryData _categoryData;
    private readonly string _apiKey;

    public CategoryService(
        VerseData verseData,
        CategoryData categoryData,
        IConfiguration config)
    {
        _verseData = verseData;
        _categoryData = categoryData;
        _apiKey = config["OpenAi:OPENAI_API_KEY"]!;
    }

    public async Task AssignCategoryToAllVerses(Category category)
    {
        var verses = await _verseData.GetAllVerses();

        foreach (var verse in verses)
        {
            float similarity = CosineSimilarity(verse.Embedding, category.Embedding);

            if (similarity > 0.75f)
            {
                await _categoryData.AssignVerseToCategory(new VerseCategory
                {
                    VerseId = verse.Id,
                    CategoryId = category.Id,
                    AssignmentSource = "ai",
                    Confidence = similarity
                });
            }
        }
    }

    public async Task AssignVersesToNewCategory(Category category)
    {
        var verses = await _verseData.GetAllVerses();

        foreach (var verse in verses)
        {
            float similarity = CosineSimilarity(verse.Embedding, category.Embedding);
            if (similarity > 0.75f)
            {
                await _categoryData.AssignVerseToCategory(new VerseCategory
                {
                    VerseId = verse.Id,
                    CategoryId = category.Id,
                    AssignmentSource = "ai",
                    Confidence = similarity
                });
            }
        }
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

    float CosineSimilarity(Vector a, Vector b)
    {
        var va = a.ToArray();
        var vb = b.ToArray();
        float dot = va.Zip(vb, (x, y) => x * y).Sum();
        float magA = MathF.Sqrt(va.Sum(x => x * x));
        float magB = MathF.Sqrt(vb.Sum(x => x * x));
        return dot / (magA * magB);
    }
}
