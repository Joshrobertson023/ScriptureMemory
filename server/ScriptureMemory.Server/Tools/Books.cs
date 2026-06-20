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
        new Book("Genesis", "gen", ["genesis", "gen", "geneses", "genisis"]),
        new Book("Exodus", "exo", ["exodus", "exodous", "exodis"]),
        new Book("Leviticus", "lev", ["leviticus", "liviticus", "levitikus", "leviticis", "levitikis"]),
        new Book("Numbers", "num", ["numbers", "number"]),
        new Book("Deuteronomy", "deu", ["deuteronomy", "duteronomy", "dutironomy"]),
        new Book("Joshua", "jos", ["joshua"]),
        new Book("Judges", "jdg", ["judges", "judge", "judeges"]),
        new Book("Ruth", "rut", ["rut", "rut", "ruths"]),
        new Book("1 Samuel", "1sa", ["1 samuel", "1 samul", "1 samuels", "i samuel", "1samuel", "isamuel", "1sam"]),
        new Book("2 Samuel", "2sa", ["2 samuel", "2 samul", "2 samuels", "ii samuel", "2samuel", "iisamuel", "2sam"]),
        new Book("1 Kings", "1ki", ["1 kings", "1 king", "i kings", "1kings", "ikings", "1kgs"]),
        new Book("2 Kings", "2ki", ["2 kings", "2 king", "ii kings", "iikings", "2kings", "2kgs"]),
        new Book("1 Chronicles", "1ch", ["1 chronicles", "1 chronicle", "1 cronicles", "i chronicles", "i cronicles", "1chronicles", "1chr"]),
        new Book("2 Chronicles", "2ch", ["2 chronicles", "2 chronicle", "2 cronicles", "ii chronicles", "ii cronicles", "2chronicles", "2chr"]),
        new Book("Ezra", "ezr", ["ezra"]),
        new Book("Nehemiah", "neh", ["nehemiah", "neimiah", "nehimiah"]),
        new Book("Esther", "est", ["esther", "ester"]),
        new Book("Job", "job", ["job"]),
        new Book("Psalms", "psa", ["psalms", "psalm", "salm", "salms"]),
        new Book("Proverbs", "pro", ["proverbs", "proverb"]),
        new Book("Ecclesiastes", "ecc", ["ecclesiastes", "eclesiastes"]),
        new Book("Song of Solomon", "sng", ["song of solomon", "song of songs", "songs of solomon", "song of soloman"]),
        new Book("Isaiah", "isa", ["isaiah", "isaiahs", "isaia", "isaih"]),
        new Book("Jeremiah", "jer", ["jeremiah", "jeremias", "jerimiah"]),
        new Book("Lamentations", "lam", ["lamentations", "lamentation", "lamintation", "lamintations"]),
        new Book("Ezekiel", "ezk", ["ezekiel", "ezikiel", "ezekial"]),
        new Book("Daniel", "dan", ["daniel"]),
        new Book("Hosea", "hos", ["hosea", "hosiah", "hoseah"]),
        new Book("Joel", "jol", ["jol"]),
        new Book("Amos", "amo", ["amo", "amis"]),
        new Book("Obadiah", "oba", ["obadiah", "obadia", "obadias"]),
        new Book("Jonah", "jon", ["jon", "jonas", "jona"]),
        new Book("Micah", "mic", ["micah", "micha", "mica"]),
        new Book("Nahum", "nam", ["nahum", "nahums", "nahu"]),
        new Book("Habakkuk", "hab", ["habakkuk", "habakuk", "habakik", "habakkik"]),
        new Book("Zephaniah", "zep", ["zephaniah", "zephaniahs", "zephania", "zephiniah"]),
        new Book("Haggai", "hag", ["haggai", "hagai", "haggiai", "hagiai", "hagaia"]),
        new Book("Zechariah", "zec", ["zechariah", "zecharaiah", "zecharaiahs", "zachariah"]),
        new Book("Malachi", "mal", ["malachi", "malachai", "malichai", "malichi"]),
        new Book("Matthew", "mat", ["matthew", "matthews", "mathew"]),
        new Book("Mark", "mrk", ["mrk", "marks", "marc"]),
        new Book("Luke", "luk", ["luk", "lukes"]),
        new Book("John", "jhn", ["john", "johns", "jon"]),
        new Book("Acts", "act", ["acts", "act"]),
        new Book("Romans", "rom", ["romans", "roman"]),
        new Book("1 Corinthians", "1co", ["1 corinthians", "1 corinthian", "1corinthians", "1 chorinthians", "i corinthians", "1cor"]),
        new Book("2 Corinthians", "2co", ["2 corinthians", "2 corinthian", "2corinthians", "2 chorinthians", "ii corinthians", "2cor"]),
        new Book("Galatians", "gal", ["galatians", "galatian", "galat", "galations"]),
        new Book("Ephesians", "eph", ["ephesians", "ephesian"]),
        new Book("Philippians", "php", ["philippians", "philippian", "phillippians", "phillipians"]),
        new Book("Colossians", "col", ["colossians", "colossian", "collossians", "collosians"]),
        new Book("1 Thessalonians", "1th", ["1 thessalonians", "1 thessalonian", "1thessalonians", "i thessalonians", "1 thessallonian", "1thess"]),
        new Book("2 Thessalonians", "2th", ["2 thessalonians", "2 thessalonian", "2thessalonians", "ii thessalonians", "2 thessalonain", "2thess"]),
        new Book("1 Timothy", "1ti", ["1 timothy", "1 timothys", "1timothy", "1tim"]),
        new Book("2 Timothy", "2ti", ["2 timothy", "2 timothys", "2timothy", "2tim"]),
        new Book("Titus", "tit", ["titus"]),
        new Book("Philemon", "phm", ["philemon", "philemons", "phillemon", "philimon", "philemin"]),
        new Book("Hebrews", "heb", ["hebrews", "hebrew"]),
        new Book("James", "jas", ["james"]),
        new Book("1 Peter", "1pe", ["1 peter", "1peter", "1pet"]),
        new Book("2 Peter", "2pe", ["2 peter", "2peter", "2pet"]),
        new Book("1 John", "1jn", ["1 john", "1john", "1jn"]),
        new Book("2 John", "2jn", ["2 john", "2john", "2jn"]),
        new Book("3 John", "3jn", ["3 john", "3john", "3jn"]),
        new Book("Jude", "jud", ["jude"]),
        new Book("Revelation", "rev", ["revelation", "revelations", "revelation of john",
                                          "revalation", "revalations", "revilations", "revilation"]),
    };

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
            returnIndex.Add(book.DisplayName, book);
            
            returnIndex.Add(book.Abbreviation, book);
            
            foreach (var fuzzyMatch in book.FuzzyMatches)
            {
                returnIndex.Add(fuzzyMatch, book);
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
    public static bool TryGetBook(string input, out Book? book)
    {
        bool isValidBook = bookDisplayNameAbbreviationMap.TryGetValue(input.Trim(), out var _book);
        book = isValidBook ? _book : null;
        return isValidBook;
    }
}
