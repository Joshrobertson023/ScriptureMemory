using DataAccess.Models;
using DataAccess.Requests;
using ScriptureMemoryLibrary;
using static ScriptureMemoryLibrary.Enums;

namespace ScriptureMemory.IntegrationTests;

public class VerseTests : BaseIntegrationTest
{
    public VerseTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task VerseTest_InsertGetUpdate()
    {
        // -- Create verse ----------------------

        Verse newVerse = new Verse
        {
            Reference = new Reference("John 3:16"),
            Text = "For God so loved the world that he gave his one and only Son, that whoever believes in him shall not perish but have eternal life.",
            UsersSavedCount = 0,
            UsersMemorizedCount = 0,
        };

        await verseContext.InsertVerse(newVerse);
        var verse = await verseContext.GetVerse(newVerse.Reference.ReadableReference);

        Assert.NotNull(verse);
        Assert.Equal(newVerse.Reference, verse.Reference);


        // -- Update verse ----------------------

        await verseContext.UpdateUsersMemorizedVerse(verse.Reference.ReadableReference);
        verse = await verseContext.GetVerse(newVerse.Reference.ReadableReference);
        Assert.NotNull(verse);
        Assert.Equal(1, verse.UsersMemorizedCount);

        await verseContext.UpdateUsersSavedVerse(verse.Reference.ReadableReference);
        verse = await verseContext.GetVerse(newVerse.Reference.ReadableReference);
        Assert.NotNull(verse);
        Assert.Equal(1, verse.UsersSavedCount);
    }

    [Fact]
    public async Task VerseTest_GetFullChapterVerses()
    {
        // -- Test getting full chapter verses -----------------------

        var john316 = new Verse
        {
            Reference = new Reference("John 3:16"),
            Text = "For God so loved the world that he gave his one and only Son, that whoever believes in him shall not perish but have eternal life.",
            UsersSavedCount = 0,
            UsersMemorizedCount = 0,
        };
        var john317 = new Verse
        {
            Reference = new Reference("John 3:17"),
            Text = "For God did not send his Son into the world to condemn the world, but to save the world through him.",
            UsersSavedCount = 0,
            UsersMemorizedCount = 0,
        };
        var john318 = new Verse
        {
            Reference = new Reference("John 3:18"),
            Text = "Whoever believes in him is not condemned, but whoever does not believe stands condemned already because they have not believed in the name of God's one and only Son.",
            UsersSavedCount = 0,
            UsersMemorizedCount = 0,
        };
        var john319 = new Verse
        {
            Reference = new Reference("John 3:19"),
            Text = "This is the verdict: Light has come into the world, but people loved darkness instead of light because their deeds were evil.",
            UsersSavedCount = 0,
            UsersMemorizedCount = 0,
        };

        List<Verse> newChapter = new()
        {
            john316,
            john317,
            john318,
            john319
        };

        foreach (var chapterVerse in newChapter)
        {
            await verseContext.InsertVerse(chapterVerse);
        }

        var chapterVerses = await verseContext.GetChapterVerses("John", 3);
        Assert.NotNull(chapterVerses);
        Assert.Equal(4, chapterVerses.Count);
        var firstFirst = chapterVerses.FirstOrDefault(v => v.Reference.ReadableReference == "John 3:16");
        Assert.NotNull(firstFirst);
        Assert.Equal(john316.Text, firstFirst.Text);
    }

    [Fact]
    public async Task VerseTest_GetAllVersesFromReferenceList()
    {
        List<string> references = new();
        var john316 = new Verse
        {
            Reference = new Reference("John 3:16"),
            Text = "For God so loved the world that he gave his one and only Son, that whoever believes in him shall not perish but have eternal life.",
            UsersSavedCount = 0,
            UsersMemorizedCount = 0,
        };
        var john317 = new Verse
        {
            Reference = new Reference("John 3:17"),
            Text = "For God did not send his Son into the world to condemn the world, but to save the world through him.",
            UsersSavedCount = 0,
            UsersMemorizedCount = 0,
        };
        var john318 = new Verse
        {
            Reference = new Reference("John 3:18"),
            Text = "Whoever believes in him is not condemned, but whoever does not believe stands condemned already because they have not believed in the name of God's one and only Son.",
            UsersSavedCount = 0,
            UsersMemorizedCount = 0,
        };
        var john319 = new Verse
        {
            Reference = new Reference("John 3:19"),
            Text = "This is the verdict: Light has come into the world, but people loved darkness instead of light because their deeds were evil.",
            UsersSavedCount = 0,
            UsersMemorizedCount = 0,
        };

        List<Verse> newChapter = new()
        {
            john316,
            john317,
            john318,
            john319
        };

        foreach (var chapterVerse in newChapter)
        {
            references.Add(chapterVerse.Reference.ReadableReference);
        }

        foreach (var chapterVerse in newChapter)
        {
            await verseContext.InsertVerse(chapterVerse);
        }

        var versesFromReferences = await verseContext.GetAllVersesFromReferenceList(references);
        Assert.NotNull(versesFromReferences);
        Assert.Equal(4, versesFromReferences.Count);
        var firstVerse = versesFromReferences.FirstOrDefault(v => v.Reference.ReadableReference == "John 3:16");
        Assert.NotNull(firstVerse);
        Assert.Equal(john316.Text, firstVerse.Text);
    }
}
