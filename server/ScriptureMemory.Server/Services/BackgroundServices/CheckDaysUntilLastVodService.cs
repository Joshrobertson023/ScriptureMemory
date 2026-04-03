using DataAccess.Data;

namespace ScriptureMemory.Server.Services.BackgroundServices;

public class CheckDaysUntilLastVodService : BackgroundService
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<CheckDaysUntilLastVodService> logger;

    private const int CHECK_INTERVAL_HOURS = 24;

    public CheckDaysUntilLastVodService(IServiceProvider serviceProvider, ILogger<CheckDaysUntilLastVodService> logger)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var vodData = scope.ServiceProvider.GetRequiredService<VerseOfDayData>();
                    int days = await vodData.GetDaysUntilLastVod();

                    var vodService = scope.ServiceProvider.GetRequiredService<VerseOfDayService>();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while checking days until last VOD.");
            }

            await Task.Delay(TimeSpan.FromHours(CHECK_INTERVAL_HOURS), stoppingToken);
        }
    }
}
