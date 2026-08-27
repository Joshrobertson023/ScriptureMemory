using ScriptureMemory.Server.Data.Dtos;
using System.Threading.Channels;

namespace ScriptureMemory.Server.Services;

public class VerseCacherJobQueue
{
    private readonly Channel<List<Verse>> _queue;
    private readonly ILogger<VerseCacherJobQueue> _logger;

    public VerseCacherJobQueue(ILogger<VerseCacherJobQueue> logger)
    {
        _logger = logger;

        var options = new BoundedChannelOptions(50) { FullMode = BoundedChannelFullMode.Wait };

        _queue = Channel.CreateBounded<List<Verse>>(options);
    }

    public async Task EnqueueAsync(List<Verse> verses)
    {
        ArgumentNullException.ThrowIfNull(verses);

        await _queue.Writer.WriteAsync(verses);

        _logger.LogInformation("Queued verse cache job");
    }

    public async Task<List<Verse>> DequeueAsync(CancellationToken cancellationToken)
    {
        var item = await _queue.Reader.ReadAsync(cancellationToken);

        _logger.LogInformation("Dequeued verse cache job");

        return item;
    }
}
