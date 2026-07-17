namespace ScriptureMemory.Server.Data.Models;

public class GetBibleSyncDataResponse
{
    public bool CurrentlySyncing { get; set; } = false;
    public DateTime? LastSync { get; set; }
    public List<BibleSyncData> SyncData { get; set; } = new();
}