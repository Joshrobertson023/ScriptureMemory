using DataAccess.Models;

namespace ScriptureMemory.Server.DataAccess.Models;

public class VerseCardResponse
{
    public int TotalSaved { get; set; }
    public int TotalMemorized { get; set; }
    public int NumPracticed { get; set; }
    public DateTime NextDue { get; set; }
    public List<Verse> CrossReferences { get; set; } = new();
    public List<Verse> Similar { get; set; } = new();
}
