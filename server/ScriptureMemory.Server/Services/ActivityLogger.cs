using DataAccess.DataInterfaces;
using DataAccess.Models;

namespace VerseAppNew.Server.Services;

public interface IActivityLogger
{
    Task Log(ActivityLog log);
}

public sealed class ActivityLogger : IActivityLogger
{
    private readonly IActivityLoggingData loggingData;
    private readonly ILogger<ActivityLogger> _logger;

    public ActivityLogger(IActivityLoggingData loggingData, ILogger<ActivityLogger> logger)
    {
        this.loggingData = loggingData;
        _logger = logger;
    }

    public async Task Log(ActivityLog log)
    {
        if (log is null)
            throw new ArgumentNullException(nameof(log));

        _logger.LogInformation(log.ToString());
        await loggingData.Create(log);
    }
}