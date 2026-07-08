using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.Data.Models.Logs;

namespace ScriptureMemory.Server.Services;

// Logs more important / specific logs to store in the database so they are easier to gather to show in a specfic place
public class DatabaseLogger(BibleSyncLogData logData)
{
    // Todo: Refactor to batch process these in a background task queue
    
    public async Task LogBibleSyncEvent(SyncLog log)
    {
        await logData.Log(log);
    }
}