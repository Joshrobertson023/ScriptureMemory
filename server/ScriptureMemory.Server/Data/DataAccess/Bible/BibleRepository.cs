using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using ScriptureMemory.Server.CustomExceptions;
using ScriptureMemory.Server.Data.DtoMappings;
using ScriptureMemory.Server.Data.Dtos;
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
    public async Task<ResponseChapterDto> GetChapterDto(Server.DataAccess.Models.Bible bible, Book book, int chapterNum)
    {
        Reference referenceToFetch = new(book, chapterNum);

        var cachedChapterDto = await _distributedCache.GetAsync(referenceToFetch.CacheKey);

        if (cachedChapterDto is not null)
        {
            ResponseChapterDto cacheChapterDto = JsonSerializer.Deserialize<ResponseChapterDto>(cachedChapterDto)
                ?? throw new Exception($"Error deserializing {nameof(Chapter)}");
            
            _logger.LogInformation("Chapter found in cache: {Reference}", referenceToFetch.ReadableReference);

            return cacheChapterDto;
        }

        var apiResponse = await _bibleApi.GetFullChapter(bible, referenceToFetch);

        ResponseChapterDto apiChapterDto = new ResponseChapterDto(
            apiResponse.Data.reference,
            apiResponse.Data.content,
            apiResponse.Data.copyright);

        var serializedChapter = JsonSerializer.Serialize(apiChapterDto);

        var cacheOptions = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(CacheExpirations.ChapterContentExpiration);

        await _distributedCache.SetStringAsync(referenceToFetch.CacheKey, serializedChapter, cacheOptions);
        
        _logger.LogInformation("Cached chapter: {Reference}", referenceToFetch.ReadableReference);
        
        return apiChapterDto;
    }
}