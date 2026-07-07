namespace ScriptureMemory.Server.Data.Models.Logs;

public class SyncLog
{
    public int Id { get; set; }
    
    public string BibleId { get; set; }
    
    public BibleSyncAction Action { get; set; }
    
    public SyncDestination Destination { get; set; }
    
    [Column(TypeName = "jsonb")]
    public object? JsonContext { get; set; }
    
    public DateTime Timestamp { get; set; }
}