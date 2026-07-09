using ScriptureMemory.Server.CustomExceptions;
using ScriptureMemory.Server.Data.Models;
using static Azure.Core.HttpHeader;

namespace ScriptureMemory.Server.Tools;

public static class Books
{
    /// <summary>
    /// Every book of the Bible's display name, abbreviation, and fuzzy matches
    /// </summary>
    public static readonly List<Book> AllBooks = new()
    {
        new Book("Genesis", 50, "gen", ["genesis", "gen", "geneses", "genisis"]),
        new Book("Exodus", 40, "exo", ["exodus", "exodous", "exodis"]),
        new Book("Leviticus", 27, "lev", ["leviticus", "liviticus", "levitikus", "leviticis", "levitikis"]),
        new Book("Numbers", 36, "num", ["numbers", "number"]),
        new Book("Deuteronomy", 34, "deu", ["deuteronomy", "duteronomy", "dutironomy"]),
        new Book("Joshua", 24, "jos", ["joshua"]),
        new Book("Judges", 21, "jdg", ["judges", "judge", "judeges"]),
        new Book("Ruth", 4, "rut", ["rut", "rut", "ruths"]),
        new Book("1 Samuel", 31, "1sa", ["1 samuel", "1 samul", "1 samuels", "i samuel", "1samuel", "isamuel", "1sam"]),
        new Book("2 Samuel", 24, "2sa", ["2 samuel", "2 samul", "2 samuels", "ii samuel", "2samuel", "iisamuel", "2sam"]),
        new Book("1 Kings", 22, "1ki", ["1 kings", "1 king", "i kings", "1kings", "ikings", "1kgs"]),
        new Book("2 Kings", 25, "2ki", ["2 kings", "2 king", "ii kings", "iikings", "2kings", "2kgs"]),
        new Book("1 Chronicles", 29, "1ch", ["1 chronicles", "1 chronicle", "1 cronicles", "i chronicles", "i cronicles", "1chronicles", "1chr"]),
        new Book("2 Chronicles", 36, "2ch", ["2 chronicles", "2 chronicle", "2 cronicles", "ii chronicles", "ii cronicles", "2chronicles", "2chr"]),
        new Book("Ezra", 10, "ezr", ["ezra"]),
        new Book("Nehemiah", 13, "neh", ["nehemiah", "neimiah", "nehimiah"]),
        new Book("Esther", 10, "est", ["esther", "ester"]),
        new Book("Job", 42, "job", ["job"]),
        new Book("Psalms", 150, "psa", ["psalms", "psalm", "salm", "salms"]),
        new Book("Proverbs", 31, "pro", ["proverbs", "proverb"]),
        new Book("Ecclesiastes", 12, "ecc", ["ecclesiastes", "eclesiastes"]),
        new Book("Song of Solomon", 8, "sng", ["song of solomon", "song of songs", "songs of solomon", "song of soloman"]),
        new Book("Isaiah", 66, "isa", ["isaiah", "isaiahs", "isaia", "isaih"]),
        new Book("Jeremiah", 52, "jer", ["jeremiah", "jeremias", "jerimiah"]),
        new Book("Lamentations", 5, "lam", ["lamentations", "lamentation", "lamintation", "lamintations"]),
        new Book("Ezekiel", 48, "ezk", ["ezekiel", "ezikiel", "ezekial"]),
        new Book("Daniel", 12, "dan", ["daniel"]),
        new Book("Hosea", 14, "hos", ["hosea", "hosiah", "hoseah"]),
        new Book("Joel", 3, "jol", ["jol"]),
        new Book("Amos", 9, "amo", ["amo", "amis"]),
        new Book("Obadiah", 1, "oba", ["obadiah", "obadia", "obadias"]),
        new Book("Jonah", 4, "jon", ["jon", "jonas", "jona"]),
        new Book("Micah", 7, "mic", ["micah", "micha", "mica"]),
        new Book("Nahum", 3, "nam", ["nahum", "nahums", "nahu"]),
        new Book("Habakkuk", 3, "hab", ["habakkuk", "habakuk", "habakik", "habakkik"]),
        new Book("Zephaniah", 3, "zep", ["zephaniah", "zephaniahs", "zephania", "zephiniah"]),
        new Book("Haggai", 2, "hag", ["haggai", "hagai", "haggiai", "hagiai", "hagaia"]),
        new Book("Zechariah", 14, "zec", ["zechariah", "zecharaiah", "zecharaiahs", "zachariah"]),
        new Book("Malachi", 4, "mal", ["malachi", "malachai", "malichai", "malichi"]),
        new Book("Matthew", 28, "mat", ["matthew", "matthews", "mathew"]),
        new Book("Mark", 16, "mrk", ["mrk", "marks", "marc"]),
        new Book("Luke", 24, "luk", ["luk", "lukes"]),
        new Book("John", 21, "jhn", ["john", "johns", "jon"]),
        new Book("Acts", 28, "act", ["acts", "act"]),
        new Book("Romans", 16, "rom", ["romans", "roman"]),
        new Book("1 Corinthians", 16, "1co", ["1 corinthians", "1 corinthian", "1corinthians", "1 chorinthians", "i corinthians", "1cor"]),
        new Book("2 Corinthians", 13, "2co", ["2 corinthians", "2 corinthian", "2corinthians", "2 chorinthians", "ii corinthians", "2cor"]),
        new Book("Galatians", 6, "gal", ["galatians", "galatian", "galat", "galations"]),
        new Book("Ephesians", 6, "eph", ["ephesians", "ephesian"]),
        new Book("Philippians", 4, "php", ["philippians", "philippian", "phillippians", "phillipians"]),
        new Book("Colossians", 4, "col", ["colossians", "colossian", "collossians", "collosians"]),
        new Book("1 Thessalonians", 5, "1th", ["1 thessalonians", "1 thessalonian", "1thessalonians", "i thessalonians", "1 thessallonian", "1thess"]),
        new Book("2 Thessalonians", 3, "2th", ["2 thessalonians", "2 thessalonian", "2thessalonians", "ii thessalonians", "2 thessalonain", "2thess"]),
        new Book("1 Timothy", 6, "1ti", ["1 timothy", "1 timothys", "1timothy", "1tim"]),
        new Book("2 Timothy", 4, "2ti", ["2 timothy", "2 timothys", "2timothy", "2tim"]),
        new Book("Titus", 3, "tit", ["titus"]),
        new Book("Philemon", 1, "phm", ["philemon", "philemons", "phillemon", "philimon", "philemin"]),
        new Book("Hebrews", 13, "heb", ["hebrews", "hebrew"]),
        new Book("James", 5, "jas", ["james"]),
        new Book("1 Peter", 5, "1pe", ["1 peter", "1peter", "1pet"]),
        new Book("2 Peter", 3, "2pe", ["2 peter", "2peter", "2pet"]),
        new Book("1 John", 5, "1jn", ["1 john", "1john", "1jn"]),
        new Book("2 John", 1, "2jn", ["2 john", "2john", "2jn"]),
        new Book("3 John", 1, "3jn", ["3 john", "3john", "3jn"]),
        new Book("Jude", 1, "jud", ["jude"]),
        new Book("Revelation", 22, "rev", ["revelation", "revelations", "revelation of john",
                                          "revalation", "revalations", "revilations", "revilation"]),
    };

    public static int TotalChapters => 1189;
    public static int TotalBooks => 66;

    /// <summary>
    /// Maps each abbreviation, fuzzy match, and display name to its book
    /// </summary>
    private static readonly Dictionary<string, Book> bookDisplayNameAbbreviationMap = BuildMap(); 
        // Key -- abbreviation, fuzzy match, or display name
        // Value -- associated book

    private static Dictionary<string, Book> BuildMap()
    {
        var returnIndex = new Dictionary<string, Book>(StringComparer.OrdinalIgnoreCase);

        foreach (var book in AllBooks)
        {
            returnIndex.TryAdd(book.DisplayName, book);
            
            returnIndex.TryAdd(book.Abbreviation, book);
            
            foreach (var fuzzyMatch in book.FuzzyMatches)
            {
                returnIndex.TryAdd(fuzzyMatch, book);
            }
        }

        return returnIndex;
    }

    /// <summary>
    /// Gets a book's abbreviation from the book name
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string? GetAbbreviation(string input)
    {
        return AllBooks.FirstOrDefault(book => book.DisplayName == input)?.Abbreviation;
    }
    
    /// <summary>
    /// Tries to get the display name for a book by abbreviation, fuzzy match, or the full book's name.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="book"></param>
    /// <returns>
    /// Bool specifying if a valid book was found, and the display name for that book as the out param.
    /// Both return null if no valid book was found.
    /// </returns>
    public static Book? TryGetBook(string input)
    {
        return bookDisplayNameAbbreviationMap.TryGetValue(input.Trim(), out var _book) is false
               ? null
               : _book;
    }

    public static Book GetBook(string input)
    {
        return bookDisplayNameAbbreviationMap.TryGetValue(input.Trim(), out var _book) is false
            ? throw new BookNotFoundException($"Book {input} not found.")
            : _book;
    }
}
