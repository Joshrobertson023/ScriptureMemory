using DataAccess.Models;

namespace ScriptureMemory.Server.DataAccess.Models;

public class VerseCardResponse
{
    public Verse Verse { get; set; } = new();
    public int NumPracticed { get; set; }
    public DateTime NextDue { get; set; }
    public List<Note> Notes { get; set; } = new();
    public List<Verse> CrossReferences { get; set; } = new();
    public List<Collection> Collections { get; set; } = new();
    public List<Verse> Similar { get; set; } = new();
}
