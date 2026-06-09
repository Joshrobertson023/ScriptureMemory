using static Azure.Core.HttpHeader;

namespace ScriptureMemory.Server.Tools;

public static class Books
{
    /// <summary>
    /// Assigns the display name for each book of the Bible
    /// </summary>
    public static class BookNames
    {
        public const string Genesis = "Genesis";
        public const string Exodus = "Exodus";
        public const string Leviticus = "Leviticus";
        public const string Numbers = "Numbers";
        public const string Deuteronomy = "Deuteronomy";
        public const string Joshua = "Joshua";
        public const string Judges = "Judges";
        public const string Ruth = "Ruth";
        public const string FirstSamuel = "1 Samuel";
        public const string SecondSamuel = "2 Samuel";
        public const string FirstKings = "1 Kings";
        public const string SecondKings = "2 Kings";
        public const string FirstChronicles = "1 Chronicles";
        public const string SecondChronicles = "2 Chronicles";
        public const string Ezra = "Ezra";
        public const string Nehemiah = "Nehemiah";
        public const string Esther = "Esther";
        public const string Job = "Job";
        public const string Psalms = "Psalms";
        public const string Proverbs = "Proverbs";
        public const string Ecclesiastes = "Ecclesiastes";
        public const string SongOfSolomon = "Song of Solomon";
        public const string Isaiah = "Isaiah";
        public const string Jeremiah = "Jeremiah";
        public const string Lamentations = "Lamentations";
        public const string Ezekiel = "Ezekiel";
        public const string Daniel = "Daniel";
        public const string Hosea = "Hosea";
        public const string Joel = "Joel";
        public const string Amos = "Amos";
        public const string Obadiah = "Obadiah";
        public const string Jonah = "Jonah";
        public const string Micah = "Micah";
        public const string Nahum = "Nahum";
        public const string Habakkuk = "Habakkuk";
        public const string Zephaniah = "Zephaniah";
        public const string Haggai = "Haggai";
        public const string Zechariah = "Zechariah";
        public const string Malachi = "Malachi";
        public const string Matthew = "Matthew";
        public const string Mark = "Mark";
        public const string Luke = "Luke";
        public const string John = "John";
        public const string Acts = "Acts";
        public const string Romans = "Romans";
        public const string FirstCorinthians = "1 Corinthians";
        public const string SecondCorinthians = "2 Corinthians";
        public const string Galatians = "Galatians";
        public const string Ephesians = "Ephesians";
        public const string Philippians = "Philippians";
        public const string Colossians = "Colossians";
        public const string FirstThessalonians = "1 Thessalonians";
        public const string SecondThessalonians = "2 Thessalonians";
        public const string FirstTimothy = "1 Timothy";
        public const string SecondTimothy = "2 Timothy";
        public const string Titus = "Titus";
        public const string Philemon = "Philemon";
        public const string Hebrews = "Hebrews";
        public const string James = "James";
        public const string FirstPeter = "1 Peter";
        public const string SecondPeter = "2 Peter";
        public const string FirstJohn = "1 John";
        public const string SecondJohn = "2 John";
        public const string ThirdJohn = "3 John";
        public const string Jude = "Jude";
        public const string Revelation = "Revelation";

        public static readonly IReadOnlyList<string> All = new[]
        {
            Genesis, Exodus, Leviticus, Numbers, Deuteronomy,
            Joshua, Judges, Ruth, FirstSamuel, SecondSamuel,
            FirstKings, SecondKings, FirstChronicles, SecondChronicles,
            Ezra, Nehemiah, Esther, Job, Psalms, Proverbs,
            Ecclesiastes, SongOfSolomon, Isaiah, Jeremiah, Lamentations,
            Ezekiel, Daniel, Hosea, Joel, Amos, Obadiah, Jonah,
            Micah, Nahum, Habakkuk, Zephaniah, Haggai, Zechariah, Malachi,
            Matthew, Mark, Luke, John, Acts, Romans,
            FirstCorinthians, SecondCorinthians, Galatians, Ephesians,
            Philippians, Colossians, FirstThessalonians, SecondThessalonians,
            FirstTimothy, SecondTimothy, Titus, Philemon, Hebrews,
            James, FirstPeter, SecondPeter, FirstJohn, SecondJohn,
            ThirdJohn, Jude, Revelation
        };
    }

    /// <summary>
    /// 
    /// </summary>
    private sealed class Book
    {
        public string DisplayName { get; init; }
        public string Abbreviation { get; init; }
        public List<string> FuzzyMatches { get; init; }

        public Book(string bookName, string abbreviation, List<string> fuzzyMatches)
        {
            DisplayName = bookName;
            Abbreviation = abbreviation;
            FuzzyMatches = fuzzyMatches;
        }
    }

    /// <summary>
    /// Assigns each book's display name with abbreviation and fuzzy matches
    /// </summary>
    private static readonly List<Book> BooksOfBible = new()
    {
        new Book(BookNames.Genesis, "gen", ["genesis", "gen", "geneses", "genisis"]),
        new Book(BookNames.Exodus, "exo", ["exodus", "exodous", "exodis"]),
        new Book(BookNames.Leviticus, "lev", ["leviticus", "liviticus", "levitikus", "leviticis", "levitikis"]),
        new Book(BookNames.Numbers, "num", ["numbers", "number"]),
        new Book(BookNames.Deuteronomy, "deu", ["deuteronomy", "duteronomy", "dutironomy"]),
        new Book(BookNames.Joshua, "jos", ["joshua"]),
        new Book(BookNames.Judges, "jdg", ["judges", "judge", "judeges"]),
        new Book(BookNames.Ruth, "rut", ["rut", "rut", "ruths"]),
        new Book(BookNames.FirstSamuel, "1sa", ["1 samuel", "1 samul", "1 samuels", "i samuel", "1samuel", "isamuel", "1sam"]),
        new Book(BookNames.SecondSamuel, "2sa", ["2 samuel", "2 samul", "2 samuels", "ii samuel", "2samuel", "iisamuel", "2sam"]),
        new Book(BookNames.FirstKings, "1ki", ["1 kings", "1 king", "i kings", "1kings", "ikings", "1kgs"]),
        new Book(BookNames.SecondKings, "2ki", ["2 kings", "2 king", "ii kings", "iikings", "2kings", "2kgs"]),
        new Book(BookNames.FirstChronicles, "1ch", ["1 chronicles", "1 chronicle", "1 cronicles", "i chronicles", "i cronicles", "1chronicles", "1chr"]),
        new Book(BookNames.SecondChronicles, "2ch", ["2 chronicles", "2 chronicle", "2 cronicles", "ii chronicles", "ii cronicles", "2chronicles", "2chr"]),
        new Book(BookNames.Ezra, "ezr", ["ezra"]),
        new Book(BookNames.Nehemiah, "neh", ["nehemiah", "neimiah", "nehimiah"]),
        new Book(BookNames.Esther, "est", ["esther", "ester"]),
        new Book(BookNames.Job, "job", ["job"]),
        new Book(BookNames.Psalms, "psa", ["psalms", "psalm", "salm", "salms"]),
        new Book(BookNames.Proverbs, "pro", ["proverbs", "proverb"]),
        new Book(BookNames.Ecclesiastes, "ecc", ["ecclesiastes", "eclesiastes"]),
        new Book(BookNames.SongOfSolomon, "sng", ["song of solomon", "song of songs", "songs of solomon", "song of soloman"]),
        new Book(BookNames.Isaiah, "isa", ["isaiah", "isaiahs", "isaia", "isaih"]),
        new Book(BookNames.Jeremiah, "jer", ["jeremiah", "jeremias", "jerimiah"]),
        new Book(BookNames.Lamentations, "lam", ["lamentations", "lamentation", "lamintation", "lamintations"]),
        new Book(BookNames.Ezekiel, "ezk", ["ezekiel", "ezikiel", "ezekial"]),
        new Book(BookNames.Daniel, "dan", ["daniel"]),
        new Book(BookNames.Hosea, "hos", ["hosea", "hosiah", "hoseah"]),
        new Book(BookNames.Joel, "jol", ["jol"]),
        new Book(BookNames.Amos, "amo", ["amo", "amis"]),
        new Book(BookNames.Obadiah, "oba", ["obadiah", "obadia", "obadias"]),
        new Book(BookNames.Jonah, "jon", ["jon", "jonas", "jona"]),
        new Book(BookNames.Micah, "mic", ["micah", "micha", "mica"]),
        new Book(BookNames.Nahum, "nam", ["nahum", "nahums", "nahu"]),
        new Book(BookNames.Habakkuk, "hab", ["habakkuk", "habakuk", "habakik", "habakkik"]),
        new Book(BookNames.Zephaniah, "zep", ["zephaniah", "zephaniahs", "zephania", "zephiniah"]),
        new Book(BookNames.Haggai, "hag", ["haggai", "hagai", "haggiai", "hagiai", "hagaia"]),
        new Book(BookNames.Zechariah, "zec", ["zechariah", "zecharaiah", "zecharaiahs", "zachariah"]),
        new Book(BookNames.Malachi, "mal", ["malachi", "malachai", "malichai", "malichi"]),
        new Book(BookNames.Matthew, "mat", ["matthew", "matthews", "mathew"]),
        new Book(BookNames.Mark, "mrk", ["mrk", "marks", "marc"]),
        new Book(BookNames.Luke, "luk", ["luk", "lukes"]),
        new Book(BookNames.John, "jhn", ["john", "johns", "jon"]),
        new Book(BookNames.Acts, "act", ["acts", "act"]),
        new Book(BookNames.Romans, "rom", ["romans", "roman"]),
        new Book(BookNames.FirstCorinthians, "1co", ["1 corinthians", "1 corinthian", "1corinthians", "1 chorinthians", "i corinthians", "1cor"]),
        new Book(BookNames.SecondCorinthians, "2co", ["2 corinthians", "2 corinthian", "2corinthians", "2 chorinthians", "ii corinthians", "2cor"]),
        new Book(BookNames.Galatians, "gal", ["galatians", "galatian", "galat", "galations"]),
        new Book(BookNames.Ephesians, "eph", ["ephesians", "ephesian"]),
        new Book(BookNames.Philippians, "php", ["philippians", "philippian", "phillippians", "phillipians"]),
        new Book(BookNames.Colossians, "col", ["colossians", "colossian", "collossians", "collosians"]),
        new Book(BookNames.FirstThessalonians, "1th", ["1 thessalonians", "1 thessalonian", "1thessalonians", "i thessalonians", "1 thessallonian", "1thess"]),
        new Book(BookNames.SecondThessalonians, "2th", ["2 thessalonians", "2 thessalonian", "2thessalonians", "ii thessalonians", "2 thessalonain", "2thess"]),
        new Book(BookNames.FirstTimothy, "1ti", ["1 timothy", "1 timothys", "1timothy", "1tim"]),
        new Book(BookNames.SecondTimothy, "2ti", ["2 timothy", "2 timothys", "2timothy", "2tim"]),
        new Book(BookNames.Titus, "tit", ["titus"]),
        new Book(BookNames.Philemon, "phm", ["philemon", "philemons", "phillemon", "philimon", "philemin"]),
        new Book(BookNames.Hebrews, "heb", ["hebrews", "hebrew"]),
        new Book(BookNames.James, "jas", ["james"]),
        new Book(BookNames.FirstPeter, "1pe", ["1 peter", "1peter", "1pet"]),
        new Book(BookNames.SecondPeter, "2pe", ["2 peter", "2peter", "2pet"]),
        new Book(BookNames.FirstJohn, "1jn", ["1 john", "1john", "1jn"]),
        new Book(BookNames.SecondJohn, "2jn", ["2 john", "2john", "2jn"]),
        new Book(BookNames.ThirdJohn, "3jn", ["3 john", "3john", "3jn"]),
        new Book(BookNames.Jude, "jud", ["jude"]),
        new Book(BookNames.Revelation, "rev", ["revelation", "revelations", "revelation of john",
                                          "revalation", "revalations", "revilations", "revilation"]),
    };

    /// <summary>
    /// Maps each abbreviation, fuzzy match, and display name to its book
    /// </summary>
    private static readonly Dictionary<string, Book> bookSearchIndex = BuildLookup(); 
        // Key -- abbreviation, fuzzy match, or display name
        // Value -- associated book

    private static Dictionary<string, Book> BuildLookup()
    {
        var returnIndex = new Dictionary<string, Book>(StringComparer.OrdinalIgnoreCase);

        foreach (var book in BooksOfBible)
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
        return BooksOfBible.FirstOrDefault(book => book.DisplayName == input)?.Abbreviation;
    }
    
    /// <summary>
    /// Tries to get the display name for a book by abbreviation, fuzzy match, or the full book's name.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="displayName"></param>
    /// <returns>
    /// Bool specifying if a valid book was found, and the display name for that book as the out param.
    /// Both return null if no valid book was found.
    /// </returns>
    public static bool TryGetBook(string input, out string? displayName)
    {
        bool isValidBook = bookSearchIndex.TryGetValue(input.Trim(), out var book);
        displayName = isValidBook ? book?.DisplayName : null;
        return isValidBook;
    }
}
