using Microsoft.Extensions.Caching.Distributed;
using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.Data.Models;

namespace ScriptureMemory.Server.Services;

public class BibleService(
    BibleApi _bibleApi,
    BibleData _bibleData,
    IDistributedCache _cache,
    ILogger<BibleService> _logger)
{
    public async Task<string> GetFullChapter(string translation, string requestedBook, int chapter, int userId)
    {
        Reference reference = new Reference(requestedBook.Trim(), chapter);
        Bible bible = new Bible(translation.Trim());
        
        

        var result = await _bibleApi.GetFullChapter(bible, reference);

        return result;
    }
}