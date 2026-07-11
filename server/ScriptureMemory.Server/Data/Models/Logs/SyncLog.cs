namespace ScriptureMemory.Server.Data.Models.Logs;

public class SyncLog
{
    public int Id { get; set; }
    
    public string? BibleId { get; set; }
    
    public int? UserId { get; set; }
    
    public string? SyncingId { get; set; } // Current bible id being synced
    
    public string? Username { get; set; }
    
    public BibleSyncAction Action { get; set; }

    public bool SystemInitiated { get; set; } = false;
    
    public string? Progress { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public ExceptionModel? Exception { get; set; }
}