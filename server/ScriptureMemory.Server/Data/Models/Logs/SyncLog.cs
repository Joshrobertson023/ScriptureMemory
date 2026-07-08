namespace ScriptureMemory.Server.Data.Models.Logs;

public class SyncLog
{
    public int Id { get; set; }
    
    public string BibleId { get; set; }
    
    public int UserId { get; set; }
    
    public BibleSyncAction Action { get; set; }
    
    public bool UserDirected { get; set; } // Was a user-directed action and not an unwanted stop from an error
    
    public SyncDestination Destination { get; set; }
    
    public DateTime Timestamp { get; set; }
}