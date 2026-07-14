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
        return await _dbContext.Bibles.AsNoTracking().ToListAsync();
    }

    public async Task UpdateAuthorizedBibles(List<Server.DataAccess.Models.Bible> biblesToSet)
    {
        var biblesInDb = _dbContext.Bibles.AsNoTracking().ToList();
        HashSet<string> bibleIdsToSet = biblesToSet.Select(b => b.Id).ToHashSet();
        HashSet<string> bibleIdsInDb = biblesInDb.Select(b => b.Id).ToHashSet();
        
        foreach (var bible in biblesInDb)
        {
            if (bibleIdsToSet.Contains(bible.Id) && !bibleIdsInDb.Contains(bible.Id))
                _dbContext.Bibles.Add(bible);
        }
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Server.DataAccess.Models.Bible>> GetActiveBibles()
    {
        return await _dbContext.Bibles.AsNoTracking().Where(b => b.Active).ToListAsync();
    }

    public async Task<string> GetBibleNameById(string bibleId)
    {
        var bible = _dbContext.Bibles.AsNoTracking()
            .SingleOrDefault(b => b.Id == bibleId.Trim());
        if (bible is null)
            _logger.LogWarning("Bible is not in database. Restrict syncing bibles not active, or figure something else out.");
        return bible?.AbbreviationLocal ?? "";
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