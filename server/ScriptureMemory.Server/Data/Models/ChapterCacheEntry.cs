namespace ScriptureMemory.Server.Data.Models;

public class ChapterCacheEntry
{
    public Reference Reference { get; set; }
    public Dictionary<string, Verse> Verses { get; set; }
}