using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using ScriptureMemory.Server.CustomExceptions;
using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.Data.Models;

namespace ScriptureMemory.Server.Services;

public class BibleService(
    BibleApi _bibleApi,
    BibleData _bibleData,
    BibleRepository _bibleRepository,
    IDistributedCache _cache,
    ILogger<BibleService> _logger,
    IDistributedCache _distributedCache,
    IMemoryCache _memoryCache)
{
    public async Task<Chapter> GetChapter(int userId, string requestedBible, string requestedBook,
        int requestedChapterNum)
    {
        if (!AvailableBibles.TryGetBible(requestedBible, out var bible))
            throw new BibleUnavailableException($"Unavailable Bible {requestedBible}");

        if (!Books.TryGetBook(requestedBook, out var book))
            throw new BookNotFoundException($"Book {requestedBook} was not found.");

        book!.EnsureValidChapter(requestedChapterNum);

        var result = await _bibleRepository.GetChapter(bible!, book!, requestedChapterNum);

        return result;
    }

    public async Task EnsureBibleAvailable(Server.DataAccess.Models.Bible bible)
    {
        var availableBibles = await GetAvailableBibles();

        HashSet<string> availableBibleIds = availableBibles.Select(b => b.Id).ToHashSet();

        if (!availableBibleIds.Contains(bible.Id))
            throw new BibleUnavailableException("Bible is not available.", bible.Abbreviation);
    }

    private async Task<List<Server.DataAccess.Models.Bible>> GetAvailableBibles()
    {
        if (!_memoryCache.TryGetValue(
                MemoryCacheKeys.AvailableBibles, 
                out List<Server.DataAccess.Models.Bible>? availableBibles))
        {
            _logger.LogInformation("No available Bibles in cache, fetching from db.");
            
            availableBibles = await _bibleData.GetAvailableBibles();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(CacheExpirations.AvailableBiblesExpiration);

            _memoryCache.Set(MemoryCacheKeys.AvailableBibles, availableBibles, cacheEntryOptions);
        }
        else
        {
            _logger.LogInformation("Available Bibles found in cache.");
        }

        return availableBibles ?? throw new NullReferenceException(nameof(availableBibles));
    }
}