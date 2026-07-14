using Microsoft.AspNetCore.SignalR;
using ScriptureMemory.Server.Data.Models;
using ScriptureMemory.Server.Data.Models.Logs;

namespace ScriptureMemory.Server.SignalR;

public class SyncHub : Hub
{
    public async Task SendLog(SyncEvent log)
    {
        await Clients.All.SendAsync("ReceiveSyncLog", log);
    }

    public async Task SendProgress(SyncEvent progress)
    {
        await Clients.All.SendAsync("ReceiveProgress", progress);
    }

    public async Task SendAuthorizationSyncCompleted()
    {
        await Clients.All.SendAsync("AuthorizationSyncCompleted");
    }
}