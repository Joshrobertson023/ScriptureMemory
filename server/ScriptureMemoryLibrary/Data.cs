using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptureMemoryLibrary;
public class Data
{
    public static string[] adminUsernames =
    {
        "Joshrobertson023",
    };

    public static string emailFromAddress = "therealjoshrobertson@gmail.com";

    public static int NOTIFICATION_SYSTEM_ID = 1;
    public static string notificationSystemName = "Scripture Memory Staff";
    public static string welcomeNotificationBody = "To interact with people you know, visit the search page to send them a friend request.";

    public static int NOTIFICATION_SYSTEM_SENDER_ID = 1;

    public static int MIN_PASSWORD_LENGTH = 12;

    public const int MAX_PASSAGES_PER_COLLECTION = 50;

    public const int MAX_COLLECTIONS_PER_USER_FREE = 5;
    public const int MAX_COLLECTIONS_PER_USER_PAID = 255;

    public const int MAX_PUBLISHED_COLLECTIONS_FREE = 5;
    public const int MAX_PUBLISHED_COLLECTIONS_PAID = 255;

    public const int MAX_SAVED_PUBLISHED_COLLECTIONS_FREE = 5;
    public const int MAX_SAVED_PUBLISHED_COLLECTIONS_PAID = 255;

    public static readonly string[] books =
    {
        "Genesis", "Exodus", "Leviticus", "Numbers", "Deuteronomy",
        "Joshua", "Judges", "Ruth", "1 Samuel", "2 Samuel",
        "1 Kings", "2 Kings", "1 Chronicles", "2 Chronicles", "Ezra",
        "Nehemiah", "Esther", "Job", "Psalms", "Proverbs",
        "Ecclesiastes", "Song of Solomon", "Isaiah", "Jeremiah", "Lamentations",
        "Ezekiel", "Daniel", "Hosea", "Joel", "Amos",
        "Obadiah", "Jonah", "Micah", "Nahum", "Habakkuk",
        "Zephaniah", "Haggai", "Zechariah", "Malachi",
        "Matthew", "Mark", "Luke", "John", "Acts",
        "Romans", "1 Corinthians", "2 Corinthians", "Galatians", "Ephesians",
        "Philippians", "Colossians", "1 Thessalonians", "2 Thessalonians", "1 Timothy",
        "2 Timothy", "Titus", "Philemon", "Hebrews", "James",
        "1 Peter", "2 Peter", "1 John", "2 John", "3 John",
        "Jude", "Revelation"
    };

    // Allowed search terms for each book of the Bible
    public static Dictionary<string, string[]> booksOfBibleSearch { get; } = new Dictionary<string, string[]>
    {
        ["Genesis"] = new[] { "genesis", "gen", "geneses", "genisis", },
        ["Exodus"] = new[] { "exodus", "exod", "exodous", "exodis" },
        ["Leviticus"] = new[] { "leviticus", "lev", "liviticus", "levitikus", "leviticis", "levitikis" },
        ["Numbers"] = new[] { "numbers", "num", "number" },
        ["Deuteronomy"] = new[] { "deuteronomy", "deut", "duteronomy", "dutironomy" },
        ["Joshua"] = new[] { "joshua", "josh" },
        ["Judges"] = new[] { "judges", "judge", "judeges" },
        ["Ruth"] = new[] { "ruth", "rut", "ruths" },
        ["1 Samuel"] = new[] { "1 samuel", "1 sam", "1 samul", "1 samuels", "i samuel", "1samuel", "isamuel" },
        ["2 Samuel"] = new[] { "2 samuel", "2 sam", "2 samul", "2 samuels", "ii samuel", "2samuel", "iisamuel" },
        ["1 Kings"] = new[] { "1 kings", "1 king", "1 kgs", "i kings", "1kings", "ikings" },
        ["2 Kings"] = new[] { "2 kings", "2 king", "2 kgs", "ii kings", "iikings", "2kings" },
        ["1 Chronicles"] = new[] { "1 chronicles", "1 chronicle", "1 chr", "1 cronicles", "i chronicles", "i cronicles", "1chronicles" },
        ["2 Chronicles"] = new[] { "2 chronicles", "2 chronicle", "2 chr", "2 cronicles", "ii chronicles", "ii cronicles", "2chronicles" },
        ["Ezra"] = new[] { "ezra" },
        ["Nehemiah"] = new[] { "nehemiah", "neh", "neimiah", "nehimiah" },
        ["Esther"] = new[] { "esther", "ester" },
        ["Job"] = new[] { "job" },
        ["Psalms"] = new[] { "psalms", "psalm", "salm", "salms" },
        ["Proverbs"] = new[] { "proverbs", "prov", "proverb" },
        ["Ecclesiastes"] = new[] { "ecclesiastes", "eccl", "eclesiastes" },
        ["Song of Solomon"] = new[] { "song of solomon", "song of songs", "song of solomons", "songs of solomon", "song of soloman" },
        ["Isaiah"] = new[] { "isaiah", "isaiahs", "isaia", "isaih" },
        ["Jeremiah"] = new[] { "jeremiah", "jeremias", "jerimiah" },
        ["Lamentations"] = new[] { "lamentations", "lamentation", "lamintation", "lamintations" },
        ["Ezekiel"] = new[] { "ezekiel", "ezikiel", "ezekial" },
        ["Daniel"] = new[] { "daniel" },
        ["Hosea"] = new[] { "hosea", "hosiah", "hoseah" },
        ["Joel"] = new[] { "joel" },
        ["Amos"] = new[] { "amos", "amis" },
        ["Obadiah"] = new[] { "obadiah", "obadia", "obadias" },
        ["Jonah"] = new[] { "jonah", "jonas", "jona" },
        ["Micah"] = new[] { "micah", "micha", "mica" },
        ["Nahum"] = new[] { "nahum", "nahums", "nahu" },
        ["Habakkuk"] = new[] { "habakkuk", "habakuk", "habakuk", "habakik", "habakkik" },
        ["Zephaniah"] = new[] { "zephaniah", "zephaniahs", "zephania", "zephiniah" },
        ["Haggai"] = new[] { "haggai", "hagai", "haggiai", "hagiai", "hagaia" },
        ["Zechariah"] = new[] { "zechariah", "zecharaiah", "zecharaiahs", "zachariah" },
        ["Malachi"] = new[] { "malachi", "malachai", "malichai", "malichi" },
        ["Matthew"] = new[] { "matthew", "matthews", "matt", "mathew" },
        ["Mark"] = new[] { "mark", "marks", "marc" },
        ["Luke"] = new[] { "luke", "lukes", "luc" },
        ["John"] = new[] { "john", "johns", "jon" },
        ["Acts"] = new[] { "acts", "act" },
        ["Romans"] = new[] { "romans", "roman" },
        ["1 Corinthians"] = new[] { "1 corinthians", "1 corinthian", "1corinthians", "1 chorinthians", "i corinthians" },
        ["2 Corinthians"] = new[] { "2 corinthians", "2 corinthian", "2corinthians", "2 chorinthians", "ii corinthians" },
        ["Galatians"] = new[] { "galatians", "galatian", "galat", "galations" },
        ["Ephesians"] = new[] { "ephesians", "ephesian", "eph" },
        ["Philippians"] = new[] { "philippians", "philippian", "phil", "phillippians", "phillipians" },
        ["Colossians"] = new[] { "colossians", "colossian", "col", "collossians", "collosians" },
        ["1 Thessalonians"] = new[] { "1 thessalonians", "1 thessalonian", "1thessalonians", "1 thessalonian", "i thessalonians", "i thessalonian", "1 thessallonian" },
        ["2 Thessalonians"] = new[] { "2 thessalonians", "2 thessalonian", "2thessalonians", "2 thessalonain", "ii thessalonians", "ii thessalonian" },
        ["1 Timothy"] = new[] { "1 timothy", "1 timothys", "1timothy" },
        ["2 Timothy"] = new[] { "2 timothy", "2 timothys", "2timothy" },
        ["Titus"] = new[] { "titus", "titis" },
        ["Philemon"] = new[] { "philemon", "philemons", "phillemon" },
        ["Hebrews"] = new[] { "hebrews", "hebrew" },
        ["James"] = new[] { "james" },
        ["1 Peter"] = new[] { "1 peter", "1peter" },
        ["2 Peter"] = new[] { "2 peter", "2peter" },
        ["1 John"] = new[] { "1 john", "1john" },
        ["2 John"] = new[] { "2 john", "2john" },
        ["3 John"] = new[] { "3 john", "3john" },
        ["Jude"] = new[] { "jude" },
        ["Revelation"] = new[] { "revelation", "revelations", "rev", "revelation of john", "revalation", "revalations", "revilations", "revilation" }
    };
}
