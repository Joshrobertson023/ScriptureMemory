using ScriptureMemory.Server.Data.Models;
using System.Threading.Channels;

namespace ScriptureMemory.Server.Services;

public class VerseCacherQueue
{
    private readonly Channel<CacheQueueItem> _queue;
    private readonly ILogger<VerseCacherJobQueue> _logger;

    public VerseCacherQueue(ILogger<VerseCacherJobQueue> logger)
    {
        _logger = logger;

        var options = new BoundedChannelOptions(100) { FullMode = BoundedChannelFullMode.Wait };

        _queue = Channel.CreateBounded<CacheQueueItem>(options);
    }

    public async Task EnqueueAsync(CacheQueueItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        await _queue.Writer.WriteAsync(item);

        _logger.LogInformation("Queued verse for caching: {VerseId}", item.Verse.Id);
    }

    public async Task<CacheQueueItem> DequeueAsync(CancellationToken cancellationToken)
    {
        var item = await _queue.Reader.ReadAsync(cancellationToken);

        _logger.LogInformation("Dequeued verse for caching: {VerseId}", item.Verse.Id);

        return item;
    }
}
