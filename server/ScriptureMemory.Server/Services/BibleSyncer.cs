using ScriptureMemory.Server.Data.Models;

namespace ScriptureMemory.Server.Services;

public class BibleSyncer
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<BibleSyncer> _logger;
    private readonly BibleApi _bibleContext;

    public BibleSyncer(
        ApplicationDbContext db,
        ILogger<BibleSyncer> logger,
        BibleApi bibleContext)
    {
        _db = db;
        _logger = logger;
        _bibleContext = bibleContext;
    }

    public async Task Sync(Bible bible)
    {
        
    }

    public async Task<string> GetChapterContentExample()
    {
        return await _bibleContext.GetFullChapter(
            _db.Bibles.Where(b => b.Version == "kjv").First(),
            new Reference(Books.GetBook("Genesis"), 1, new List<int>() { 1 }));
    }
}