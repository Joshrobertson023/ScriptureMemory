using System.Net.Http.Headers;
using System.Text.Json;
using static ScriptureMemory.Server.Tools.BibleData;

namespace ScriptureMemory.Server.Tools;

public class ApiResponse<T>
{
    public T Data { get; set; }
}

public class DataRoot
{
    public string Id { get; set; }
    public string BibleId { get; set; }
    public string Number { get; set; }
    public string BookId { get; set; }
    public string Reference { get; set; } // chapter reference
    public string Copyright { get; set; }
    public int VerseCount { get; set; }
    public List<Content> Content { get; set; }
    //public string Content { get; set; }
}

public class Content
{
    public string Name { get; set; } // "para", "verse", "char"
    public string Text { get; set; } 
    public string Type { get; set; } // "tag", "text"
    public Dictionary<string, object> Attrs { get; set; } // style, verseId, sid, closed, verseOrgIds[]
    public List<Content> Items { get; set; }
}

// Todo
// - Refactor codebase to use new verse / chapter paradigm
// - Flatten items to store plain verse text and json content
// - Store chapter content as-is in database
// - Create models for chapter content, accounting for all possible styles / indentation
// - Method to extract chapter content into a class to display in React Native

public class ChaptersData
{
    public string Id { get; set; }
    public string Number { get; set; }
}

public class BibleApi
{
    readonly string _baseUrl = "https://rest.api.bible/v1";
    
    private readonly ILogger<BibleApi> _logger;
    private readonly IConfiguration _config;

    public BibleApi(
        IConfiguration config,
        ILogger<BibleApi> logger)
    {
        _config = config;
        _logger = logger;
    }
    
    // TODO: Refactor each step into its own function, then in background service job have it run through
    // each Bible and version to handle errors and logging
    // Have a dedicated table for logging Bible syncing
    // Have logging and error handling send me emails, and also on admin portal show alerts and logs
    /// <summary>
    /// Sync Postgres bible data with API.Bible data (at least every 30 days) as it says in their Terms & Conditions
    /// </summary>
    /// <param name="_logger"></param>
    /// <param name="_config"></param>
    /// <returns></returns>
    public async Task SyncDatabaseWithApiBible()
    {
        using HttpClient http = new();
        http.DefaultRequestHeaders.Add("api-key", _config["ApiBible:ApiKey"]);

        // For each translation
        for (int bibleIndex = 0; bibleIndex < BibleData.Bibles.Count; bibleIndex++)
        {
            // For each book in the Bible
            for (int bookIndex = 0; bookIndex < Books.AllBooks.Count; bookIndex++)
            {
                // Get all chapters in the book
                List<ChaptersData> chapters = new();
                string bibleId = BibleData.Bibles[bibleIndex].Id;
                string bookAbbr = Books.AllBooks[bookIndex].Abbreviation;
                try
                {
                    var chaptersResponse = await http.GetFromJsonAsync<ApiResponse<List<ChaptersData>>>
                        ($"{_baseUrl}/bibles/{bibleId}" 
                        + $"/books/{bookAbbr}"
                        + $"/chapters");
                    chapters = chaptersResponse?.Data;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed to get chapters:" + ex.Message);
                }

                int numChapters = 0;
                foreach (var chapter in chapters)
                {
                    int.TryParse(chapter.Number, out var chapterNum);
                    numChapters = chapterNum > 0
                        ? numChapters += 1
                        : numChapters;
                }

                // For each chapter in the book
                for (int chapterIndex = 0; chapterIndex < numChapters; chapterIndex++)
                {
                    // Get the chapter content
                    var response = await http.GetFromJsonAsync<ApiResponse<DataRoot>>(
                        $"{_baseUrl}/bibles/{bibleId}" +
                        $"/chapters/{chapters[chapterIndex].Id}" + 
                        "?content-type=json&include-titles=true&" +
                        "include-verse-numbers=true");

                    var _test = 0;
                    string jsonString = JsonSerializer.Serialize(response.Data);
                }
            }
        }
    }
}
