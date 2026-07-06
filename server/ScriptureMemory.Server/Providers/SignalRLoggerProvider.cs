using Microsoft.AspNetCore.SignalR;
using ScriptureMemory.Server.SignalR;
using System.Collections.Concurrent;

namespace ScriptureMemory.Server.Providers;

using Microsoft.Extensions.Logging;

public class SignalRLoggerProvider : ILoggerProvider
{
    private readonly IHubContext<LogHub> _hubContext;
    private readonly ConcurrentDictionary<string, SignalRLogger> _loggers = new();

    public SignalRLoggerProvider(IHubContext<LogHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, name => new SignalRLogger(name, _hubContext));
    }

    public void Dispose() { }
}
