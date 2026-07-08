using ScriptureMemory.Server.Data.Models;
using ScriptureMemory.Server.Data.Models.Logs;

namespace ScriptureMemory.Server.Services;

/// <summary>
/// Assigned with dequeuing and executing the Bible syncer background tasks
/// </summary>
/// <param name="queue"></param>
/// <param name="logger"></param>
public class BibleSyncerBackgroundTaskWorker(
    BibleSyncerBackgroundTaskQueue queue,
    ILogger<BibleSyncerBackgroundTaskWorker> logger,
    DatabaseLogger dbLogger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("{Name} is running.", nameof(BibleSyncerBackgroundTaskWorker));

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var workItem = await queue.DequeueAsync(cancellationToken);

                await dbLogger.LogBibleSyncEvent(new SyncLog
                {
                    Action = BibleSyncAction.Started, 
                    SystemInitiated = true, 
                    BibleId = workItem.BibleId,
                    Username = workItem.Initiator
                });

                await workItem.InvokeAsync(cancellationToken);

                await dbLogger.LogBibleSyncEvent(new SyncLog
                {
                    Action = BibleSyncAction.Completed, 
                    SystemInitiated = true, 
                    BibleId = workItem.BibleId,
                    Username = workItem.Initiator
                });
            }
            catch (OperationCanceledException)
            {
                await dbLogger.LogBibleSyncEvent(new SyncLog
                {
                    Action = BibleSyncAction.Stopped,
                    SystemInitiated = true
                });
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error executing the last work item in the background task queue");
                await dbLogger.LogBibleSyncEvent(new SyncLog
                {
                    Action = BibleSyncAction.Stopped, 
                    Exception = new ExceptionModel(e)
                });
                
                // Todo: Make sure when streaming the new content when syncing, to use a transaction, and rollback
                // before throwing again so that the exception propagates to here to log it
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("{Name} has been stopped.", nameof(BibleSyncerBackgroundTaskWorker));

        await base.StopAsync(cancellationToken);
    }
}