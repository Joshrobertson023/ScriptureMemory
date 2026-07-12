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
        
        var lastSyncReports = await _syncLogContext.GetLastSyncProgressForBibles();
        var activeBibles = await _bibleContext.GetActiveBibles();
        var authorizedBibles = await _bibleApi.GetAuthorizedBibles();
        
        HashSet<string> activeBibleIds = activeBibles.Select(b => b.Id).ToHashSet();

        for (int i = 0; i < authorizedBibles.Count; i++)
        {
            var data = new BibleSyncData
            {
                Bible = authorizedBibles[i],
                LastSyncReport = lastSyncReports.GetValueOrDefault(authorizedBibles[i].Id)
            };
            if (activeBibleIds.Contains(authorizedBibles[i].Id))
                data.Bible.Active = true;
            
            dataToReturn.Add(data);
        }
        
        // Sync authorized Bibles with my database on a background task every day
        // On admin dashboard use my database Bibles, but have a button to start the sync with API.Bible
        
        // LastSynced, etc are optional
        // Once sync is completed, update LastSynced, NextScheduled, etc.
        // For updating the client, on Completion event, fetch the specific Bible that just synced from the server,
        // then update the last synced and next scheduled on the UI. Show a spinner in those fields until updated.
        
        // Server is source of truth for sync events and state
        // When clicking sync or cancel, show spinner until get confirmation from server
            // Add to array of waitingForSync or waitingForCancel
            // When event is received, check in there to remove

        return dataToReturn;
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
                await Task.Delay(random.Next(100, 300));
                

                // Get chapter usx and plaintext from API.Bible
                // Push to database
                // Then do same for each individual verse
                // Update IProgress progress every chapter
            }

            booksCompleted++;
            
            var percentage = (int)Math.Round((booksCompleted / (double)Books.TotalBooks) * 100);

            await _eventDispatcher.Send(new SyncEvent
            {
                BibleId = task.BibleId,
                BibleName = task.BibleName,
                Message = $"Completed book {book}",
                Percentage = percentage,
                Event = BibleSyncEvent.Progress
            });
        }
                
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