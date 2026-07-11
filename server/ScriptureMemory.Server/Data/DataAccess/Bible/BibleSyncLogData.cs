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

    public async Task InsertLog(SyncProgressReport log)
    {
        await _dbContext.SyncProgressReports.AddAsync(log);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<SyncProgressReport>> GetSyncLogs(int position = 0)
    {
        return await _dbContext.SyncProgressReports
            .AsNoTracking()
            .OrderBy(l => l.Id)
            .Skip(position)
            .Take(10)
            .ToListAsync();
    }

    public async Task<List<SyncProgressReport>> GetSyncLogsForBible(string bibleId, int position = 0)
    {
        return await _dbContext.SyncProgressReports
            .AsNoTracking()
            .OrderBy(l => l.Id)
            .Where(l => l.BibleId == bibleId.Trim())
            .Skip(position)
            .Take(10)
            .ToListAsync();
    }

    public async Task Log(SyncProgressReport log)
    {
        await _dbContext.SyncProgressReports.AddAsync(log);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddLogs(List<SyncProgressReport> logs)
    {
        await _dbContext.SyncProgressReports.AddRangeAsync(logs);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Dictionary<string, SyncProgressReport>> GetLastSyncProgressForBibles()
    {
        var reports = await _dbContext.SyncProgressReports
            .FromSql($@"
                select distinct on (BibleId) *
                from SyncProgressReports
            ")
            .ToListAsync();

        var returnDictionary = new Dictionary<string, SyncProgressReport>();

        foreach (var report in reports)
        {
            returnDictionary[report.BibleId] = report;
        }

        return returnDictionary;
    }
}