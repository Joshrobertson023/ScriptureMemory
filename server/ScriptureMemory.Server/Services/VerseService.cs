using DataAccess.Data;
using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.DataAccess.Models;
using ScriptureMemory.Server.DataAccess.Requests;

namespace VerseAppNew.Server.Services;

public sealed class VerseService
{
    private readonly VerseDataEfCore _verseContext;
    private readonly BibleApi _bibleApi;
    private readonly EmbeddingGenerator _embeddingGenerator;

    public VerseService(
        VerseDataEfCore verseContext,
        BibleApi bibleApi,
        EmbeddingGenerator embeddingGenerator)
    {
        _verseContext = verseContext;
        _bibleApi = bibleApi;
        _embeddingGenerator = embeddingGenerator;
    }

    public async Task<VerseTranslationContent> GetVerseTranslationContent(Verse verse, string version)
    {
        string plainText = string.Empty;
        string contentUsx = string.Empty;

        (plainText, contentUsx) = await _bibleApi.GetVerseUsxAndPlaintext(version, verse.Id);
        
        var newTranslationContent = new VerseTranslationContent
        {
            Version = version,
            PlainText = plainText,
            ContentUsx = contentUsx,
            LastUpdated = DateTime.UtcNow
        };

        newTranslationContent.Embedding = await _embeddingGenerator.GetVerseContentEmbedding(newTranslationContent);

        return newTranslationContent;
    }

    /// <summary>
    /// Adds a verse and its content for a Bible version into the database 
    /// </summary>
    public async Task AddNewVerse(string bookName, int chapter, int verse, string version)
    {
        Verse newVerse = new(bookName, chapter, verse);

        newVerse.TranslationContents = new List<VerseTranslationContent>()
        {
            await GetVerseTranslationContent(newVerse, version)
        };
        
        await _verseContext.InsertVerse(newVerse);
    }
}
