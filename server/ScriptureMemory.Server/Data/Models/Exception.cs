using ScriptureMemory.Server.Data.Models.Logs;

namespace ScriptureMemory.Server.Data.Models;

public class ExceptionModel
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Message { get; set; }
    public string? StackTrace { get; set; }
    public string? Source { get; set; }
    public DateTime Timestamp { get; set; }
    
    public int? SyncReportId { get; set; }
    public SyncEvent? SyncLogNavigation { get; set; }

    public ExceptionModel(Exception ex)
    {
        Type = ex.GetType().ToString();
        Message = ex.Message;
        StackTrace = ex.StackTrace;
        Source = ex.Source;
        Timestamp = DateTime.UtcNow;
    }
    
    public ExceptionModel() { }
}