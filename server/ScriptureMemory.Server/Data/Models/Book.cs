namespace ScriptureMemory.Server.Data.Models;

/// <summary>
/// Represents a book of the Bible's name, abbreviation, and fuzzy matches
/// </summary>
[NotMapped]
public sealed class Book
{
    public string DisplayName { get; init; } = string.Empty;
    public string Abbreviation { get; init; } = string.Empty;
    public List<string>? FuzzyMatches { get; init; }
    public int NumChapters { get; set; }

    /// <summary>
    /// Used when initializing all 66 books on app start
    /// </summary>
    public Book(
        string bookName,
        int numChapters,
        string abbreviation,
        List<string> fuzzyMatches)
    {
        DisplayName = bookName;
        Abbreviation = abbreviation;
        FuzzyMatches = fuzzyMatches;
        NumChapters = numChapters;
    }

    /// <summary>
    /// Creates an independent Book instance from a known book name/abbreviation.
    /// </summary>
    public Book(string requestedBook)
    {
        var result = Books.GetBook(requestedBook);

        DisplayName = result.DisplayName;
        Abbreviation = result.Abbreviation;
        FuzzyMatches = result.FuzzyMatches is null
            ? new List<string>()
            : new List<string>(result.FuzzyMatches);
        NumChapters = result.NumChapters;
    }

    public Book()
    {
    }
}