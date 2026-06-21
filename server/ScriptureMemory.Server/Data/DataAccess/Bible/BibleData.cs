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
    
    [Obsolete("Only used once to seed database with Bibles")]
    public async Task AddAuthorizedBiblesOnStartup()
    {
        var existingBibles = _dbContext.Bibles.ToList();
        
        foreach (var bible in Tools.Data.authorizedBibles)
        {
            if (existingBibles.Any(b => b.Version == bible.Version))
                continue;
            
            bible.LastUpdated = DateTime.UtcNow;
            
            _dbContext.Bibles.Add(bible);
        }
        
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation($"{string.Join(", ", Tools.Data.authorizedBibles.Select(b => b.Version))} have been added/replaced.");
    }
}