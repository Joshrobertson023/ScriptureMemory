namespace ScriptureMemory.Server.Data.Models;

public class SyncProgressReport
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string BibleId { get; set; }
    public string? BibleName { get; set; }
    public int Percentage { get; set; }
    public BibleSyncAction? Action { get; set; }
    public string? Message { get; set; }
    public bool SystemInitiated { get; set; } = false;
    public ExceptionModel? Exception { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}