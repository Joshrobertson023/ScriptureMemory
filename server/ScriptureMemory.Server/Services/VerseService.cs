using DataAccess.Data;
using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.DataAccess.Models;
using ScriptureMemory.Server.DataAccess.Requests;

namespace VerseAppNew.Server.Services;

public sealed class VerseService
{
    private readonly VerseDataEfCore _verseContext;
    private readonly BibleApi _bibleApi;

    public VerseService(
        VerseDataEfCore verseContext,
        BibleApi bibleApi)
    {
        _verseContext = verseContext;
        _bibleApi = bibleApi;
    }

    public async Task<List<VerseTranslationContent>> GetVerseContents(List<Bible> bibles, Verse verse)
    {
        List<VerseTranslationContent> contentToReturn = new();
        
        foreach (var bible in bibles)
        {
            contentToReturn.Add(new VerseTranslationContent
            {
                Version = bible.Version,
                ContentUsx = await _bibleApi.GetVerseUsx(bible, verse.Reference),
                LastUpdated = DateTime.UtcNow,
                VerseId = verse.Id,
                VerseNavigation = verse
            });
        }
        
        return contentToReturn;
        
        // Where left off. Create tests for this method and others.
        // Will have to add methods to add plain text and embeddings for verse translation contents
    }
}
