using DataAccess.Models;

namespace ScriptureMemory.Server.DataAccess.Models;

public class Passage
{
    public Reference Reference { get; set; }
    
    public List<Verse> Verses { get; set; } = new();

    public Passage(Reference reference)
    {
        Reference = reference;
    }

    public Passage(string readableReference)
    {
        Reference = new Reference(readableReference);
    }
    
    public Passage() { }
}
