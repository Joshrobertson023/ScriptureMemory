using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.Data.Models;
using System.Collections.Concurrent;

namespace ScriptureMemory.Server.Services;

/// <summary>
/// Adds tasks to the Bible Syncer background task queue
/// </summary>
public class BibleSyncerService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<BibleSyncerService> _logger;
    private readonly BibleApi _bibleApi;
    private readonly BibleData _bibleContext;
    private readonly IVerseData _verseData;

    public BibleSyncerService(
        ApplicationDbContext db,
        ILogger<BibleSyncerService> logger,
        BibleApi bibleApi,
        BibleData bibleContext,
        IVerseData verseData)
    {
        _dbContext = db;
        _logger = logger;
        _bibleApi = bibleApi;
        _verseData = verseData;
        _bibleContext = bibleContext;
    }

    /// <summary>
    /// Gets authorized Bible data, showing which Bibles are not in my database or are not authorized
    /// </summary>
    /// <returns></returns>
    public async Task<List<BibleSyncData>> GetBibleSyncData()
    {
        List<BibleSyncData> dataToReturn = new();
        
        var biblesInDatabase = await _bibleContext.GetBibles();
        var authorizedBibles = await _bibleApi.GetAuthorizedBibles();

        var bibleIdsInDatabase = biblesInDatabase.Select(b => b.Id).ToHashSet();
        var authorizedBibleIds = authorizedBibles.Select(b => b.Id).ToHashSet();

        var allBibles = biblesInDatabase.Concat(authorizedBibles).DistinctBy(b => b.Id).ToList();

        foreach (var bible in allBibles)
        {
            dataToReturn.Add(new BibleSyncData
            {
                Bible = bible,
                Authorized = authorizedBibleIds.Contains(bible.Id),
                InDatabase = bibleIdsInDatabase.Contains(bible.Id),
            });
        }

        return dataToReturn;
    }

    public async Task<List<string>> GetSyncingBibles()
    {
        var scope = 
    }

    public async Task StartSync(string bibleId, int userId)
    {
        
    }
    
    
    public async Task<string> GetChapterContentExample()
    {
        return await _bibleApi.GetFullChapter(
            _dbContext.Bibles.Where(b => b.Version == "kjv").First(),
            new Reference(Books.TryGetBook("Genesis"), 1, new List<int>() { 1 }));
    }
}