using Microsoft.AspNetCore.SignalR;
using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.Data.Models;
using ScriptureMemory.Server.SignalR;

namespace ScriptureMemory.Server.Services;

public class BibleSyncerEventDispatcher
{
    private readonly IHubContext<SyncHub> _hubContext;
    private readonly BibleSyncLogData _logContext;
    private readonly ILogger<BibleSyncerEventDispatcher> _logger;

    public BibleSyncerEventDispatcher(
        IHubContext<SyncHub> hubContext,
        BibleSyncLogData logData,
        ILogger<BibleSyncerEventDispatcher> logger)
    {
        _hubContext = hubContext;
        _logContext = logData;
        _logger = logger;
    }

    public async Task Send(SyncEvent progress)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveProgress", progress);

        if (progress.Event == BibleSyncEvent.Progress)
            return;

        _logContext.Log(progress);
    }
}