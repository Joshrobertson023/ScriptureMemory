using ScriptureMemory.Server.Data.Models;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace ScriptureMemory.Server.Services;

/// <summary>
/// Manages the queue of background tasks to be completed for the Bible Syncer
/// </summary>
public class BibleSyncerQueue
{
    private readonly Channel<BibleSyncerTask> _queue;
    private readonly HashSet<string> _idsInQueue;
    private string _syncingId;
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _bibleIdCancellationTokens;
    
    private ILogger<BibleSyncerQueue> logger;

    public BibleSyncerQueue(ILogger<BibleSyncerQueue> logger)
    {
        this.logger = logger;
        
        var options = new BoundedChannelOptions(10) { FullMode = BoundedChannelFullMode.Wait };
        
        _queue = Channel.CreateBounded<BibleSyncerTask>(options);
        _idsInQueue = new();
        _syncingId = string.Empty;
        _bibleIdCancellationTokens = new();
    }

    public async Task EnqueueAsync(BibleSyncerTask task)
    {
        ArgumentNullException.ThrowIfNull(task);

        if (_idsInQueue.Contains(task.BibleId) || _syncingId == task.BibleId)
            return;

        var cts = new CancellationTokenSource();
        
        _bibleIdCancellationTokens[task.BibleId] = cts;
        task.Cts = cts;
        
        await _queue.Writer.WriteAsync(task);
        _idsInQueue.Add(task.BibleId);
        
        logger.LogInformation("Queued Bible sync by {Username}: {BibleName}", 
            task.Initiator,
            task.BibleName);
    }

    public async Task<BibleSyncerTask> DequeueAsync(CancellationToken cancellationToken)
    {
        var task = await _queue.Reader.ReadAsync(cancellationToken);
        _idsInQueue.Remove(task.BibleId);
        _syncingId = task.BibleId;
        
        logger.LogInformation("Dequeued for syncing by the background worker: {MethodName}", 
            task.BibleName);

        return task;
    }

    public async Task Clear()
    {
        while (_queue.Reader.TryRead(out _)) { }
        _idsInQueue.Clear();
        _syncingId = string.Empty;
    }

    public List<string> GetQueuedBibleIds()
    {
        return _idsInQueue.ToList();
    }

    public bool IsEmpty()
    {
        return !_queue.Reader.TryPeek(out _);
    }

    public void Cancel(string bibleId)
    {
        RemoveIdFromHelpers(bibleId);

        if (!_bibleIdCancellationTokens.TryRemove(bibleId, out var removedCts))
            return;
        
        removedCts.Cancel();
        removedCts.Dispose();
    }

    public void Complete(string bibleId)
    {
        RemoveIdFromHelpers(bibleId);

        if (!_bibleIdCancellationTokens.TryRemove(bibleId, out var removedCts))
            return;
        
        removedCts.Dispose();
    }

    public void RemoveIdFromHelpers(string bibleId)
    {
        if (_idsInQueue.Remove(bibleId))
            return;
        
        if (_syncingId == bibleId)
            _syncingId = string.Empty;
    }
}