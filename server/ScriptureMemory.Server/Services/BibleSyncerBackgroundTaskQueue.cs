using ScriptureMemory.Server.Data.Models;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace ScriptureMemory.Server.Services;

/// <summary>
/// Manages the queue of background tasks to be completed for the Bible Syncer
/// </summary>
public class BibleSyncerBackgroundTaskQueue
{
    private readonly Channel<BibleSyncerTask> _queue;
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _cancellations = new();
    
    private ILogger<BibleSyncerBackgroundTaskQueue> logger;

    public BibleSyncerBackgroundTaskQueue(ILogger<BibleSyncerBackgroundTaskQueue> logger)
    {
        this.logger = logger;
        
        var options = new BoundedChannelOptions(10) { FullMode = BoundedChannelFullMode.Wait };
        
        _queue = Channel.CreateBounded<BibleSyncerTask>(options);
    }

    public async Task EnqueueAsync(BibleSyncerTask task)
    {
        ArgumentNullException.ThrowIfNull(task);

        var cts = new CancellationTokenSource();
        
        _cancellations[task.BibleId] = cts;
        task.Cts = cts;
        
        await _queue.Writer.WriteAsync(task);
        
        logger.LogInformation("A Bible sync has been queued by {Username}: {BibleName}", 
            task.Initiator,
            task.BibleName);
    }

    public async Task<BibleSyncerTask> DequeueAsync(CancellationToken cancellationToken)
    {
        var task = await _queue.Reader.ReadAsync(cancellationToken);
        
        logger.LogInformation("A Bible sync has been dequeued for execution by the background worker: {MethodName}", 
            task.BibleName);

        return task;
    }

    public void Cancel(string bibleId)
    {
        if (!_cancellations.TryRemove(bibleId, out var removedCts))
            return;
        
        removedCts.Cancel();
        removedCts.Dispose();
    }

    public void Complete(string bibleId)
    {
        if (!_cancellations.TryRemove(bibleId, out var cts))
            return;
        
        cts.Dispose();
    }
}