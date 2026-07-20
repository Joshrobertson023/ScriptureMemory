using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using ScriptureMemory.Server.CustomExceptions;
using ScriptureMemory.Server.Services;

namespace ScriptureMemory.Server.Data.DataAccess.Bible;

public class BibleRepository(
    BibleApi _bibleApi,
    BibleData _bibleData,
    IDistributedCache _distributedCache,
    ILogger<BibleRepository> _logger,
    BibleService _bibleService)
{
    public async Task<string> GetChapter(Server.DataAccess.Models.Bible bible, Reference reference)
    {
        await _bibleService.EnsureBibleAvailable(bible);
        
        
    }
}