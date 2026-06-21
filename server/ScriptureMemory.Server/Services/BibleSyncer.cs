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

    [Obsolete("Only used once to seed database with Bibles")]
    public async Task AddAuthorizedBiblesOnStartup()
    {
        foreach (var bible in Tools.Data.authorizedBibles)
        {
            _db.Bibles.Add(bible);
        }
        
        await _db.SaveChangesAsync();
        _logger.LogInformation($"{string.Join(", ", Tools.Data.authorizedBibles.Select(b => b.Version))} have been added/replaced.");
    }

    public async Task<string> GetChapterContentExample()
    {
        return await _bibleContext.GetFullChapter(
            _db.Bibles.Where(b => b.Version == "kjv").First(),
            new Reference(Books.GetBook("Genesis"), 1, new List<int>() { 1 }));
    }
}