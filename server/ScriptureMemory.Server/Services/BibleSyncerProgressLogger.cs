using Microsoft.AspNetCore.SignalR;
using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.Data.Models;
using ScriptureMemory.Server.Migrations;
using ScriptureMemory.Server.SignalR;

namespace ScriptureMemory.Server.Services;

public class BibleSyncerProgressLogger
{
    private readonly IHubContext<SyncHub> _hubContext;
    private readonly BibleSyncLogData _logContext;

    public BibleSyncerProgressLogger(
        IHubContext<SyncHub> hubContext,
        BibleSyncLogData logData)
    {
        _hubContext = hubContext;
        _logContext = logData;
    }

    public async Task Update(SyncProgressReport progress)
    {
        _ = _hubContext.Clients.All.SendAsync("ReceiveProgress", progress);

        if (progress.Action == BibleSyncAction.Progress)
            return;

        await _logContext.Log(progress);
    }
}