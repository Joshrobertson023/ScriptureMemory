using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.Data.Models;

namespace ScriptureMemory.Server.Services;

public class BibleSyncer
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<BibleSyncer> _logger;
    private readonly BibleApi _bibleContext;
    private readonly IVerseData _verseData;

    public BibleSyncer(
        ApplicationDbContext db,
        ILogger<BibleSyncer> logger,
        BibleApi bibleContext,
        IVerseData verseData)
    {
        _dbContext = db;
        _logger = logger;
        _bibleContext = bibleContext;
        _verseData = verseData;
    }

    /// <summary>
    /// Return any Bibles that I am authorized which have not been added to the database yet
    /// </summary>
    /// <returns></returns>
    public async Task<List<Bible>> CheckForBiblesNeedingAdded()
    {
        List<Bible> biblesNeedingAdded = new();
        
        var uniqueVersions = _dbContext.VerseTranslationContents
            .DistinctBy(c => c.Version)
            .ToList();

        foreach (var version in uniqueVersions)
        {
            foreach (var authorizedBible in Bibles.authorizedBibles)
            {
                if (version.Version != authorizedBible.Version)
                    biblesNeedingAdded.Add(authorizedBible);
            }
        }
        
        return biblesNeedingAdded;
    }

    /// <summary>
    /// Return any verses who's content is not fully added (broken) for any Bible version
    /// </summary>
    /// <returns></returns>
    // public async Task<List<Verse>> CheckForIncompleteVerses()
    // {
    //     
    // }
    
    
    public async Task<string> GetChapterContentExample()
    {
        return await _bibleContext.GetFullChapter(
            _dbContext.Bibles.Where(b => b.Version == "kjv").First(),
            new Reference(Books.GetBook("Genesis"), 1, new List<int>() { 1 }));
    }
}