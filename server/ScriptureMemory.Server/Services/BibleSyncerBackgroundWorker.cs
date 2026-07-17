using Microsoft.AspNetCore.SignalR;
using ScriptureMemory.Server.Data.Models;
using ScriptureMemory.Server.Data.Models.Logs;
using ScriptureMemory.Server.SignalR;

namespace ScriptureMemory.Server.Services;

/// <summary>
/// Handles automatically syncing Bible translations
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

        while (!cancellationToken.IsCancellationRequested)
        {
            var workItem = await queue.DequeueAsync(cancellationToken);
            
            await using var scope = scopeFactory.CreateAsyncScope();
            var eventDispatcher = scope.ServiceProvider.GetRequiredService<BibleSyncerEventDispatcher>();
            var bibleSyncer = scope.ServiceProvider.GetRequiredService<BibleSyncer>();

            try
            {
                if (workItem.Cts.IsCancellationRequested)
                {
                    logger.LogInformation("{Name} has been cancelled, Background Worker has skipped this item...", workItem.BibleId);
                    continue;
                }

                await bibleSyncer.Sync(workItem);
            }
            catch (OperationCanceledException ex) when (!workItem.Cts.IsCancellationRequested)
            {
                logger.LogError("{Name} has been cancelled unexpectedly.", workItem.BibleId);
                await eventDispatcher.Send(new SyncEvent
                {
                    Event = BibleSyncEvent.Cancelled,
                    Initiator = "Background worker",
                    Percentage = 0,
                    BibleId = workItem.BibleId,
                    BibleName = workItem.BibleName,
                    Exception = new ExceptionModel(ex),
                    Message = $"Unexpected sync cancellation for {workItem.BibleName}"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing the last work item {Name} in the background task queue: {Error}",
                    workItem.BibleId,
                    ex.Message);
                await eventDispatcher.Send(new SyncEvent
                {
                    Event = BibleSyncEvent.Stopped,
                    Initiator = "Background worker",
                    Percentage = 0,
                    BibleId = workItem.BibleId,
                    BibleName = workItem.BibleName,
                    Exception = new ExceptionModel(ex),
                    Message = $"Unexpected error when syncing {workItem.BibleName}"
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