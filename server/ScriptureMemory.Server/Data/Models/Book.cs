namespace ScriptureMemory.Server.Data.Models;

/// <summary>
/// Represents a book of the Bible's name, abbreviation, and fuzzy matches
/// </summary>
[NotMapped]
public sealed class Book
{
    public string DisplayName { get; init; }
    public string Abbreviation { get; init; }
    public List<string> FuzzyMatches { get; init; }

    /// <summary>
    /// Used when initializing all 66 books on app start
    /// </summary>
    /// <param name="bookName"></param>
    /// <param name="abbreviation"></param>
    /// <param name="fuzzyMatches"></param>
    public Book(string bookName, string abbreviation, List<string> fuzzyMatches)
    {
        DisplayName = bookName;
        Abbreviation = abbreviation;
        FuzzyMatches = fuzzyMatches;
    }
}