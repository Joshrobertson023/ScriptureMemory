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
    private Reference _reference;

    public Reference Reference
    {
        get => _reference;
        set
        {
            _reference = value;

            if (string.IsNullOrEmpty(_reference.Book.Abbreviation))
            {
                _reference.Book = Books.GetBook(_reference.Book.DisplayName);
            }

            this.Id = _reference.VerseId;
        }
    }

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
