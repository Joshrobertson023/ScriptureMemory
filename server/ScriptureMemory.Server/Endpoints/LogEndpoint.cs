using ScriptureMemory.Server.SignalR;

namespace ScriptureMemory.Server.Endpoints;

public static class LogEndpoint
{
    public static void ConfigureLogEndpoints(this WebApplication app)
    {
        app.MapHub<LogHub>("/logs/stream");
    }
}