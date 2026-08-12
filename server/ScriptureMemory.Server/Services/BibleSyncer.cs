using Microsoft.Extensions.Caching.Memory;
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
    private readonly AuthorizationSyncerData _authorizationSyncerActive;
    private readonly IMemoryCache _memoryCache;

    public BibleSyncer(
        ApplicationDbContext db,
        ILogger<BibleSyncer> logger,
        BibleApi bibleApi,
        BibleData bibleContext,
        IVerseData verseData,
        IServiceScopeFactory scope,
        BibleSyncerQueue queue,
        BibleSyncerEventDispatcher eventDispatcher,
        BibleSyncLogData syncLogContext,
        AuthorizationSyncerData authorizationSyncerActive,
        IMemoryCache memoryCache)
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
        _authorizationSyncerActive = authorizationSyncerActive;
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// Gets initial load-in information
    /// </summary>
    /// <returns></returns>
    public async Task<GetBibleSyncDataResponse> GetBibleSyncData()
    {
        GetBibleSyncDataResponse response = new();
        
        var lastSyncReports = await _syncLogContext.GetLastSyncProgressForBibles();
        var biblesInDb = await _bibleContext.GetBibles();

        foreach (var bible in biblesInDb)
        {
            var data = new BibleSyncData
            {
                Bible = bible,
                LastSyncReport = lastSyncReports.GetValueOrDefault(bible.Id),
            };
            
            response.SyncData.Add(data);
        }

        response.CurrentlySyncing = _authorizationSyncerActive.IsCurrentlySyncing();
        response.LastSync = await _syncLogContext.GetLastAuthorizationSync();
        
        return response;
    }

    public async Task SyncBibleAuthorization(string initiator, List<Bible>? authorizedBibles = null)
    {
        List<Bible> mergedBibles = new();
        int retries = 0;
        int maxRetries = 3;
        
        if (!_authorizationSyncerActive.SetActive())
            throw new InvalidOperationException("Syncer already active.");

        while (retries <= maxRetries)
        {

            try
            {
                await _eventDispatcher.Send(new SyncEvent
                {
                    AuthorizationSync = true, Initiator = initiator, Event = BibleSyncEvent.Started
                });

                var dbBibles = await _bibleContext.GetBibles();

                if (authorizedBibles is null)
                    authorizedBibles = await _bibleApi.GetAuthorizedBibles();

                (mergedBibles, var unauthorizedAndActive)
                    = BibleHelper.MergeBiblesToSet(dbBibles, authorizedBibles);

                if (unauthorizedAndActive.Count > 0)
                {
                    _logger.LogCritical("Found {Count} Bibles needing removal because unauthorized",
                        unauthorizedAndActive.Count);

                    // Send emails

                    foreach (var purging in unauthorizedAndActive)
                    {
                        await _eventDispatcher.Send(new SyncEvent
                        {
                            AuthorizationSync = true,
                            Event = BibleSyncEvent.StartedRemoval,
                            BibleId = purging.Id,
                            BibleName = purging.Abbreviation
                        });

                        // Todo: Implement this method
                        await PurgeBible(purging.Id);

                        await _eventDispatcher.Send(new SyncEvent
                        {
                            AuthorizationSync = true,
                            Event = BibleSyncEvent.CompletedRemoval,
                            BibleId = purging.Id,
                            BibleName = purging.Abbreviation
                        });
                    }
                }

                await _bibleContext.UpdateAuthorizedBibles(mergedBibles);

                break;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Error syncing Bible authorization: {Message}", ex.Message);
                await _eventDispatcher.Send(new SyncEvent
                {
                    AuthorizationSync = true, Event = BibleSyncEvent.Stopped, Exception = new ExceptionModel(ex)
                });
                _logger.LogCritical("Retrying authorization sync {Retry}/{MaxRetries}", retries += 1, maxRetries);
                retries++;
                await Task.Delay(1000*(retries*retries));

                // Send emails
            }
            finally
            {
            }
            
        }

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(CacheExpirations.AvailableBiblesExpiration);
        _memoryCache.Set(MemoryCacheKeys.AvailableBibles, await _bibleContext.GetAvailableBibles(), cacheEntryOptions);
        
        _authorizationSyncerActive.SetInactive();

        await _eventDispatcher.Send(new SyncEvent
        {
            AuthorizationSync = true,
            Event = BibleSyncEvent.Completed,
        });
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

    public async Task SetVisible(string bibleId, string username)
    {
        var bibleAbbr = await _bibleContext.SetBibleActive(bibleId);
        
        await _eventDispatcher.Send(new SyncEvent
        {
            BibleId = bibleId,
            BibleName = bibleAbbr,
            Event = BibleSyncEvent.SetActive,
            Initiator = username
        });
    }

    public async Task SetInvisible(string bibleId, string username)
    {
        var bibleAbbr = await _bibleContext.SetBibleInactive(bibleId);
        
        await _eventDispatcher.Send(new SyncEvent
        {
            BibleId = bibleId,
            BibleName = bibleAbbr,
            Event = BibleSyncEvent.SetInactive,
            Initiator = username
        });
    }

    public async Task PurgeBible(string bibleId)
    {
        await Task.Delay(3000);
    }
}