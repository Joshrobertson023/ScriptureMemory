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
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _idCancellationTokens = new();
    
    private ILogger<BibleSyncerQueue> logger;

    public BibleSyncerQueue(ILogger<BibleSyncerQueue> logger)
    {
        this.logger = logger;
        
        var options = new BoundedChannelOptions(10) { FullMode = BoundedChannelFullMode.Wait };
        
        _queue = Channel.CreateBounded<BibleSyncerTask>(options);
        _idsInQueue = new();
        _syncingId = string.Empty;
    }

    public async Task EnqueueAsync(BibleSyncerTask task)
    {
        ArgumentNullException.ThrowIfNull(task);

        var cts = new CancellationTokenSource();
        
        _idCancellationTokens[task.BibleId] = cts;
        task.Cts = cts;
        
        await _queue.Writer.WriteAsync(task);
        _idsInQueue.Add(task.BibleId);
        
        logger.LogInformation("A Bible sync has been queued by {Username}: {BibleName}", 
            task.Initiator,
            task.BibleName);
    }

    public async Task<BibleSyncerTask> DequeueAsync(CancellationToken cancellationToken)
    {
        var task = await _queue.Reader.ReadAsync(cancellationToken);
        _idsInQueue.Remove(task.BibleId);
        _syncingId = task.BibleId;
        
        logger.LogInformation("A Bible sync has been dequeued for execution by the background worker: {MethodName}", 
            task.BibleName);

        return task;
    }

    public List<string> GetQueuedBibleIds()
    {
        return _idsInQueue.ToList();
    }

    public void Cancel(string bibleId)
    {
        RemoveIdFromHelpers(bibleId);

        if (!_idCancellationTokens.TryRemove(bibleId, out var removedCts))
            return;
        
        removedCts.Cancel();
        removedCts.Dispose();
    }

    public void Complete(string bibleId)
    {
        RemoveIdFromHelpers(bibleId);

        if (!_idCancellationTokens.TryRemove(bibleId, out var removedCts))
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