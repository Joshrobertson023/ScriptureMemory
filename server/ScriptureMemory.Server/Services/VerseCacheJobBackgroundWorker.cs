namespace ScriptureMemory.Server.Services;

public class VerseCacheJobBackgroundWorker(
    VerseCacherJobQueue _queue,
    ILogger<VerseCacheJobBackgroundWorker> _logger,
    IServiceScopeFactory _scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{Name} is running.", nameof(VerseCacheJobBackgroundWorker));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var verses = await _queue.DequeueAsync(stoppingToken);

                using var scope = _scopeFactory.CreateScope();
                var bulkCacher = scope.ServiceProvider.GetRequiredService<BackgroundCacher>();

                //await bulkCacher.CleanVerseNumbers(verses);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in Chapter Formatter: " + ex.Message);
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("{Name} has been stopped.", nameof(VerseCacheJobBackgroundWorker));
        await base.StopAsync(cancellationToken);
    }
}
