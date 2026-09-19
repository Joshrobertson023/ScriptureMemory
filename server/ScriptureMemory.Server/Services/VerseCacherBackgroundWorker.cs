using Microsoft.Extensions.Caching.Distributed;
using ScriptureMemory.Server.Data.DataAccess.Bible;
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
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var cacheItem = await _queue.DequeueAsync(stoppingToken);

                using var scope = _scopeFactory.CreateScope();
                var verseData = scope.ServiceProvider.GetRequiredService<VerseDataDapper>();
                var distributedCache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();

                var verses = cacheItem.Verses;

                bool embeddingsMissing = verses.Any(v => v.TranslationContents.Any(c => c.Embedding is null));

                if (embeddingsMissing)
                {
                    _logger.LogWarning("Embedding missing in cache item, fetching from db...");

                    var embeddings = await verseData.GetEmbeddingsForVerses(verses.Select(v => v.Id));

                    foreach (var verse in verses)
                    {
                        var embedding = embeddings.FirstOrDefault(e => e.VerseId == verse.Id);
                        if (embedding != null && verse.TranslationContents.Any())
                        {
                            verse.TranslationContents.First().Embedding = embedding.Embedding;
                        }
                    }
                }

                foreach (var verse in verses)
                {
                    if (!CacheValidator.IsGoodToCache(verse))
                    {
                        _logger.LogWarning("Verse {Id} is not valid, skipping cache...", verse.Id);
                        continue;
                    }

                    var content = verse.TranslationContents.FirstOrDefault();

                    if (content == null) 
                        continue;

                    if (verse.Reference == null)
                    {
                        _logger.LogWarning("Cached a verse with a null reference.");
                    }

                    await distributedCache.SetStringAsync(
                        CacheKeyGenerator.GetVerseCacheKey(
                            verse.Id,
                            content.Version,
                            cacheItem.CacheType),
                        JsonSerializer.Serialize(verse, VectorJsonConverter.SerializerOptions),
                        stoppingToken);

                    _logger.LogInformation("Cached verse {Id}:{Translation}.", verse.Id, verse.TranslationContents.First().Version);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred background caching.");
            }
        }
    }


    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("{Name} has been stopped.", nameof(VerseCacherBackgroundWorker));
        await base.StopAsync(cancellationToken);
    }
}
