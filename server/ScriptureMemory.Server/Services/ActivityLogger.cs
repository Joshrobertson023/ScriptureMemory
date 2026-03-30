using DataAccess.Data;
using DataAccess.Models;

namespace VerseAppNew.Server.Services;

public sealed class ActivityLogger
{
    private readonly ActivityLoggingData loggingData;
    private readonly ILogger<ActivityLogger> _logger;

    public ActivityLogger(ActivityLoggingData loggingData, ILogger<ActivityLogger> logger)
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
