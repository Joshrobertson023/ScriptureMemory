using ScriptureMemory.Server.Data.Models.Logs;

namespace ScriptureMemory.Server.Data.DataAccess.Bible;

public class BibleSyncLogData
{
    private readonly ApplicationDbContext _dbContext;

    public BibleSyncLogData(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InsertLog(SyncLog log)
    {
        
    }
}