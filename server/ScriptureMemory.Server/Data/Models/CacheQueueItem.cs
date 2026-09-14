namespace ScriptureMemory.Server.Data.Models;

public class CacheQueueItem
{
    public List<Verse> Verses { get; set; } = new();
    public string Translation { get; set; } = string.Empty;
    public MemoryCacheType CacheType { get; set; } = MemoryCacheType.PlainText;
}
