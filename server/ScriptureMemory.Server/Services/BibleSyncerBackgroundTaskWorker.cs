using Microsoft.AspNetCore.SignalR;
using ScriptureMemory.Server.Data.Models;
using ScriptureMemory.Server.Data.Models.Logs;
using ScriptureMemory.Server.SignalR;

namespace ScriptureMemory.Server.Services;

/// <summary>
/// Assigned with dequeuing and executing the Bible syncer background tasks
/// </summary>
/// <param name="queue"></param>
/// <param name="logger"></param>
public class BibleSyncerBackgroundTaskWorker(
    BibleSyncerBackgroundTaskQueue queue,
    ILogger<BibleSyncerBackgroundTaskWorker> logger,
    DatabaseLogger dbLogger,
    IHubContext<LogHub> hubContext,
    BibleSyncer bibleSyncer) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("{Name} is running.", nameof(BibleSyncerBackgroundTaskWorker));

        while (!cancellationToken.IsCancellationRequested)
        {
            var workItem = await queue.DequeueAsync(cancellationToken);

            try
            {
                await dbLogger.LogBibleSyncEvent(new SyncLog
                {
                    Action = BibleSyncAction.Started,
                    SystemInitiated = true,
                    BibleId = workItem.BibleId
                });

                var progress = new Progress<SyncTaskProgressReport>(report =>
                {
                    _ = hubContext.Clients.All.SendAsync("ReceiveProgress", report);
                });

                await bibleSyncer.Sync(workItem, progress);

                await dbLogger.LogBibleSyncEvent(new SyncLog
                {
                    Action = BibleSyncAction.Completed,
                    SystemInitiated = true,
                    BibleId = workItem.BibleId
                });
            }
            catch (OperationCanceledException) when (!workItem.Cts.IsCancellationRequested)
            {
                logger.LogError("{Name} has been cancelled unexpectedly.", workItem.BibleId);
                await dbLogger.LogBibleSyncEvent(new SyncLog
                {
                    Action = BibleSyncAction.Cancelled, 
                    SystemInitiated = true,
                    BibleId = workItem.BibleId
                });
            }
            catch (OperationCanceledException) when (workItem.Cts.IsCancellationRequested)
            {
                logger.LogInformation("{Name} has been cancelled, skipping this item...", workItem.BibleId);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error executing the last work item {Name} in the background task queue: {Error}",
                    workItem.BibleId,
                    e.Message);
                await dbLogger.LogBibleSyncEvent(new SyncLog
                {
                    Action = BibleSyncAction.Stopped, 
                    SystemInitiated = true,
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