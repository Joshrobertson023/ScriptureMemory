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
    private readonly BibleSyncerQueue _queue;
    private readonly BibleSyncerEventDispatcher _eventDispatcher;
    private readonly BibleSyncLogData _syncLogContext;

    public BibleSyncer(
        ApplicationDbContext db,
        ILogger<BibleSyncer> logger,
        BibleApi bibleApi,
        BibleData bibleContext,
        IVerseData verseData,
        IServiceScopeFactory scope,
        BibleSyncerQueue queue,
        BibleSyncerEventDispatcher eventDispatcher,
        BibleSyncLogData syncLogContext)
    {
        _dbContext = db;
        _logger = logger;
        _bibleApi = bibleApi;
        _verseData = verseData;
        _bibleContext = bibleContext;
        _scope = scope;
        _queue = queue;
        _eventDispatcher = eventDispatcher;
        _syncLogContext = syncLogContext;
    }

    /// <summary>
    /// Gets initial load-in information
    /// </summary>
    /// <returns></returns>
    public async Task<List<BibleSyncData>> GetBibleSyncData()
    {
        List<BibleSyncData> dataToReturn = new();

        await SyncBibleAuthorization(); // Todo: Temporary until set up auto syncer
        
        var lastSyncReports = await _syncLogContext.GetLastSyncProgressForBibles();
        var biblesInDb = await _bibleContext.GetBibles();

        foreach (var bible in biblesInDb)
        {
            var data = new BibleSyncData
            {
                Bible = bible,
                LastSyncReport = lastSyncReports.GetValueOrDefault(bible.Id)
            };
            
            dataToReturn.Add(data);
        }
        
        // LastSynced, etc are optional
        // Once sync is completed, update LastSynced, NextScheduled, etc.
        // For updating the client, on Completion event, fetch the specific Bible that just synced from the server,
        // then update the last synced and next scheduled on the UI. Show a spinner in those fields until updated.

        return dataToReturn;
    }

    public async Task<List<Bible>> SyncBibleAuthorization(List<Bible>? authorizedBibles = null)
    {
        var dbBibles = await _bibleContext.GetBibles();
        
        if (authorizedBibles is null)
            authorizedBibles = await _bibleApi.GetAuthorizedBibles();
        
        (var mergedBibles, var needingLogged) = BibleHelper.MergeBiblesToSet(dbBibles, authorizedBibles);

        if (needingLogged.Count > 0)
        {
            foreach (var needed in needingLogged)
            {
                _logger.LogWarning("{Name} is not authorized but is active.", needed.AbbreviationLocal);
            }
        }

        await _bibleContext.UpdateAuthorizedBibles(mergedBibles);

        return mergedBibles;
    }

    public async Task QueueBibleForSync(string bibleId, string initiator)
    {
        var bibleName = await _bibleContext.GetBibleNameById(bibleId);
        
        await _queue.EnqueueAsync(new BibleSyncerTask
        {
            Initiator = initiator,
            BibleId = bibleId.Trim(),
            BibleName = bibleName
        });
        
        await _eventDispatcher.Send(new SyncEvent
        {
            BibleId = bibleId.Trim(),
            BibleName = bibleName,
            Initiator = initiator.Trim(),
            Event = BibleSyncEvent.Queued,
            Message = $"{initiator} queued {bibleName} for sync"
        });
    }

    public async Task CancelSync(string bibleId, string bibleName, string username)
    {
        try
        {
            _queue.Cancel(bibleId.Trim());
            
            await _eventDispatcher.Send(new SyncEvent
            {
                Event = BibleSyncEvent.Cancelled,
                BibleId = bibleId.Trim(),
                BibleName = bibleName.Trim(),
                Initiator = username.Trim(),
                Message = $"{username} cancelled sync for {bibleName}"
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    public async Task Sync(BibleSyncerTask task)
    {
        var taskCancellationToken = task.Cts.Token;
        int booksCompleted = 0;
        Random random = new();

        // Todo: delegate to background auto syncer when implemented
        DateTime nextScheduledAutoSync = DateTime.UtcNow.AddDays(29);

        await _eventDispatcher.Send(new SyncEvent
        {
            Event = BibleSyncEvent.Started,
            Initiator = "Background worker",
            Message = $"Sync started for {task.BibleName}",
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
                await Task.Delay(random.Next(1, 3));
                

                // Get chapter usx and plaintext from API.Bible
                // Push to database
                // Then do same for each individual verse
                // Update IProgress progress every chapter
            }

            booksCompleted++;
            
            var percentage = (int)Math.Round((booksCompleted / (double)Books.TotalBooks) * 100);

            _logger.LogInformation("Syncing {Name}: {Progress}", task.BibleName, percentage);
            await _eventDispatcher.Send(new SyncEvent
            {
                BibleId = task.BibleId,
                BibleName = task.BibleName,
                Message = $"Completed book {book}",
                Percentage = percentage,
                Event = BibleSyncEvent.Progress
            });
        }

        await _bibleContext.UpdateBibleSync(task.BibleId, DateTime.UtcNow, nextScheduledAutoSync);
                
        await _eventDispatcher.Send(new SyncEvent
        {
            BibleId = task.BibleId,
            BibleName = task.BibleName,
            Message = $"Completed sync for {task.BibleName}",
            Percentage = 100,
            Event = BibleSyncEvent.Completed
        });
    }


    public async Task<string> GetChapterContentExample()
    {
        return await _bibleApi.GetFullChapter(
            _dbContext.Bibles.Where(b => b.Abbreviation == "kjv").First(),
            new Reference(Books.TryGetBook("Genesis"), 1, new List<int>() { 1 }));
    }
}