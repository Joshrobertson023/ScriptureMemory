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
}