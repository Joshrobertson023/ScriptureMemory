using DataAccess.Data;
using VerseAppNew.Server.Services;

namespace ScriptureMemory.Server.Services.BackgroundServices;

public class CheckDaysUntilLastVodService : BackgroundService
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<CheckDaysUntilLastVodService> logger;

    private static readonly TimeZoneInfo EasternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
    private const int RUN_HOUR = 8;

    public CheckDaysUntilLastVodService(IServiceProvider serviceProvider, ILogger<CheckDaysUntilLastVodService> logger)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await WaitUntilNextRunTime(stoppingToken);

            try
            {
                using var scope = serviceProvider.CreateScope();

                var vodData = scope.ServiceProvider.GetRequiredService<VerseOfDayData>();
                int days = await vodData.GetDaysUntilLastVod();

                var emailService = scope.ServiceProvider.GetRequiredService<EmailSenderService>();
                await emailService.NotifyAdminsUpcomingLastVod(days);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while checking days until last VOD.");
            }
        }
    }

    private async Task WaitUntilNextRunTime(CancellationToken stoppingToken)
    {
        var nowEastern = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, EasternZone);
        var nextRun = nowEastern.Date.AddHours(RUN_HOUR);

        // If 8am has already passed today, schedule for tomorrow
        if (nowEastern >= nextRun)
            nextRun = nextRun.AddDays(1);

        var delay = nextRun - nowEastern;
        logger.LogInformation("Next VOD check scheduled in {Delay} at {NextRun} Eastern", delay, nextRun);

        await Task.Delay(delay, stoppingToken);
    }
}
