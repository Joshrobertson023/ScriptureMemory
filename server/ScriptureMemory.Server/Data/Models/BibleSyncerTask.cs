using ScriptureMemory.Server.Data.Models;

namespace ScriptureMemory.Server.Data.Models;

public class BibleSyncerTask
{
    public string Initiator { get; set; } = string.Empty; // User that initiated/queued the work item for execution
    public string BibleId { get; set; } = string.Empty; 
        // Id of Bible that's being synced
        // Acts as unique identifier for queue tasks
    public string BibleName { get; set; } = string.Empty;
    public CancellationTokenSource Cts { get; set; } = new(); 
        // Checks if the sync task has been canceled before starting the sync
        // Allows the user to cancel the sync if it's in the queue
}