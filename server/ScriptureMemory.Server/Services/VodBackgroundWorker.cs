using Quartz;
using ScriptureMemory.Server.Data.Models.Vod;

namespace ScriptureMemory.Server.Services;

public class VodBackgroundWorker : IJob
{
    private ILogger<VodBackgroundWorker> _logger;
    private readonly HttpClient _http;
    private readonly VerseOfDayService _service;

    public VodBackgroundWorker(
        ILogger<VodBackgroundWorker> logger,
        HttpClient http,
        VerseOfDayService service)
    {
        _logger = logger;
        _http = http;
        _service = service;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Running Vod daily job");

        VerseOfDay result;
        Reference reference;

        reference = await _service.InsertTodayVod();

        _logger.LogInformation("Successfully inserted vod {Reference}", reference.VerseId);

        return;
    }
}
