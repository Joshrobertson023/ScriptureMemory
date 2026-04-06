using DataAccess.Models;

namespace ScriptureMemory.Server.DataAccess.Models;

public class Passage
{
    public Reference Reference { get; set; } = new();
    public List<Verse> Verses { get; set; } = new();
}
