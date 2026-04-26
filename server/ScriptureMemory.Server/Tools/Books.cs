using static Azure.Core.HttpHeader;

namespace ScriptureMemory.Server.Tools;

public class Books
{
    // Get official book display name from user input

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

        public static bool IsValid(string book) => All.Contains(book);
    }

    private sealed record Book(string[] Abbreviations, string[] FuzzyMatches);

    private static readonly Dictionary<string, Book> BookNamesOfBible = new(StringComparer.OrdinalIgnoreCase)
    {
        [BookNames.Genesis] = new(["gen"], ["genesis", "gen", "geneses", "genisis"]),
        [BookNames.Exodus] = new(["exo"], ["exodus", "exodous", "exodis"]),
        [BookNames.Leviticus] = new(["lev"], ["leviticus", "liviticus", "levitikus", "leviticis", "levitikis"]),
        [BookNames.Numbers] = new(["num"], ["numbers", "number"]),
        [BookNames.Deuteronomy] = new(["deu"], ["deuteronomy", "duteronomy", "dutironomy"]),
        [BookNames.Joshua] = new(["jos"], ["joshua"]),
        [BookNames.Judges] = new(["jdg"], ["judges", "judge", "judeges"]),
        [BookNames.Ruth] = new([], ["rut", "rut", "ruths"]),
        [BookNames.FirstSamuel] = new(["1sa"], ["1 samuel", "1 samul", "1 samuels", "i samuel", "1samuel", "isamuel", "1sam"]),
        [BookNames.SecondSamuel] = new(["2sa"], ["2 samuel", "2 samul", "2 samuels", "ii samuel", "2samuel", "iisamuel", "2sam"]),
        [BookNames.FirstKings] = new(["1ki"], ["1 kings", "1 king", "i kings", "1kings", "ikings", "1kgs"]),
        [BookNames.SecondKings] = new(["2ki"], ["2 kings", "2 king", "ii kings", "iikings", "2kings", "2kgs"]),
        [BookNames.FirstChronicles] = new(["1ch"], ["1 chronicles", "1 chronicle", "1 cronicles", "i chronicles", "i cronicles", "1chronicles", "1chr"]),
        [BookNames.SecondChronicles] = new(["2ch"], ["2 chronicles", "2 chronicle", "2 cronicles", "ii chronicles", "ii cronicles", "2chronicles", "2chr"]),
        [BookNames.Ezra] = new(["ezr"], ["ezra"]),
        [BookNames.Nehemiah] = new(["neh"], ["nehemiah", "neimiah", "nehimiah"]),
        [BookNames.Esther] = new(["est"], ["esther", "ester"]),
        [BookNames.Job] = new(["job"], ["job"]),
        [BookNames.Psalms] = new(["psa"], ["psalms", "psalm", "salm", "salms"]),
        [BookNames.Proverbs] = new(["pro"], ["proverbs", "proverb"]),
        [BookNames.Ecclesiastes] = new(["ecc"], ["ecclesiastes", "eclesiastes"]),
        [BookNames.SongOfSolomon] = new(["sng"], ["song of solomon", "song of songs", "songs of solomon", "song of soloman"]),
        [BookNames.Isaiah] = new(["isa"], ["isaiah", "isaiahs", "isaia", "isaih"]),
        [BookNames.Jeremiah] = new(["jer"], ["jeremiah", "jeremias", "jerimiah"]),
        [BookNames.Lamentations] = new(["lam"], ["lamentations", "lamentation", "lamintation", "lamintations"]),
        [BookNames.Ezekiel] = new(["ezk"], ["ezekiel", "ezikiel", "ezekial"]),
        [BookNames.Daniel] = new(["dan"], ["daniel"]),
        [BookNames.Hosea] = new(["hos"], ["hosea", "hosiah", "hoseah"]),
        [BookNames.Joel] = new(["jol"], ["jol"]),
        [BookNames.Amos] = new(["amo"], ["amo", "amis"]),
        [BookNames.Obadiah] = new(["oba"], ["obadiah", "obadia", "obadias"]),
        [BookNames.Jonah] = new(["jon"], ["jon", "jonas", "jona"]),
        [BookNames.Micah] = new(["mic"], ["micah", "micha", "mica"]),
        [BookNames.Nahum] = new(["nam"], ["nahum", "nahums", "nahu"]),
        [BookNames.Habakkuk] = new(["hab"], ["habakkuk", "habakuk", "habakik", "habakkik"]),
        [BookNames.Zephaniah] = new(["zep"], ["zephaniah", "zephaniahs", "zephania", "zephiniah"]),
        [BookNames.Haggai] = new(["hag"], ["haggai", "hagai", "haggiai", "hagiai", "hagaia"]),
        [BookNames.Zechariah] = new(["zec"], ["zechariah", "zecharaiah", "zecharaiahs", "zachariah"]),
        [BookNames.Malachi] = new(["mal"], ["malachi", "malachai", "malichai", "malichi"]),
        [BookNames.Matthew] = new(["mat"], ["matthew", "matthews", "mathew"]),
        [BookNames.Mark] = new(["mrk"], ["mrk", "marks", "marc"]),
        [BookNames.Luke] = new(["luk"], ["luk", "lukes"]),
        [BookNames.John] = new(["jhn"], ["john", "johns", "jon"]),
        [BookNames.Acts] = new(["act"], ["acts", "act"]),
        [BookNames.Romans] = new(["rom"], ["romans", "roman"]),
        [BookNames.FirstCorinthians] = new(["1co"], ["1 corinthians", "1 corinthian", "1corinthians", "1 chorinthians", "i corinthians", "1cor"]),
        [BookNames.SecondCorinthians] = new(["2co"], ["2 corinthians", "2 corinthian", "2corinthians", "2 chorinthians", "ii corinthians", "2cor"]),
        [BookNames.Galatians] = new(["gal"], ["galatians", "galatian", "galat", "galations"]),
        [BookNames.Ephesians] = new(["eph"], ["ephesians", "ephesian"]),
        [BookNames.Philippians] = new(["php"], ["philippians", "philippian", "phillippians", "phillipians"]),
        [BookNames.Colossians] = new(["col"], ["colossians", "colossian", "collossians", "collosians"]),
        [BookNames.FirstThessalonians] = new(["1th"], ["1 thessalonians", "1 thessalonian", "1thessalonians", "i thessalonians", "1 thessallonian", "1thess"]),
        [BookNames.SecondThessalonians] = new(["2th"], ["2 thessalonians", "2 thessalonian", "2thessalonians", "ii thessalonians", "2 thessalonain", "2thess"]),
        [BookNames.FirstTimothy] = new(["1ti"], ["1 timothy", "1 timothys", "1timothy", "1tim"]),
        [BookNames.SecondTimothy] = new(["2ti"], ["2 timothy", "2 timothys", "2timothy", "2tim"]),
        [BookNames.Titus] = new(["tit"], ["titus"]),
        [BookNames.Philemon] = new(["phm"], ["philemon", "philemons", "phillemon", "philimon", "philemin"]),
        [BookNames.Hebrews] = new(["heb"], ["hebrews", "hebrew"]),
        [BookNames.James] = new(["jas"], ["james"]),
        [BookNames.FirstPeter] = new(["1pe"], ["1 peter", "1peter", "1pet"]),
        [BookNames.SecondPeter] = new(["2pe"], ["2 peter", "2peter", "2pet"]),
        [BookNames.FirstJohn] = new(["1jn"], ["1 john", "1john", "1jn"]),
        [BookNames.SecondJohn] = new(["2jn"], ["2 john", "2john", "2jn"]),
        [BookNames.ThirdJohn] = new(["3jn"], ["3 john", "3john", "3jn"]),
        [BookNames.Jude] = new(["jud"], ["jude"]),
        [BookNames.Revelation] = new(["rev"], ["revelation", "revelations", "revelation of john",
                                          "revalation", "revalations", "revilations", "revilation"]),
    };

    private static readonly Dictionary<string, string> lookup = BuildLookup();

    private static Dictionary<string, string> BuildLookup()
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var (displayName, book) in BookNamesOfBible)
        {
            map[displayName] = displayName;

            foreach (var abbreviation in book.Abbreviations)
            {
                map.TryAdd(abbreviation, displayName);
            }

            foreach (var variant in book.FuzzyMatches)
            {
                map.TryAdd(variant, displayName);
            }
        }

        return map;
    }

    /// <summary>
    /// Tries to get a book name from user input
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string? GetBookName(string input)
    {
        if (string.IsNullOrEmpty(input)) return null;
        return lookup.TryGetValue(input.Trim(), out var bookName) ? bookName : null;
    }

    public static string? GetAbbreviation(string bookName)
    {
        if (string.IsNullOrWhiteSpace(bookName))
            return null;

        if (!BookNamesOfBible.TryGetValue(bookName, out var book))
            return null;

        return book.Abbreviations.FirstOrDefault().ToUpper();
    }

    /// <summary>
    /// Returns true if book is found and the display name
    /// </summary>
    /// <param name="input"></param>
    /// <param name="displayName"></param>
    /// <returns></returns>
    public static bool TryGetBook(string input, out string displayName)
    {
        var result = GetBookName(input);
        displayName = result ?? string.Empty;
        return result is not null;
    }
}
