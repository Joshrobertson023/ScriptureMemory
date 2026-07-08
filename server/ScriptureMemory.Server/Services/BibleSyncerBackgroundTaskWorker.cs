namespace ScriptureMemory.Server.Services;

/// <summary>
/// Assigned with dequeuing and executing the Bible syncer background tasks
/// </summary>
/// <param name="queue"></param>
/// <param name="logger"></param>
public class BibleSyncerBackgroundTaskWorker(
    BibleSyncerBackgroundTaskQueue queue,
    ILogger<BibleSyncerBackgroundTaskWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("{Name} is running.", nameof(BibleSyncerBackgroundTaskWorker));

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var workItem = await queue.DequeueAsync(cancellationToken);

                await workItem.InvokeAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Don't throw error when cancellation requested
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error executing the last work item in the background task queue");
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("{Name} has been stopped.", nameof(BibleSyncerBackgroundTaskWorker));

        await base.StopAsync(cancellationToken);
    }
}