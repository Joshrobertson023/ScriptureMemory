using DataAccess.Models;

namespace ScriptureMemory.Server.DataAccess.Models;

public class Passage
{
    public string Id
    {
        get
        {
            if (Reference == null)
            {
                return "";
            }

            return Reference.VerseId ?? "";
        }
        set;
    }

    public Reference Reference { get; set; }
    
    public List<Verse> Verses { get; set; } = new();

    public string? CacheKey => Reference?.CacheKey ?? "";

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
