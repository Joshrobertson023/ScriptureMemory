namespace ScriptureMemory.Server.Data.Models.Logs;

/// <summary>
/// A simplified version of logs to send to Admin dashboard
/// </summary>
public class SignalRLog
{
    public DateTime Timestamp { get; set; }
    public string Level { get; set; } = "Information";
    public string Message { get; set; } = string.Empty;
}