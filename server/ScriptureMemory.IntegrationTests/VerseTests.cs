using DataAccess.Models;
using DataAccess.Requests;
using ScriptureMemoryLibrary;
using System.Net.Http.Json;
using static ScriptureMemoryLibrary.Enums;

namespace ScriptureMemory.IntegrationTests;

public class VerseTests : BaseIntegrationTest
{
    public VerseTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task VerseTest_InsertGetUpdate()
    {
        // -- Create verse ----------------------
        var newVerse = new Verse
        {
            Reference = new DataAccess.Models.Reference("John 3:16"),
            Text = "For God so loved the world that he gave his one and only Son, that whoever believes in him shall not perish but have eternal life.",
            UsersSavedCount = 0,
            UsersMemorizedCount = 0,
        };

        // Insert via direct context (if no endpoint exists)
        await verseContext.InsertVerse(newVerse);

        // Get verse via endpoint
        var getResponse = await Api.PostAsJsonAsync("/verses", newVerse.Reference.ReadableReference);
        getResponse.EnsureSuccessStatusCode();
        var verse = await getResponse.Content.ReadFromJsonAsync<Verse>();
        Assert.NotNull(verse);
        Assert.Equal(newVerse.Reference.ReadableReference, verse.Reference.ReadableReference);

        // -- Update verse ----------------------
        var updateResponse = await Api.PutAsync($"/verses/saved/{newVerse.Reference.ReadableReference}", null);
        updateResponse.EnsureSuccessStatusCode();

        var getAfterUpdate = await Api.PostAsJsonAsync("/verses", newVerse.Reference.ReadableReference);
        getAfterUpdate.EnsureSuccessStatusCode();
        var updatedVerse = await getAfterUpdate.Content.ReadFromJsonAsync<Verse>();
        Assert.NotNull(updatedVerse);
        Assert.Equal(verse.UsersSavedCount + 1, updatedVerse.UsersSavedCount);
    }

    [Fact]
    public async Task VerseTest_GetFullChapterVerses()
    {
        // Insert a few verses for the chapter (if not present)
        var john316 = new Verse
        {
            Reference = new DataAccess.Models.Reference("John 3:16"),
            Text = "For God so loved the world that he gave his one and only Son, that whoever believes in him shall not perish but have eternal life.",
            UsersSavedCount = 0,
            UsersMemorizedCount = 0,
        };
        var john317 = new Verse
        {
            Reference = new DataAccess.Models.Reference("John 3:17"),
            Text = "For God did not send his Son into the world to condemn the world, but to save the world through him.",
            UsersSavedCount = 0,
            UsersMemorizedCount = 0,
        };
        var john318 = new Verse
        {
            Reference = new DataAccess.Models.Reference("John 3:18"),
            Text = "Whoever believes in him is not condemned, but whoever does not believe stands condemned already because they have not believed in the name of God's one and only Son.",
            UsersSavedCount = 0,
            UsersMemorizedCount = 0,
        };
        var john319 = new Verse
        {
            Reference = new DataAccess.Models.Reference("John 3:19"),
            Text = "This is the verdict: Light has come into the world, but people loved darkness instead of light because their deeds were evil.",
            UsersSavedCount = 0,
            UsersMemorizedCount = 0,
        };

        List<Verse> newChapter = new() { john316, john317, john318, john319 };
        foreach (var v in newChapter)
            await verseContext.InsertVerse(v);

        // Get chapter via endpoint
        var chapterRequest = new { Book = "John", Chapter = 3 };
        var response = await Api.PostAsJsonAsync("/verses/chapter", chapterRequest);
        response.EnsureSuccessStatusCode();
        var chapterVerses = await response.Content.ReadFromJsonAsync<List<Verse>>();
        Assert.NotNull(chapterVerses);
        Assert.True(chapterVerses.Count >= 4); // At least the ones we inserted
        var firstFirst = chapterVerses.FirstOrDefault(v => v.Reference.ReadableReference == "John 3:16");
        Assert.NotNull(firstFirst);
        Assert.Equal(john316.Text, firstFirst.Text);
    }

    // No endpoint for GetAllVersesFromReferenceList, so this test is skipped or would need a new endpoint.
}
