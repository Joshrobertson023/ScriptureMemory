using DataAccess.Models;

namespace ScriptureMemory.Server.DataAccess.Models;

public class CrossReferenceResponse
{
    public Verse FromVerse { get; set; } = new();
    public List<Passage> CrossReferences { get; set; } = new();
}
