using DataAccess.Models;

namespace ScriptureMemory.Server.DataAccess.Models;

public class Passage
{
    public string Id { get; set; } = string.Empty;

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

            if (string.IsNullOrEmpty(Id))
            {
                Id = _reference.VerseId ?? string.Empty;
            }
        }
    }

    public List<Verse> Verses { get; set; } = new();

    public string? CacheKey => Reference?.CacheKey ?? string.Empty;

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