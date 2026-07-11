namespace ScriptureMemory.Server.Data.Models;

[NotMapped]
public class BibleSyncData
{
    public Bible Bible { get; set; } = new();
    public SyncProgressReport? LastSyncReport { get; set; }
}