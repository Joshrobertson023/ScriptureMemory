using System.Net.Http.Headers;
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
    public int Number { get; set; }
    public string BookId { get; set; }
    public string Reference { get; set; } // chapter reference
    public string Copyright { get; set; }
    public int VerseCount { get; set; }
    public List<Content> Content { get; set; }

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
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config["ApiBible:ApiKey"]);
        string baseUrl = "https://rest.api.bible";

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
                int numChapters = chapters.Sum(c => Convert.ToInt32(c.Number));

                // For each chapter in the book
                for (int chapterIndex = 0; chapterIndex < numChapters; chapterIndex++)
                {
                    // Get the chapter content
                    var response = http.GetFromJsonAsync<ApiResponse<DataRoot>>(
                        $"{baseUrl}/bibles/{bibleId}" +
                        $"/books/{bookAbbr}" +
                        $"/chapters/{bookAbbr + "." + (chapterIndex + 1).ToString()}");


                }
            }
        }
    }
}
