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

public class ChaptersData
{
    public string Id { get; set; }
    public string Number { get; set; }
}

public class BibleApi
{
    /// <summary>
    /// Sync Postgres bible data with API.Bible data (at least every 30 days) as it says in their Terms
    /// </summary>
    /// <param name="_logger"></param>
    /// <param name="_config"></param>
    /// <returns></returns>
    public async Task SyncDatabaseWithApiBible(ILogger _logger, IConfiguration _config)
    {
        using HttpClient http = new();
        http.DefaultRequestHeaders.Add("api-key", _config["ApiBible:ApiKey"]);
        string baseUrl = "https://rest.api.bible/v1";

        // For each translation
        for (int bibleIndex = 0; bibleIndex < BibleData.Bibles.Count; bibleIndex++)
        {
            // For each book in the Bible
            for (int bookIndex = 0; bookIndex < Books.BookNames.All.Count; bookIndex++)
            {
                // Get all chapters in the book
                List<ChaptersData> chapters = new();
                string bibleId = BibleData.Bibles[bibleIndex].Id;
                string bookAbbr = Books.GetAbbreviation(Books.BookNames.All[bookIndex]);
                try
                {
                    var chaptersResponse = await http.GetFromJsonAsync<ApiResponse<List<ChaptersData>>>
                        ($"{baseUrl}/bibles/{bibleId}" 
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
                        $"{baseUrl}/bibles/{bibleId}" +
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
