using DataAccess.Data;
using DataAccess.Models;
using ScriptureMemory.Server.DataAccess.Models;

namespace VerseAppNew.Server.Services;

public sealed class ActivityLogger
{
    private readonly ActivityLoggingData loggingData;
    private readonly AdminData adminData;
    private readonly ILogger<ActivityLogger> _logger;

    public ActivityLogger(
        ActivityLoggingData loggingData,
        AdminData adminData,
        ILogger<ActivityLogger> logger)
    {
        this.loggingData = loggingData;
        this.adminData = adminData;
        _logger = logger;
    }

    public async Task Log(ActivityLog log)
    {
        await loggingData.Create(log);
    }

    public async Task LogAdminAction(AdminAction action)
    {
        await adminData.InsertAdminAction(action);
    }
}
