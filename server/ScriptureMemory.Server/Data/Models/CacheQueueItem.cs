namespace ScriptureMemory.Server.Data.Models;

public class CacheQueueItem
{
    public Verse Verse { get; set; } = new();
    public MemoryCacheType CacheType { get; set; }
}
