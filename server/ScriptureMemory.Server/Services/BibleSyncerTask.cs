using ScriptureMemory.Server.Data.Models;

namespace ScriptureMemory.Server.Services;

public class BibleSyncerTask
{
    public string Initiator { get; set; } = string.Empty; // Who initiated/queued the work item for execution
    public string BibleId { get; set; } = string.Empty;
    public string BibleName { get; set; } = string.Empty;
    public CancellationTokenSource Cts { get; set; } = new();
}