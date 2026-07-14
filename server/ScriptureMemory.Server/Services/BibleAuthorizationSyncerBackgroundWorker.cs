using ScriptureMemory.Server.Data.DataAccess.Bible;

namespace ScriptureMemory.Server.Services;

public class BibleAuthorizationSyncerBackgroundWorker(
    BibleApi _bibleApi,
    BibleData _bibleData,
    BibleSyncer _bibleSyncer,
    ILogger<BibleAuthorizationSyncerBackgroundWorker> _logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var dbBibles = await _bibleData.GetBibles();
                var authorizedBibles = await _bibleApi.GetAuthorizedBibles();
                
                await _bibleData.SetBibles(_bibleSyncer.GetMergedBibles(dbBibles, authorizedBibles));
                
                // Make sure if API.Bible is down to cancel gracefully
            }
            catch (Exception e)
            {
                _logger.LogError("Error syncing Bible authorization: {Message}", e.Message);
            }
        }
    }
}