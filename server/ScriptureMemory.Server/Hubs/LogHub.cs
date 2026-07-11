using Microsoft.AspNetCore.SignalR;
using ScriptureMemory.Server.Data.Models;
using ScriptureMemory.Server.Data.Models.Logs;

namespace ScriptureMemory.Server.SignalR;

public class LogHub : Hub
{
    public async Task SendLog(SignalRLog log)
    {
        await Clients.All.SendAsync("ReceiveLog", log);
    }
}