namespace ScriptureMemory.Server.Data.Models;

[NotMapped]
public class BibleSyncerTask
{
    public Func<CancellationToken, Task> WorkItem { get; set; } = null!;
    public string Initializer { get; set; } = string.Empty; // Who initialized/queued the work item for execution
    public string MethodName => WorkItem.Method.Name;             // Task method name

    public async Task InvokeAsync(CancellationToken cancellationToken)
    {
        await WorkItem.Invoke(cancellationToken);
    }
}