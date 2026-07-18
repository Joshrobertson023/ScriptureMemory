using ScriptureMemory.Server.Data.Models;
using ScriptureMemory.Server.Data.Models.Logs;

namespace ScriptureMemory.Server.Data.DataAccess.Bible;

public class BibleSyncLogData
{
    private readonly ApplicationDbContext _dbContext;

    public BibleSyncLogData(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InsertLog(SyncEvent log)
    {
        await _dbContext.SyncProgressReports.AddAsync(log);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<SyncEvent>> GetSyncLogs(int position = 0)
    {
        return await _dbContext.SyncProgressReports
            .AsNoTracking()
            .OrderByDescending(l => l.Timestamp)
            .Skip(position)
            .Take(10)
            .ToListAsync();
    }

    public async Task<List<SyncEvent>> GetSyncLogsForBible(string bibleId, int position = 0)
    {
        return await _dbContext.SyncProgressReports
            .AsNoTracking()
            .OrderBy(l => l.Id)
            .Where(l => l.BibleId == bibleId.Trim())
            .Skip(position)
            .Take(10)
            .ToListAsync();
    }

    public void Log(SyncEvent log)
    {
        _dbContext.SyncProgressReports.Add(log);
        _dbContext.SaveChanges();
    }

    public async Task AddLogs(List<SyncEvent> logs)
    {
        await _dbContext.SyncProgressReports.AddRangeAsync(logs);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Dictionary<string, SyncEvent>> GetLastSyncProgressForBibles()
    {
        var reports = await _dbContext.SyncProgressReports
            .FromSql($@"
                select distinct on (""BibleId"") *
                from ""SyncProgressReports""
                where ""Event"" != 'Progress'
                order by ""BibleId"", ""Timestamp"" desc
            ")
            .ToListAsync();

        var returnDictionary = new Dictionary<string, SyncEvent>();

        foreach (var report in reports)
        {
            if (string.IsNullOrEmpty(report.BibleId))
                continue;
            
            returnDictionary[report.BibleId] = report;
        }

        return returnDictionary;
    }

    public async Task<DateTime?> GetLastAuthorizationSync()
    {
        var result = await _dbContext.SyncProgressReports
            .FirstOrDefaultAsync(r 
                => r.AuthorizationSync == true && r.Event == BibleSyncEvent.Completed);
        return result?.Timestamp;
    }
}