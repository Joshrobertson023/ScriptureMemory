using BenchmarkDotNet.Loggers;
using Microsoft.Extensions.Caching.Distributed;
using Pgvector;
using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.Data.Dtos;
using ScriptureMemory.Server.Data.Models;
using ScriptureMemory.Server.Services;
using System.Text;
using System.Text.RegularExpressions;

namespace ScriptureMemory.Server.Tools;

public class BackgroundCacher(
    VerseCacherQueue _cacheQueue,
    VerseDataDapper _verseData,
    ILogger<BackgroundCacher> _logger)
{
    public async Task Cache(List<Verse> verses)
    {
        List<VerseEmbedding> embeddings;
        bool embeddingsMissing = false;

        foreach (var verse in verses)
        {
            foreach (var contents in verse.TranslationContents)
            {
                if (contents.Embedding is null)
                {
                    embeddingsMissing = true;
                    break;
                }
            }
        }

        if (embeddingsMissing)
        {
            _logger.LogWarning("Embedding was missing in background cacher, fetching from db...");

            embeddings = await _verseData.GetEmbeddingsForVerses(verses.Select(v => v.Id));

            for (int i = 0; i < verses.Count; i++)
            {
                VerseEmbedding embedding = embeddings.First(e => e.VerseId == verses[i].Id);

                verses[i].TranslationContents.First().Embedding = embedding.Embedding;
            }
        }

        foreach (var verse in verses)
        {
            await _cacheQueue.EnqueueAsync(new CacheQueueItem()
            {
                Verse = verse,
                CacheType = MemoryCacheType.PlainText
            });
        }
    }
}
