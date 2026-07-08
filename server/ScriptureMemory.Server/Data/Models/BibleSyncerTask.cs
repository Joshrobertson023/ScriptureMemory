namespace ScriptureMemory.Server.Data.Models;

[NotMapped]
public class BibleSyncerTask
{
    public Func<CancellationToken, Task> WorkItem { get; set; } = null!;
    public string Initiator { get; set; } = string.Empty; // Who initiated/queued the work item for execution
    public string BibleId { get; set; } = string.Empty;
    public string MethodName => WorkItem.Method.Name; // Task method name

    public async Task InvokeAsync(CancellationToken cancellationToken)
    {
        await WorkItem.Invoke(cancellationToken);
    }
}