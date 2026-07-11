namespace ScriptureMemory.Server.Data.DataAccess.Bible;

public class BibleData
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<BibleData> _logger;

    public BibleData(
        ApplicationDbContext dbContext,
        ILogger<BibleData> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<List<Server.DataAccess.Models.Bible>> GetBibles()
    {
        return await _dbContext.Bibles.ToListAsync();
    }

    public async Task<string> GetBibleNameById(string bibleId)
    {
        var bible = _dbContext.Bibles
            .SingleOrDefault(b => b.Id == bibleId.Trim());
        ArgumentNullException.ThrowIfNull(bible);
        return bible.Id!;
    }

    public async Task InsertBible(Server.DataAccess.Models.Bible bible)
    {
        await _dbContext.Bibles.AddAsync(bible);
        await _dbContext.SaveChangesAsync();
    }

    public async Task SetBibleActive(Server.DataAccess.Models.Bible bible)
    {
        var resultBible = await _dbContext.Bibles.SingleOrDefaultAsync(b => b.Id == bible.Id);
        resultBible?.Active = true;
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task SetBibleInactive(Server.DataAccess.Models.Bible bible)
    {
        var resultBible = await _dbContext.Bibles.SingleOrDefaultAsync(b => b.Id == bible.Id);
        resultBible?.Active = false;
        await _dbContext.SaveChangesAsync();
    }
}