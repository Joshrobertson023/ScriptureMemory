using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using ScriptureMemory.Server.CustomExceptions;
using ScriptureMemory.Server.Data.Models;
using ScriptureMemory.Server.Services;

namespace ScriptureMemory.Server.Data.DataAccess.Bible;

public class BibleRepository(
    BibleApi _bibleApi,
    BibleData _bibleData,
    IDistributedCache _distributedCache,
    ILogger<BibleRepository> _logger)
{
    /// <summary>
    /// Gets a chapter from either the db or cache,and caches if not found in cache
    /// </summary>
    /// <param name="bible"></param>
    /// <param name="book"></param>
    /// <param name="chapterNum"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<Chapter> GetChapter(Server.DataAccess.Models.Bible bible, Book book, int chapterNum)
    {
        Chapter chapter = new();
        Reference referenceToFetch = new(book, chapterNum);

        var cachedChapter = await _distributedCache.GetAsync(referenceToFetch.CacheKey);

        if (cachedChapter is not null)
        {
            chapter = JsonSerializer.Deserialize<Chapter>(cachedChapter)
                ?? throw new Exception($"Error deserializing {nameof(Chapter)}");
            
            _logger.LogInformation("Chapter found in cache: {Reference}", referenceToFetch.ReadableReference);
        }
        else
        {
            chapter.ContentUsx = await _bibleApi.GetFullChapter(bible, referenceToFetch);
            chapter.Reference = referenceToFetch;
            chapter.Version = bible.Abbreviation;

            var serializedChapter = JsonSerializer.Serialize(chapter);

            var cacheOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(CacheExpirations.ChapterContentExpiration);

            await _distributedCache.SetStringAsync(referenceToFetch.CacheKey, serializedChapter, cacheOptions);
            
            _logger.LogInformation("Cached chapter: {Reference}", referenceToFetch.ReadableReference);
        }

        return chapter;
    }
}