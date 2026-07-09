namespace ScriptureMemory.Server.Data.Models;

public class SyncTaskProgressReport
{
    public string BibleId { get; set; }
    public int Percentage { get; set; }
    public string Message { get; set; } = string.Empty;
}