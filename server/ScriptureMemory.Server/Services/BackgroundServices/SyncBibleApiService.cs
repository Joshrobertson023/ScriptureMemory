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
        // Sync chapter content, verse content, Bible metadata every 29 days
    }
}