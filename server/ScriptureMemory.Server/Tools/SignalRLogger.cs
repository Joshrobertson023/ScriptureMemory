using ScriptureMemory.Server.SignalR;

namespace ScriptureMemory.Server.Tools;

using Microsoft.AspNetCore.SignalR;

/// <summary>
/// Custom logger that takes logs and sends via SignalR
/// </summary>
public class SignalRLogger : ILogger
{
    private readonly string _name;
    private readonly IHubContext<LogHub> _hubContext;

    public SignalRLogger(string name, IHubContext<LogHub> hubContext)
    {
        _name = name;
        _hubContext = hubContext;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) =>
        !_name.StartsWith("Microsoft.AspNetCore.SignalR") &&
        !_name.StartsWith("Microsoft.AspNetCore.Http.Connections") &&
        !_name.StartsWith("Npgsql.Command");

    public void Log<TState>(
        LogLevel logLevel, 
        EventId eventId, 
        TState state, 
        Exception? exception, 
        Func<TState, Exception, string> formatter)
    {
        var message = formatter(state, exception!);

        if (!message.Contains("Executed DbCommand") 
            && !message.Contains("Command execution completed") 
            && !message.Contains("Executing command"))
            _ = _hubContext.Clients.All.SendAsync(
                "ReceiveLog",
                new { Timestamp = DateTime.UtcNow, Level = logLevel.ToString(), Message = message });
    }
}
