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
public class BibleSyncerBackgroundWorker(
    BibleSyncerQueue queue,
    ILogger<BibleSyncerBackgroundWorker> logger,
    IHubContext<LogHub> hubContext,
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("{Name} is running.", nameof(BibleSyncerBackgroundWorker));

        var scope = scopeFactory.CreateScope();
        var progressLogger = scope.ServiceProvider.GetRequiredService<BibleSyncerProgressLogger>();
        var bibleSyncer = scope.ServiceProvider.GetRequiredService<BibleSyncer>();

        while (!cancellationToken.IsCancellationRequested)
        {
            var workItem = await queue.DequeueAsync(cancellationToken);

            try
            {
                if (workItem.Cts.IsCancellationRequested)
                {
                    logger.LogInformation("{Name} has been cancelled, skipping this item...", workItem.BibleId);
                    return;
                }

                await bibleSyncer.Sync(workItem);
            }
            catch (OperationCanceledException ex) when (!workItem.Cts.IsCancellationRequested)
            {
                logger.LogError("{Name} has been cancelled unexpectedly.", workItem.BibleId);
                await progressLogger.Update(new SyncProgressReport
                {
                    Action = BibleSyncAction.Cancelled,
                    SystemInitiated = true,
                    Percentage = 0,
                    BibleId = workItem.BibleId,
                    BibleName = workItem.BibleName,
                    Exception = new ExceptionModel(ex),
                    Initiator = $"Unexpected sync cancellation for {workItem.BibleName}"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing the last work item {Name} in the background task queue: {Error}",
                    workItem.BibleId,
                    ex.Message);
                await progressLogger.Update(new SyncProgressReport
                {
                    Action = BibleSyncAction.Stopped,
                    SystemInitiated = true,
                    Percentage = 0,
                    BibleId = workItem.BibleId,
                    BibleName = workItem.BibleName,
                    Exception = new ExceptionModel(ex),
                    Initiator = $"Unexpected error when syncing {workItem.BibleName}"
                });
                // Todo: Make sure when streaming the new content when syncing, to use a transaction, and rollback
                // before throwing again so that the exception propagates to here to log it
            }
            finally
            {
                queue.Cancel(workItem.BibleId);
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("{Name} has been stopped.", nameof(BibleSyncerBackgroundWorker));

        await base.StopAsync(cancellationToken);
    }
}