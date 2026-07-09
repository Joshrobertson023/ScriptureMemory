using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.Data.Models;
using ScriptureMemory.Server.Data.Models.Logs;
using System.Collections.Concurrent;

namespace ScriptureMemory.Server.Services;

/// <summary>
/// Adds tasks to the Bible Syncer background task queue
/// </summary>
public class BibleSyncer
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<BibleSyncer> _logger;
    private readonly BibleApi _bibleApi;
    private readonly BibleData _bibleContext;
    private readonly IVerseData _verseData;
    private readonly IServiceScopeFactory _scope;
    private readonly BibleSyncerBackgroundTaskQueue _queue;
    private readonly DatabaseLogger _dbLogger; 

    public BibleSyncer(
        ApplicationDbContext db,
        ILogger<BibleSyncer> logger,
        BibleApi bibleApi,
        BibleData bibleContext,
        IVerseData verseData,
        IServiceScopeFactory scope,
        BibleSyncerBackgroundTaskQueue queue,
        DatabaseLogger dbLogger)
    {
        _dbContext = db;
        _logger = logger;
        _bibleApi = bibleApi;
        _verseData = verseData;
        _bibleContext = bibleContext;
        _scope = scope;
        _queue = queue;
        _dbLogger = _dbLogger;
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

    public async Task QueueBibleForSync(CancellationToken cancellationToken, string bibleId, string username)
    {
        var scope = _scope.CreateScope();
        var queue = scope.ServiceProvider.GetRequiredService<BibleSyncerBackgroundTaskQueue>();

        if (cancellationToken.IsCancellationRequested)
            return;
        
        await queue.EnqueueAsync(new BibleSyncerTask
        {
            Initiator = username,
            BibleId = bibleId.Trim(),
            BibleName = await _bibleContext.GetBibleNameById(bibleId)
        });
        
        await _dbLogger.LogBibleSyncEvent(new SyncLog
        {
            BibleId = bibleId.Trim(),
            Username = username.Trim(),
            Action = BibleSyncAction.Queued
        });
    }

    public async Task CancelSync(string bibleId, string username)
    {
        _queue.Cancel(bibleId.Trim());
        
        await _dbLogger.LogBibleSyncEvent(new SyncLog
        {
            Action = BibleSyncAction.Cancelled,
            BibleId = bibleId.Trim(),
            Username = username.Trim()
        });
    }

    public async Task Sync(BibleSyncerTask task, IProgress<SyncTaskProgressReport> progress)
    {
        var taskCancellationToken = task.Cts.Token;
        var timeoutCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(taskCancellationToken);

        foreach (var book in Books.AllBooks)
        {
            taskCancellationToken.ThrowIfCancellationRequested();
            
            
        }
    }


    public async Task<string> GetChapterContentExample()
    {
        return await _bibleApi.GetFullChapter(
            _dbContext.Bibles.Where(b => b.Version == "kjv").First(),
            new Reference(Books.TryGetBook("Genesis"), 1, new List<int>() { 1 }));
    }
}