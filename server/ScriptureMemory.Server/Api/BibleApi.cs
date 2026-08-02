using ScriptureMemory.Server.Data.Dtos;
using ScriptureMemory.Server.Data.Models;
using ScriptureMemory.Server.Tools.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ScriptureMemory.Server.Tools;

// Todo
// - Refactor codebase to use new verse / chapter paradigm
// - Flatten items to store plain verse text and json content
// - Store chapter content as-is in database
// - Create models for chapter content, accounting for all possible styles / indentation
// - Method to extract chapter content into a class to display in React Native

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

    public async Task<List<Bible>> GetAuthorizedBibles()
    {
        using HttpClient http = new();
        http.DefaultRequestHeaders.Add("api-key", _config["ApiBible:ApiKey"]);

        var response = await http.GetFromJsonAsync<GetBiblesResponse>(
            $"{_baseUrl}/bibles?language=eng");

        return response?.Data.ToList()
               ?? new List<Bible>();
    }

    public async Task<ApiResponse<ChapterData>> GetFullChapter(Bible bible, Reference chapterReference)
    {
        using HttpClient http = new();
        http.DefaultRequestHeaders.Add("api-key", _config["ApiBible:ApiKey"]);
        
        var response = await http.GetStringAsync(
            $"{_baseUrl}/bibles/{bible.Id}" +
            $"/chapters/{chapterReference.ChapterId}" + 
            "?content-type=html&" +
            "include-titles=true&" +
            "include-verse-numbers=true&" +
            "include-verse-spans=true");

        return JsonSerializer.Deserialize<ApiResponse<ChapterData>>(response)
            ?? throw new InvalidOperationException("response was null deserializing");
    }

    public async Task<(string, string)> GetVerseUsxAndPlaintext(string bibleId, string verseId)
    {
        using HttpClient http = new();
        http.DefaultRequestHeaders.Clear();
        http.DefaultRequestHeaders.Add("api-key", _config["ApiBible:ApiKey"]);
        
        Task<string> getUsx = http.GetStringAsync(
            $"{_baseUrl}/bibles/{bibleId}" +
            $"/verses/{verseId}" + 
            "?content-type=html&" +
            "include-titles=true&" +
            "include-verse-numbers=true&" +
            "include-verse-spans=true");
        
        Task<string> getPlaintext = http.GetStringAsync(
            $"{_baseUrl}/bibles/{bibleId}" +
            $"/verses/{verseId}" + 
            "?content-type=text&" +
            "include-titles=false&" +
            "include-verse-numbers=true&" +
            "include-verse-spans=false");

        await Task.WhenAll(getUsx, getPlaintext);

        return (await getUsx, await getPlaintext);
    }

    public async Task<string> GetVerseUsx(string bibleId, string verseId)
    {
        using HttpClient http = new();
        http.DefaultRequestHeaders.Clear();
        http.DefaultRequestHeaders.Add("api-key", _config["ApiBible:ApiKey"]);
        
        var response = await http.GetStringAsync(
            $"{_baseUrl}/bibles/{bibleId}" +
            // $"/chapters/{chapterReference.ChapterId}" + 
            $"/verses/{verseId}" + 
            "?content-type=html&" +
            "include-titles=true&" +
            "include-verse-numbers=true&" +
            "include-verse-spans=true");

        return response;
    }

    public async Task<int> GetChaptersInBook(Bible bible, Reference bookReference)
    {
        using HttpClient http = new();
        http.DefaultRequestHeaders.Add("api-key", _config["ApiBible:ApiKey"]);
        
        var response = await http.GetFromJsonAsync<ApiResponse<List<ChaptersCountData>>>
            ($"{_baseUrl}/bibles/{bible.Id}" 
            + $"/books/{bookReference.Book.Abbreviation}"
            + $"/chapters");
        
        var chapters = response?.Data;

        return chapters.Count;
    }

    public async Task<int> GetVersesInChapter(Bible bible, Reference chapterReference)
    {
        using HttpClient http = new();
        http.DefaultRequestHeaders.Add("api-key", _config["ApiBible:ApiKey"]);
        
        var response = await http.GetFromJsonAsync<ApiResponse<List<ChaptersCountData>>>
        ($"{_baseUrl}/bibles/{bible.Id}" 
         + $"/chapters/{chapterReference.ChapterId}"
         + $"/verses");
        
        var verses = response?.Data;

        return verses.Count;
    }
}
