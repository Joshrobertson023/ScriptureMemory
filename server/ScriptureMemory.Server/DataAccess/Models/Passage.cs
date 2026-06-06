using DataAccess.Models;

namespace ScriptureMemory.Server.DataAccess.Models;

public class Passage
{
    public Reference Reference { get; set; } = new();
    
    [InverseProperty(nameof(Verse.PassageNavigation))]
    public List<Verse> Verses { get; set; } = new();
}
