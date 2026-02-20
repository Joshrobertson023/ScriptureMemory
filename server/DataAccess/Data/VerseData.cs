using Dapper;
using DataAccess.DataInterfaces;
using DataAccess.Models;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ScriptureMemoryLibrary;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccess.Data;

public class VerseData : IVerseData
{
    private readonly IConfiguration _config;
    private readonly string connectionString;
    private readonly ICategoryData? _categoryData;

    public VerseData(IConfiguration config, ICategoryData? categoryData = null)
    {
        _config = config;
        connectionString = _config.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");
        _categoryData = categoryData;
    }
}

//    public async Task<List<Verse>> GetAllVerses(int offset, int nextFetch)
//    {
//        var sql = $@"SELECT * FROM VERSES OFFSET :offset ROWS FETCH NEXT :nextFetch ROWS ONLY";
//        await using var conn = new OracleConnection(connectionString);
//        var results = await conn.QueryAsync<Verse>(sql, new { offset = offset, nextFetch = nextFetch });

//        return results.ToList();
//    }

//    public async Task InsertVerse(Verse verse)
//    {
//        var sql = $@"INSERT INTO VERSES (verse_reference, Text)
//                     VALUES
//                     (:Reference, :Text)";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql,
//            new { verse.Reference, verse.Text },
//            commandType: CommandType.Text);
//    }

//    public async Task<Verse?> GetVerse(string reference)
//    {
//        string sql = $@"SELECT * FROM VERSES WHERE verse_reference = :Reference";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var results = await conn.QueryAsync<Verse?>(sql, new { Reference = reference });
//        return results.FirstOrDefault();
//    }

//    public async Task<List<Verse>> GetChapterVerses(string book, int chapter)
//    {
//        var verseReferences = new List<string>();
//        const int mostVersesInChapter = 176;

//        for (int verse = 1; verse <= mostVersesInChapter; verse++)
//        {
//            verseReferences.Add($"{book} {chapter}:{verse}");
//        }

//        bool first = true;
//        StringBuilder sql = new StringBuilder("SELECT * FROM VERSES WHERE verse_reference IN (");
//        foreach (var reference in verseReferences)
//        {
//            if (first) sql.Append($"\'{reference}\'");
//            else sql.Append($",\'{reference}\'");
//            first = false;
//        }
//        sql.Append(")");

//        await using var conn = new OracleConnection(connectionString);
//        var verses = await conn.QueryAsync<Verse>(sql.ToString(), commandType: CommandType.Text);
//        return verses.ToList();
//    }

//    public async Task<Verse?> GetVerseFromId(int id)
//    {
//        var sql = "SELECT * FROM VERSES WHERE verse_id = :id";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var userVerses = await conn.QueryAsync<Verse?>(sql, new { id = id },
//            commandType: CommandType.Text);
//        return userVerses.FirstOrDefault();
//    }

//    public async Task UpdateVerseText(string text, int id)
//    {
//        var sql = "update verses set text = :newText where verse_id = :id";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new { newText = text, id = id },
//            commandType: CommandType.Text);
//        return;
//    }

//    public async Task UpdateUsersSavedVerse(string reference)
//    {
//        var sql = @"UPDATE VERSES SET Users_Saved_Verse = Users_Saved_Verse + 1
//                     WHERE verse_reference = :Reference";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new { Reference = reference },
//            commandType: CommandType.Text);
//    }

//    public async Task UpdateUsersMemorizedVerse(string reference)
//    {
//        var sql = @"UPDATE VERSES SET Users_Memorized = Users_Memorized + 1
//                     WHERE verse_reference = :Reference";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        await conn.ExecuteAsync(sql, new { Reference = reference },
//            commandType: CommandType.Text);
//    }

//    public async Task<IEnumerable<Verse>> GetTopSavedVerses(int top)
//    {
//        var sql = $@"SELECT * FROM (
//                        SELECT * FROM VERSES
//                        ORDER BY USERS_SAVED_VERSE DESC, VERSE_ID DESC
//                        FETCH FIRST 20 ROWS ONLY
//                    )
//                    WHERE USERS_SAVED_VERSE > 0";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var verses = await conn.QueryAsync<Verse>(sql, commandType: CommandType.Text);
//        return verses;
//    }

//    public async Task<IEnumerable<Verse>> GetTopMemorizedVerses(int top)
//    {
//        var limit = Math.Max(1, top);
//        var sql = $@"SELECT * FROM (
//                        SELECT * FROM VERSES
//                        ORDER BY USERS_MEMORIZED DESC, VERSE_ID DESC
//                        FETCH FIRST 20 ROWS ONLY
//                    )
//                    WHERE USERS_MEMORIZED > 0";
//        using IDbConnection conn = new OracleConnection(connectionString);
//        var verses = await conn.QueryAsync<Verse>(sql, commandType: CommandType.Text);
//        return verses;
//    }

//    public async Task<List<Verse>> GetAllVersesFromReferenceList(List<string> references)
//    {
//        bool first = true;
//        StringBuilder sql = new("SELECT * FROM VERSES WHERE verse_reference IN (");
//        using IDbConnection conn = new OracleConnection(connectionString);
//        foreach (var reference in references)
//        {
//            if (first) sql.Append($"\'{reference}\'");
//            else sql.Append($",\'{reference}\'");
//            first = false;
//        }
//        sql.Append(")");

//        Debug.WriteLine(sql);

//        var resultVerses = await conn.QueryAsync<Verse>(sql.ToString(), commandType: CommandType.Text);

//        return resultVerses.ToList();
//    }

//    public async Task<Models.SearchData> GetVerseSearchResults(string search)
//    {
//        try
//        {
//            int i = 0;
//            if (string.IsNullOrWhiteSpace(search))
//                throw new Exception("Invalid search.");

//            search = search.Trim();
//            string reference = string.Empty;
//            string firstWord = string.Empty;

//            // Get the first word (handle single-word queries)
//            string[] words = search.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
//            if (words.Length == 0)
//                throw new Exception("Invalid search.");

//            firstWord = words[0].Trim();

//            // Remove trailing punctuation from first word (but keep it for book matching)
//            string firstWordClean = firstWord.ToLower();
//            while (firstWordClean.Length > 0 && char.IsPunctuation(firstWordClean[firstWordClean.Length - 1]))
//            {
//                firstWordClean = firstWordClean.Substring(0, firstWordClean.Length - 1);
//            }

//            Debug.WriteLine("firstWord: " + firstWordClean);

//            // Check if the first word is a book of the Bible
//            bool isValidBook = false;
//            foreach (var book in VerseAppLibrary.Data.booksOfBibleSearch)
//            {
//                if (book.Value.Contains(firstWordClean))
//                {
//                    firstWord = book.Key;
//                    reference = $"{book.Key} ";
//                    isValidBook = true;
//                    break;
//                }
//            }
//            Debug.WriteLine("firstWord after book check: " + firstWord + ", isValidBook: " + isValidBook);

//            // Check if search starts with "verses about" or "passages about" (case-insensitive)
//            var searchLower = search.ToLower();
//            string? categorySearchTerm = null;
//            if (searchLower.StartsWith("verses about ") || searchLower.StartsWith("passages about "))
//            {
//                // Extract text after "verses about " or "passages about "
//                var aboutIndex = searchLower.IndexOf(" about ");
//                if (aboutIndex >= 0)
//                {
//                    categorySearchTerm = search.Substring(aboutIndex + " about ".Length).Trim();
//                    Debug.WriteLine($"Detected category search pattern. Searching for category: {categorySearchTerm}");
//                }
//            }

//            // If we have a category search term, search by category first
//            List<Verse> categoryVerses = new List<Verse>();
//            if (!string.IsNullOrWhiteSpace(categorySearchTerm) && _categoryData != null)
//            {
//                var category = await _categoryData.GetCategoryByName(categorySearchTerm);
//                if (category != null)
//                {
//                    Debug.WriteLine($"Found category: {category.Name} (ID: {category.CategoryId})");
//                    categoryVerses = await GetTopVersesInCategory(100, category.CategoryId);
//                    Debug.WriteLine($"Found {categoryVerses.Count} verses in category");
//                }
//            }

//            if (!isValidBook)
//            { 
//                // Not a valid book - search by keyword/phrase
//                // Use the original search query (not the modified one with trailing space)
//                // If we have a category search term, also search by that term
//                var searchPhrase = !string.IsNullOrWhiteSpace(categorySearchTerm) ? categorySearchTerm : search;
//                var phraseResults = await SearchByPhrase(searchPhrase);
//                // Filter out error messages and null results, but keep all valid verses
//                var resultVerses = phraseResults
//                    .Where(r => r != null && 
//                           !string.IsNullOrEmpty(r.verse_reference) && 
//                           !r.verse_reference.Contains("No more results") &&
//                           !r.verse_reference.Contains("No more results found"))
//                    .ToList();

//                // Combine category verses with phrase search results, removing duplicates
//                var allVerses = new HashSet<string>();
//                var combinedVerses = new List<Verse>();

//                // Add category verses first
//                foreach (var verse in categoryVerses)
//                {
//                    if (verse != null && !string.IsNullOrEmpty(verse.verse_reference) && !allVerses.Contains(verse.verse_reference))
//                    {
//                        allVerses.Add(verse.verse_reference);
//                        combinedVerses.Add(verse);
//                    }
//                }

//                // Add phrase search results
//                foreach (var verse in resultVerses)
//                {
//                    if (verse != null && !string.IsNullOrEmpty(verse.verse_reference) && !allVerses.Contains(verse.verse_reference))
//                    {
//                        allVerses.Add(verse.verse_reference);
//                        combinedVerses.Add(verse);
//                    }
//                }

//                return new Models.SearchData
//                {
//                    Readable_Reference = "",
//                    Searched_By_Passage = false,
//                    Verses = combinedVerses
//                };
//            }

//            // Find the position after the first word in the original search string
//            // This is needed for chapter/verse parsing
//            // Use the original first word from words array (before book normalization)
//            string originalFirstWord = words[0].Trim();
//            int firstWordEndIndex = search.IndexOf(originalFirstWord, StringComparison.OrdinalIgnoreCase);
//            if (firstWordEndIndex >= 0)
//            {
//                i = firstWordEndIndex + originalFirstWord.Length;
//            }
//            else
//            {
//                // Fallback: find first space or end of string
//                int spaceIndex = search.IndexOf(' ');
//                i = spaceIndex >= 0 ? spaceIndex : search.Length;
//            }

//            // Reset search string for book-based parsing (add space back for parsing logic)
//            search = search + " ";

//            // Get the chapter (optional - might be book-only search)
//            string _book = firstWord;
//            int _chapter = 0;
//            string chapterString = string.Empty;
//            bool hasChapter = false;
//            int chapterStartIndex = i;

//            for (; i < search.Length; i++)
//            {
//                if (char.IsNumber(search[i]))
//                {
//                    chapterString += search[i];
//                    hasChapter = true;
//                    if (i < search.Length - 1)
//                    {
//                        if (!char.IsNumber(search[i + 1]))
//                            break;
//                        if (i + 1 == search.Length)
//                            break; // Allow chapter at end of string
//                    }
//                }
//            }

//            // If no chapter found, this is a book-only search
//            if (!hasChapter || string.IsNullOrWhiteSpace(chapterString))
//            {
//                // Search for entire book
//                using (IDbConnection conn = new OracleConnection(connectionString))
//                {
//                    var bookPattern = _book + " %";
//                    var sql = @"SELECT * FROM VERSES 
//                                WHERE UPPER(verse_reference) LIKE UPPER(:BookPattern)
//                                ORDER BY verse_id
//                                FETCH FIRST 50 ROWS ONLY";
//                    var verses = await conn.QueryAsync<Verse>(
//                        sql,
//                        new { BookPattern = bookPattern },
//                        commandType: CommandType.Text);

//                    var foundVerses = verses.ToList();
//                    foreach (var verse in foundVerses)
//                    {
//                        if (verse != null)
//                            verse.Verse_Number = ReferenceParse.GetVerseNumber(verse.verse_reference);
//                    }

//                    if (foundVerses.Count == 0)
//                        throw new Exception("Nothing came back from that search. Please check your spelling.");

//                    return new Models.SearchData 
//                    { 
//                        Searched_By_Passage = true, 
//                        Verses = foundVerses, 
//                        Readable_Reference = _book 
//                    };
//                }
//            }

//            _chapter = int.TryParse(chapterString, out int chapterResult)
//                ? chapterResult
//                : throw new Exception("Error getting chapter.");
//            reference += _chapter.ToString() + ":";
//            Debug.WriteLine($"Book: {_book}, Chapter: {_chapter}");

//            // Get the verses
//            List<int> _verses = new();
//            string numberString = string.Empty;
//            List<string> verseRanges = new();
//            string verseRange = string.Empty;

//            i++;
//            for (int searchIndex = i + 1; searchIndex < search.Length; searchIndex++)
//            {
//                reference += search[searchIndex];
//            }

//            verseRange = ",";
//            for (; i < search.Length; i++)
//            {
//                // Get next verse number as string
//                numberString = "";
//                for (; i < search.Length; i++)
//                {
//                    if (char.IsNumber(search[i]))
//                    {
//                        numberString += search[i];
//                        if (i < search.Length - 1)
//                            if (!char.IsNumber(search[i + 1]))
//                                break;
//                    }
//                }

//                if (i < search.Length - 1)
//                    Debug.WriteLine("Search[i]: " + search[i]);
//                if (verseRange[verseRange.Length - 1] == '-')
//                {
//                    verseRange += numberString;
//                    verseRanges.Add(verseRange);
//                    Debug.WriteLine("verseRange ends with '-'. Adding " + numberString + " to verseRange: " + verseRange);
//                    numberString = string.Empty;
//                    verseRange = ",";
//                }
//                else
//                {
//                    if (i < search.Length - 1)
//                    {
//                        if (search[i + 1] == '-')
//                        {
//                            verseRange += numberString + "-";
//                            Debug.WriteLine("verseRange does not end with '-'. Adding" + numberString + " + \"-\" to verseRange: " + verseRange);
//                            numberString = string.Empty;
//                        }
//                        else
//                        {
//                            Debug.WriteLine("Adding to verses, numberString: " + numberString);
//                            _verses.Add(int.TryParse(numberString, out int verseResult) ? verseResult : throw new Exception("Error getting verse."));
//                            numberString = string.Empty;
//                        }
//                        Debug.WriteLine("verseRange: " + verseRange);
//                        Debug.WriteLine("search[i]: " + search[i]);
//                    }
//                }
//            }

//            // Get all individual verses from verse ranges
//            foreach (var _range in verseRanges)
//            {
//                string range = _range + ",";
//                string firstNumber = string.Empty;
//                string secondNumber = string.Empty;
//                int rangeIndex = 0;
//                for (; rangeIndex < range.Length; rangeIndex++)
//                {
//                    if (char.IsNumber(range[rangeIndex]))
//                    {
//                        firstNumber += range[rangeIndex];
//                        if (rangeIndex < range.Length - 1)
//                            if (!char.IsNumber(range[rangeIndex + 1]))
//                                break;
//                    }
//                }
//                rangeIndex++;
//                for (; rangeIndex < range.Length; rangeIndex++)
//                {
//                    if (char.IsNumber(range[rangeIndex]))
//                    {
//                        secondNumber += range[rangeIndex];
//                        if (rangeIndex < range.Length - 1)
//                            if (!char.IsNumber(range[rangeIndex + 1]))
//                                break;
//                    }
//                }

//                int firstVerseInRange = int.TryParse(firstNumber, out int firstResult) ? firstResult : throw new Exception("Error getting first verse in range.");
//                int secondVerseInRange = int.TryParse(secondNumber, out int secondResult) ? secondResult : throw new Exception("Error getting second verse in range.");

//                Debug.WriteLine($"Adding verses from range: {firstVerseInRange} to {secondVerseInRange}");
//                if (firstVerseInRange > secondVerseInRange)
//                {
//                    int temp = firstVerseInRange;
//                    firstVerseInRange = secondVerseInRange;
//                    secondVerseInRange = temp;
//                }

//                for (int verseRangeIndex = firstVerseInRange; verseRangeIndex <= secondVerseInRange; verseRangeIndex++)
//                {
//                    if (!_verses.Contains(verseRangeIndex))
//                        _verses.Add(verseRangeIndex);
//                }

//            }

//            Debug.WriteLine($"Book: {_book}, Chapter: {_chapter}, Verses: {string.Join(", ", _verses)}");

//            using (IDbConnection conn = new OracleConnection(connectionString))
//            {
//                Debug.WriteLine("Running!!");
//                var foundVerses = new List<Verse>();

//                if (_verses.Count <= 0)
//                {
//                    // Get chapter
//                    Debug.WriteLine("Getting whole chapter.");
//                    var chapterVerses = await GetChapterVerses(_book, _chapter);
//                    foundVerses = chapterVerses.ToList();
//                }
//                else
//                {
//                    // Search by reference
//                    Debug.WriteLine("_verses.Count: " + _verses.Count);
//                    foreach (int v in _verses)
//                    {
//                        string _reference = $"{firstWord} {_chapter}:{v}";
//                        Debug.WriteLine("Searching for passage: " + _reference);
//                        var sql = "SELECT * FROM VERSES WHERE verse_reference = :Reference";
//                        var verse = await conn.QueryAsync<Verse>(
//                            sql,
//                            new { Reference = _reference },
//                            commandType: CommandType.Text);

//                        var firstVerse = verse?.FirstOrDefault();
//                        if (firstVerse != null)
//                            foundVerses.Add(firstVerse);
//                    }
//                }

//                foreach (var verse in foundVerses)
//                {
//                    if (verse == null)
//                        throw new Exception("Not a valid verse");
//                    verse.Verse_Number = ReferenceParse.GetVerseNumber(verse.verse_reference);
//                }

//                Debug.WriteLine(reference);

//                if (foundVerses.Count == 0)
//                    throw new Exception("Nothing came back from that search. Please check your spelling.");

//                return new Models.SearchData { Searched_By_Passage = true, Verses = foundVerses, Readable_Reference = reference };
//            }
//        }
//        catch (Exception ex)
//        {
//            return new Models.SearchData { Searched_By_Passage = false, 
//                Verses = new List<Verse> { new Verse { verse_reference = ex.Message } },
//            Readable_Reference = ""};
//        }
//    }

//    public async Task<List<Verse>> SearchByPhrase(string query)
//    {
//        List<Verse> verses = new List<Verse>();
//        HashSet<string> added = new HashSet<string>();

//        if (string.IsNullOrWhiteSpace(query))
//        {
//            verses.Add(new Verse { verse_reference = "No more results. Check your spelling.", Text = "" });
//            return verses;
//        }

//        string[] queryWords = query.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
//        List<string> keywords = queryWords.Where(k => !string.IsNullOrWhiteSpace(k)).ToList();

//        if (keywords.Count == 0)
//        {
//            verses.Add(new Verse { verse_reference = "No more results. Check your spelling.", Text = "" });
//            return verses;
//        }

//        if (keywords.Count == 1)
//        {
//            // Get verses with most occurrences of keyword
//            var singleKeywordResults = await SingleKeywordHighestCount(keywords[0]);
//            foreach (var verse in singleKeywordResults)
//            {
//                if (verse != null && !string.IsNullOrEmpty(verse.verse_reference))
//                {
//                    if (added.Add(verse.verse_reference))
//                        verses.Add(verse);
//                }
//            }

//            if (verses.Count > 0)
//                return verses;
//        }

//        List<Verse> nearExactAnd = new List<Verse>();
//        List<Verse> nearAnd = new List<Verse>();
//        List<Verse> andVerses = new List<Verse>();
//        List<Verse> exactPhrase = new List<Verse>();
//        List<Verse> exactAnd = new List<Verse>();

//        if (keywords.Count == 2)
//        {
//            nearExactAnd = await NearExactAndVerses(keywords);
//            nearAnd = await NearAnd(keywords);
//        }
//        if (keywords.Count > 2 && keywords.Count < 5)
//        {
//            exactPhrase = await ExactPhraseVerses(keywords);
//            exactAnd = await ExactAndVerses(keywords);
//            andVerses = await AndVerses(keywords);
//        }
//        if (keywords.Count >= 5)
//        {
//            exactPhrase = await ExactPhraseVerses(keywords);
//            exactAnd = await ExactAndVerses(keywords);
//        }

//        foreach (var verse in exactPhrase)
//        {
//            if (verse != null && !string.IsNullOrEmpty(verse.verse_reference))
//            {
//                if (added.Add(verse.verse_reference))
//                    verses.Add(verse);
//            }
//        }

//        foreach (var verse in nearExactAnd)
//        {
//            if (verse != null && !string.IsNullOrEmpty(verse.verse_reference))
//            {
//                if (added.Add(verse.verse_reference))
//                    verses.Add(verse);
//            }
//        }

//        foreach (var verse in exactAnd)
//        {
//            if (verse != null && !string.IsNullOrEmpty(verse.verse_reference))
//            {
//                if (added.Add(verse.verse_reference))
//                    verses.Add(verse);
//            }
//        }

//        foreach (var verse in nearAnd)
//        {
//            if (verse != null && !string.IsNullOrEmpty(verse.verse_reference))
//            {
//                if (added.Add(verse.verse_reference))
//                    verses.Add(verse);
//            }
//        }

//        foreach (var verse in andVerses)
//        {
//            if (verse != null && !string.IsNullOrEmpty(verse.verse_reference))
//            {
//                if (added.Add(verse.verse_reference))
//                    verses.Add(verse);
//            }
//        }

//        if (verses.Count == 0)
//            verses.Add(new Verse { verse_reference = "No more results. Check your spelling.", Text = "" });

//        return verses;
//    }


//    public async Task<List<Verse>> SingleKeywordHighestCount(string keyword)
//    {
//        List<Verse> verses = new List<Verse>();

//        if (string.IsNullOrWhiteSpace(keyword))
//        {
//            return verses;
//        }

//        string cleanKeyword = keyword.Trim().ToLower();

//        string query = @"SELECT * FROM VERSES WHERE CONTAINS(TEXT, :keyword, 1) > 0 ORDER BY SCORE(1) DESC FETCH FIRST 20 ROWS ONLY";

//        try
//        {
//            using OracleConnection conn = new OracleConnection(connectionString);
//            await conn.OpenAsync();
//            using OracleCommand cmd = new OracleCommand(query, conn);

//            string searchTerm = "{" + cleanKeyword + "}";
//            cmd.Parameters.Add(new OracleParameter("keyword", searchTerm));
//            OracleDataReader reader = await cmd.ExecuteReaderAsync();

//            while (await reader.ReadAsync())
//            {
//                Verse verse = new Verse
//                {
//                    Id = reader.GetInt32(reader.GetOrdinal("VERSE_ID")),
//                    verse_reference = reader.GetString(reader.GetOrdinal("VERSE_REFERENCE")),
//                    Users_Saved_Verse = reader.GetInt32(reader.GetOrdinal("USERS_SAVED_VERSE")),
//                    Users_Memorized = reader.GetInt32(reader.GetOrdinal("USERS_MEMORIZED")),
//                    Text = reader.GetString(reader.GetOrdinal("TEXT"))
//                };
//                verses.Add(verse);
//            }

//            reader.Close();

//            if (verses.Count == 0)
//            {
//                using OracleCommand cmd2 = new OracleCommand(query, conn);
//                string prefixTerm = cleanKeyword + "*";
//                cmd2.Parameters.Add(new OracleParameter("keyword", prefixTerm));
//                OracleDataReader reader2 = await cmd2.ExecuteReaderAsync();

//                while (await reader2.ReadAsync())
//                {
//                    Verse verse = new Verse
//                    {
//                        Id = reader2.GetInt32(reader2.GetOrdinal("VERSE_ID")),
//                        verse_reference = reader2.GetString(reader2.GetOrdinal("VERSE_REFERENCE")),
//                        Users_Saved_Verse = reader2.GetInt32(reader2.GetOrdinal("USERS_SAVED_VERSE")),
//                        Users_Memorized = reader2.GetInt32(reader2.GetOrdinal("USERS_MEMORIZED")),
//                        Text = reader2.GetString(reader2.GetOrdinal("TEXT"))
//                    };
//                    verses.Add(verse);
//                }

//                reader2.Close();
//            }

//            conn.Close();
//            conn.Dispose();
//        }
//        catch (Exception ex)
//        {
//            Debug.WriteLine($"Error in SingleKeywordHighestCount: {ex.Message}");
//        }

//        return verses;
//    }

//    public async Task<List<Verse>> ExactPhraseVerses(List<string> keywords)
//    {
//        List<Verse> verses = new List<Verse>();

//        string phrase = "\"" + string.Join(" ", keywords) + "\"";

//        string query = @"SELECT * FROM VERSES WHERE CONTAINS(TEXT, :phrase, 1) > 0 ORDER BY SCORE(1) DESC FETCH FIRST 20 ROWS ONLY";

//        using OracleConnection conn = new OracleConnection(connectionString);
//        await conn.OpenAsync();

//        using OracleCommand cmd = new OracleCommand(query, conn);
//        cmd.Parameters.Add(new OracleParameter("phrase", phrase));
//        OracleDataReader reader = await cmd.ExecuteReaderAsync();

//        while (await reader.ReadAsync())
//        {
//            Verse verse = new Verse
//            {
//                Id = reader.GetInt32(reader.GetOrdinal("VERSE_ID")),
//                verse_reference = reader.GetString(reader.GetOrdinal("VERSE_REFERENCE")),
//                Users_Saved_Verse = reader.GetInt32(reader.GetOrdinal("USERS_SAVED_VERSE")),
//                Users_Memorized = reader.GetInt32(reader.GetOrdinal("USERS_MEMORIZED")),
//                Text = reader.GetString(reader.GetOrdinal("TEXT"))
//            };
//            verses.Add(verse);
//        }

//        conn.Close();
//        conn.Dispose();
//        return verses;
//    }

//    public async Task<List<Verse>> NearExactAndVerses(List<string> keywords)
//    {
//        List<Verse> verses = new List<Verse>();

//        string query = @"SELECT * FROM VERSES WHERE CONTAINS(TEXT, :keywords, 1) > 0 ORDER BY SCORE(1) DESC FETCH FIRST 20 ROWS ONLY";

//        using OracleConnection conn = new OracleConnection(connectionString);
//        await conn.OpenAsync();

//        using OracleCommand cmd = new OracleCommand(query, conn);

//        List<string> parts = new List<string>();
//        foreach (var _keyword in keywords)
//        {
//            string cleanKeyword = _keyword.Trim().ToLower();
//            if (!string.IsNullOrEmpty(cleanKeyword))
//                parts.Add(cleanKeyword);
//        }
//        string nearKeywords = string.Join(" NEAR ", parts);

//        cmd.Parameters.Add(new OracleParameter("keywords", nearKeywords));
//        OracleDataReader reader = await cmd.ExecuteReaderAsync();

//        while (await reader.ReadAsync())
//        {
//            Verse verse = new Verse
//            {
//                Id = reader.GetInt32(reader.GetOrdinal("VERSE_ID")),
//                verse_reference = reader.GetString(reader.GetOrdinal("VERSE_REFERENCE")),
//                Users_Saved_Verse = reader.GetInt32(reader.GetOrdinal("USERS_SAVED_VERSE")),
//                Users_Memorized = reader.GetInt32(reader.GetOrdinal("USERS_MEMORIZED")),
//                Text = reader.GetString(reader.GetOrdinal("TEXT"))
//            };
//            verses.Add(verse);
//        }

//        conn.Close();
//        conn.Dispose();
//        return verses;
//    }

//    public async Task<List<Verse>> ExactAndVerses(List<string> keywords)
//    {
//        List<Verse> verses = new List<Verse>();

//        string query = @"SELECT * FROM VERSES WHERE CONTAINS(TEXT, :keywords, 1) > 0 ORDER BY SCORE(1) DESC FETCH FIRST 20 ROWS ONLY";

//        using OracleConnection conn = new OracleConnection(connectionString);
//        await conn.OpenAsync();

//        using OracleCommand cmd = new OracleCommand(query, conn);

//        List<string> parts = new List<string>();
//        foreach (var _keyword in keywords)
//        {
//            string cleanKeyword = _keyword.Trim().ToLower();
//            if (!string.IsNullOrEmpty(cleanKeyword))
//                parts.Add(cleanKeyword);
//        }
//        string andKeywords = string.Join(" AND ", parts);

//        cmd.Parameters.Add(new OracleParameter("keywords", andKeywords));
//        OracleDataReader reader = await cmd.ExecuteReaderAsync();

//        while (await reader.ReadAsync())
//        {
//            Verse verse = new Verse
//            {
//                Id = reader.GetInt32(reader.GetOrdinal("VERSE_ID")),
//                verse_reference = reader.GetString(reader.GetOrdinal("VERSE_REFERENCE")),
//                Users_Saved_Verse = reader.GetInt32(reader.GetOrdinal("USERS_SAVED_VERSE")),
//                Users_Memorized = reader.GetInt32(reader.GetOrdinal("USERS_MEMORIZED")),
//                Text = reader.GetString(reader.GetOrdinal("TEXT"))
//            };
//            verses.Add(verse);
//        }

//        conn.Close();
//        conn.Dispose();
//        return verses;
//    }

//    public async Task<List<Verse>> NearAnd(List<string> keywords)
//    {
//        List<Verse> verses = new List<Verse>();

//        string query = @"SELECT * FROM VERSES WHERE CONTAINS(TEXT, :keywords, 1) > 0 ORDER BY SCORE(1) DESC FETCH FIRST 50 ROWS ONLY";

//        using OracleConnection conn = new OracleConnection(connectionString);
//        await conn.OpenAsync();

//        using OracleCommand cmd = new OracleCommand(query, conn);

//        List<string> parts = new List<string>();
//        foreach (var _keyword in keywords)
//        {
//            string cleanKeyword = _keyword.Trim().ToLower();
//            if (!string.IsNullOrEmpty(cleanKeyword))
//                parts.Add(cleanKeyword);
//        }
//        string nearKeywords = string.Join(" NEAR ", parts);

//        cmd.Parameters.Add(new OracleParameter("keywords", nearKeywords));
//        OracleDataReader reader = await cmd.ExecuteReaderAsync();

//        while (await reader.ReadAsync())
//        {
//            Verse verse = new Verse
//            {
//                Id = reader.GetInt32(reader.GetOrdinal("VERSE_ID")),
//                verse_reference = reader.GetString(reader.GetOrdinal("VERSE_REFERENCE")),
//                Users_Saved_Verse = reader.GetInt32(reader.GetOrdinal("USERS_SAVED_VERSE")),
//                Users_Memorized = reader.GetInt32(reader.GetOrdinal("USERS_MEMORIZED")),
//                Text = reader.GetString(reader.GetOrdinal("TEXT"))
//            };
//            verses.Add(verse);
//        }

//        conn.Close();
//        conn.Dispose();
//        return verses;
//    }

//    public async Task<List<Verse>> AndVerses(List<string> keywords)
//    {
//        List<Verse> verses = new List<Verse>();

//        string query = @"SELECT * FROM VERSES WHERE CONTAINS(TEXT, :keywords, 1) > 0 ORDER BY SCORE(1) DESC FETCH FIRST 49 ROWS ONLY";

//        using OracleConnection conn = new OracleConnection(connectionString);
//        await conn.OpenAsync();

//        using OracleCommand cmd = new OracleCommand(query, conn);

//        List<string> parts = new List<string>();
//        foreach (var _keyword in keywords)
//        {
//            string cleanKeyword = _keyword.Trim().ToLower();
//            if (!string.IsNullOrEmpty(cleanKeyword))
//                parts.Add(cleanKeyword);
//        }
//        string andKeywords = string.Join(" AND ", parts);

//        cmd.Parameters.Add(new OracleParameter("keywords", andKeywords));
//        OracleDataReader reader = await cmd.ExecuteReaderAsync();

//        while (await reader.ReadAsync())
//        {
//            Verse verse = new Verse
//            {
//                Id = reader.GetInt32(reader.GetOrdinal("VERSE_ID")),
//                verse_reference = reader.GetString(reader.GetOrdinal("VERSE_REFERENCE")),
//                Users_Saved_Verse = reader.GetInt32(reader.GetOrdinal("USERS_SAVED_VERSE")),
//                Users_Memorized = reader.GetInt32(reader.GetOrdinal("USERS_MEMORIZED")),
//                Text = reader.GetString(reader.GetOrdinal("TEXT"))
//            };
//            verses.Add(verse);
//        }

//        conn.Close();
//        conn.Dispose();
//        return verses;
//    }

//    public async Task<List<Verse>> OrVerses(List<string> keywords)
//    {
//        List<Verse> verses = new List<Verse>();

//        string query = @"SELECT * FROM VERSES WHERE CONTAINS(TEXT, :keywords, 1) > 0 ORDER BY SCORE(1) DESC FETCH FIRST 20 ROWS ONLY";

//        using OracleConnection conn = new OracleConnection(connectionString);
//        await conn.OpenAsync();

//        using OracleCommand cmd = new OracleCommand(query, conn);

//        List<string> parts = new List<string>();
//        foreach (var _keyword in keywords)
//        {
//            string cleanKeyword = _keyword.Trim().ToLower();
//            if (!string.IsNullOrEmpty(cleanKeyword))
//                parts.Add(cleanKeyword);
//        }
//        string orKeywords = string.Join(" OR ", parts);

//        cmd.Parameters.Add(new OracleParameter("keywords", orKeywords));
//        OracleDataReader reader = await cmd.ExecuteReaderAsync();

//        while (await reader.ReadAsync())
//        {
//            Verse verse = new Verse
//            {
//                Id = reader.GetInt32(reader.GetOrdinal("VERSE_ID")),
//                verse_reference = reader.GetString(reader.GetOrdinal("VERSE_REFERENCE")),
//                Users_Saved_Verse = reader.GetInt32(reader.GetOrdinal("USERS_SAVED_VERSE")),
//                Users_Memorized = reader.GetInt32(reader.GetOrdinal("USERS_MEMORIZED")),
//                Text = reader.GetString(reader.GetOrdinal("TEXT"))
//            };
//            verses.Add(verse);
//        }

//        conn.Close();
//        conn.Dispose();
//        return verses;
//    }

//    public async Task<List<Verse>> GetTopVersesInCategory(int top, int categoryId)
//    {
//        using IDbConnection conn = new OracleConnection(connectionString);

//        var getDirectVersesSql = @"
//            SELECT VERSE_REFERENCE
//            FROM VERSE_CATEGORIES
//            WHERE CATEGORY_ID = :CategoryId";

//        var directVerseReferences = await conn.QueryAsync<string>(getDirectVersesSql, new { CategoryId = categoryId }, commandType: CommandType.Text);

//        var getCollectionsSql = @"
//            SELECT DISTINCT p.VERSE_ORDER
//            FROM PUBLISHED_COLLECTIONS p
//            INNER JOIN COLLECTION_CATEGORIES cc ON p.PUBLISHED_ID = cc.PUBLISHED_ID
//            WHERE cc.CATEGORY_ID = :CategoryId
//              AND p.VERSE_ORDER IS NOT NULL
//              AND p.VERSE_ORDER != ''";

//        var verseOrderStrings = await conn.QueryAsync<string>(getCollectionsSql, new { CategoryId = categoryId }, commandType: CommandType.Text);

//        var allVerseReferences = new HashSet<string>(directVerseReferences);

//        foreach (var verseOrder in verseOrderStrings)
//        {
//            if (string.IsNullOrWhiteSpace(verseOrder)) continue;
//            var references = verseOrder.Split(',', StringSplitOptions.RemoveEmptyEntries)
//                .Select(r => r.Trim())
//                .Where(r => !string.IsNullOrWhiteSpace(r));
//            foreach (var verseRef in references)
//            {
//                allVerseReferences.Add(verseRef);
//            }
//        }

//        if (allVerseReferences.Count == 0)
//        {
//            return new List<Verse>();
//        }

//        var quotedReferences = allVerseReferences.Select(r => $"'{r.Replace("'", "''")}'");
//        var referencesList = string.Join(",", quotedReferences);

//        var sql = $@"
//            SELECT * FROM VERSES
//            WHERE VERSE_REFERENCE IN ({referencesList})
//            ORDER BY USERS_SAVED_VERSE DESC
//            FETCH FIRST :Top ROWS ONLY";

//        var results = await conn.QueryAsync<Verse>(sql, new { Top = top }, commandType: CommandType.Text);

//        return results.ToList();
//    }

//    public async Task<string> GetPassageTextFromListOfReferences(List<string> references)
//    {
//        var quotedReferences = references.Select(r => $"'{r.Replace("'", "''")}'");
//        var referencesList = string.Join(",", quotedReferences);

//        var sql = $@"SELECT TEXT FROM VERSES WHERE VERSE_REFERENCE IN ({referencesList})";

//        using IDbConnection conn = new OracleConnection(connectionString);
//        var results = await conn.QueryAsync<string>(sql, commandType: CommandType.Text);
//        var allText = string.Join(" ", results);
//        return allText;
//    }
//}
