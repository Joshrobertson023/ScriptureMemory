using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.Data.Models;
using ScriptureMemory.Server.Data.Models.Logs;
using ScriptureMemory.Server.Migrations;
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
    private readonly BibleSyncerQueue _queue;
    private readonly BibleSyncerProgressLogger _progressLogger;
    private readonly BibleSyncLogData _syncLogContext;

    public BibleSyncer(
        ApplicationDbContext db,
        ILogger<BibleSyncer> logger,
        BibleApi bibleApi,
        BibleData bibleContext,
        IVerseData verseData,
        IServiceScopeFactory scope,
        BibleSyncerQueue queue,
        BibleSyncerProgressLogger progressLogger,
        BibleSyncLogData syncLogContext)
    {
        _dbContext = db;
        _logger = logger;
        _bibleApi = bibleApi;
        _verseData = verseData;
        _bibleContext = bibleContext;
        _scope = scope;
        _queue = queue;
        _progressLogger = progressLogger;
        _syncLogContext = syncLogContext;
    }

    /// <summary>
    /// Gets initial load-in information
    /// </summary>
    /// <returns></returns>
    public async Task<List<BibleSyncData>> GetBibleSyncData()
    {
        List<BibleSyncData> dataToReturn = new();
        
        var authorizedBiblesTask = _bibleApi.GetAuthorizedBibles();
        var lastSyncReportsTask = _syncLogContext.GetLastSyncProgressForBibles();

        await Task.WhenAll(authorizedBiblesTask, lastSyncReportsTask);

        var authorizedBibles = authorizedBiblesTask.Result;
        var lastSyncReports = lastSyncReportsTask.Result;

        foreach (var bible in authorizedBibles)
        {
            dataToReturn.Add(new BibleSyncData
            {
                Bible = bible,
                LastSyncReport = lastSyncReports.GetValueOrDefault(bible.Id)
            });
        }

        return dataToReturn;
    }

    public async Task QueueBibleForSync(CancellationToken cancellationToken, string bibleId, string initiator)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        var bibleName = await _bibleContext.GetBibleNameById(bibleId);
        
        await _queue.EnqueueAsync(new BibleSyncerTask
        {
            Initiator = initiator,
            BibleId = bibleId.Trim(),
            BibleName = bibleName
        });
        
        await _progressLogger.Update(new SyncProgressReport
        {
            BibleId = bibleId.Trim(),
            BibleName = bibleName,
            Username = initiator.Trim(),
            Action = BibleSyncAction.Queued,
            Initiator = $"{initiator} queued {bibleName} for sync"
        });
    }

    public async Task CancelSync(string bibleId, string bibleName, string username)
    {
        _queue.Cancel(bibleId.Trim());
        
        await _progressLogger.Update(new SyncProgressReport
        {
            Action = BibleSyncAction.Cancelled,
            BibleId = bibleId.Trim(),
            BibleName = bibleName.Trim(),
            Username = username.Trim(),
            Initiator = $"{username} cancelled sync for {bibleName}"
        });
    }

    public async Task Sync(BibleSyncerTask task)
    {
        var taskCancellationToken = task.Cts.Token;
        int chaptersCompleted = 0;
        Random random = new();

        await _progressLogger.Update(new SyncProgressReport
        {
            Action = BibleSyncAction.Started,
            SystemInitiated = true,
            Initiator = $"Sync started for {task.BibleName}",
            BibleId = task.BibleId,
            BibleName = task.BibleName,
            Percentage = 0
        });

        foreach (var book in Books.AllBooks)
        {
            foreach (var chapterNum in Enumerable.Range(0, book.NumChapters))
            {
                if (taskCancellationToken.IsCancellationRequested)
                {
                    // Log where left off, rollback verse content
                    return;
                }

                // Simulate syncing for testing
                await Task.Delay(random.Next(100, 300));

                chaptersCompleted++;
                
                await _progressLogger.Update(new SyncProgressReport
                {
                    BibleId = task.BibleId,
                    BibleName = task.BibleName,
                    Initiator = $"Completed chapter {chapterNum} for {book.DisplayName}",
                    Percentage = chaptersCompleted / Books.TotalChapters,
                    Action = BibleSyncAction.Progress
                });

                // Get chapter usx and plaintext from API.Bible
                // Push to database
                // Then do same for each individual verse
                // Update IProgress progress every chapter
            }
        }
                
        await _progressLogger.Update(new SyncProgressReport
        {
            BibleId = task.BibleId,
            BibleName = task.BibleName,
            Initiator = $"Completed sync for {task.BibleName}",
            Percentage = 100,
            Action = BibleSyncAction.Completed
        });
    }


    public async Task<string> GetChapterContentExample()
    {
        return await _bibleApi.GetFullChapter(
            _dbContext.Bibles.Where(b => b.Abbreviation == "kjv").First(),
            new Reference(Books.TryGetBook("Genesis"), 1, new List<int>() { 1 }));
    }
}