using Quartz;

namespace ScriptureMemory.Server.Services.BackgroundServices;

public class SyncBibleApiService : IJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ApplicationDbContext _dbContext;
    private readonly BibleSyncer _syncer;

    public SyncBibleApiService(
        IServiceProvider serviceProvider,
        ApplicationDbContext dbContext,
        BibleSyncer syncer)
    {
        _serviceProvider = serviceProvider;
        _dbContext = dbContext;
        _syncer = syncer;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var bibles = _dbContext.Bibles.ToList();
        
        var now = DateTime.UtcNow;

        foreach (var bible in bibles)
        {
            if (now - bible.LastSynced < TimeSpan.FromDays(29)) // If it's been 29 days
            {
                await _syncer.Sync(bible);
            }
        }
    }
}