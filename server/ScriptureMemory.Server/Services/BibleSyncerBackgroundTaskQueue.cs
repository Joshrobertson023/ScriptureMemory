using ScriptureMemory.Server.Data.Models;
using System.Threading.Channels;

namespace ScriptureMemory.Server.Services;

/// <summary>
/// Manages the queue of background tasks to be completed for the Bible Syncer
/// </summary>
public class BibleSyncerBackgroundTaskQueue
{
    private readonly Channel<BibleSyncerTask> _queue;
    
    private ILogger<BibleSyncerBackgroundTaskQueue> logger;

    public BibleSyncerBackgroundTaskQueue(ILogger<BibleSyncerBackgroundTaskQueue> logger)
    {
        this.logger = logger;
        
        var options = new BoundedChannelOptions(10) { FullMode = BoundedChannelFullMode.Wait };
        
        _queue = Channel.CreateBounded<BibleSyncerTask>(options);
    }

    public async Task QueueBackgroundWorkItemAsync(BibleSyncerTask task)
    {
        ArgumentNullException.ThrowIfNull(task);

        await _queue.Writer.WriteAsync(task);
        
        logger.LogInformation("A work item has been queued by {Username}: {MethodName}", 
            task.MethodName,
            task.Initializer);
    }

    public async Task<BibleSyncerTask> DequeueAsync(CancellationToken cancellationToken)
    {
        var task = await _queue.Reader.ReadAsync(cancellationToken);
        
        logger.LogInformation("A work item has been dequeued for execution by {Username}: {MethodName}", 
            task.MethodName,
            task.Initializer);

        return task;
    }
}