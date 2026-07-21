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
    ILogger<BibleRepository> _logger,
    BibleService _bibleService)
{
    public async Task<Chapter> GetChapter(Server.DataAccess.Models.Bible bible, string book, int chapterNum)
    {
        Chapter chapter = new();
        
        await _bibleService.EnsureBibleAvailable(bible);

        var cachedChapter = await _distributedCache.GetAsync(reference.CacheKey);

        if (cachedChapter is not null)
        {
            chapter = JsonSerializer.Deserialize<Chapter>(cachedChapter)
                ?? throw new Exception($"Error deserializing {nameof(Chapter)}");
            
            _logger.LogInformation("Chapter found in cache: {Reference}", reference.ReadableReference);
        }
        else
        {
            chapter.ContentUsx = await _bibleApi.GetFullChapter(bible, reference);
            chapter.Book = reference.Book;
            chapter.ChapterNum = reference.Chapter;
            chapter.Version = bible.Abbreviation;
            chapter.Id = chapter.GetId();

            var serializedChapter = JsonSerializer.Serialize(chapter);

            var cacheOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(CacheExpirations.ChapterContentExpiration);

            await _distributedCache.SetStringAsync(reference.CacheKey, serializedChapter, cacheOptions);
            
            _logger.LogInformation("Cached chapter: {Reference}", reference.ReadableReference);
        }

        return chapter;
    }
}