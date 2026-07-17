namespace ScriptureMemory.Server.Data.Models;

[NotMapped]
public class BibleSyncData
{
    public Bible Bible { get; set; } = new();
    public SyncEvent? LastSyncReport { get; set; }
    public bool SyncInProgress { get; set; } = false;
}