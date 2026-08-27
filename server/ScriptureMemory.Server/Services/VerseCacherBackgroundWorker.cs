using Microsoft.Extensions.Caching.Distributed;
using ScriptureMemory.Server.Data.Models;
using ScriptureMemory.Server.Tools;
using System.Text.Json;

namespace ScriptureMemory.Server.Services;

public class VerseCacherBackgroundWorker : BackgroundService
{
    private readonly VerseCacherQueue _queue;
    private readonly ILogger<VerseCacherBackgroundWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public VerseCacherBackgroundWorker(
        VerseCacherQueue queue,
        ILogger<VerseCacherBackgroundWorker> logger,
        IServiceScopeFactory scopeFactory)
    {
        _queue = queue;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{Name} is running.", nameof(VerseCacherBackgroundWorker));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var cacheItem = await _queue.DequeueAsync(stoppingToken);

                using var scope = _scopeFactory.CreateScope();
                var distributedCache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();

                if (!CacheValidator.IsGoodToCache(cacheItem.Verse))
                {
                    _logger.LogWarning(
                        "Cache item {Reference}:{Version} is not valid for cache, skipping...",
                        cacheItem.Verse.Id, 
                        cacheItem.Verse.TranslationContents.First().Version);
                    continue;
                }

                await distributedCache.SetStringAsync(
                    CacheKeyGenerator.GetVerseCacheKey(
                        cacheItem.Verse.Id,
                        cacheItem.Verse.TranslationContents.First().Version,
                        cacheItem.CacheType),
                    JsonSerializer.Serialize(
                        cacheItem.Verse,
                        new JsonSerializerOptions()
                        {
                            Converters = { new VectorJsonConverter() }
                        }),
                    new DistributedCacheEntryOptions().SetAbsoluteExpiration(CacheExpirations.VerseContentExpiration));

                _logger.LogInformation(
                    "Successfully cached item from worker: {Id}:{Translation}",
                    cacheItem.Verse.Id,
                    cacheItem.Verse.TranslationContents.First().Version);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error caching verse: " + ex.Message);
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("{Name} has been stopped.", nameof(VerseCacherBackgroundWorker));
        await base.StopAsync(cancellationToken);
    }
}
