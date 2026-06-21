using DataAccess.Models;
using ScriptureMemory.Server.Data.Models;
using ScriptureMemory.Server.Tools;

namespace ScriptureMemory.UnitTests.VerseTests;

public class VerseTests
{
    [Fact]
    public async Task InsertVerse_Should_Create_Valid_Verse_Id()
    {
        Verse newVerse = new("Genesis", 1, 1);

        Assert.Equal("GEN.1.1", newVerse.Id);
    }

    [Fact]
    public async Task InsertVerse_InvalidBookThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new Verse("FakeBook", 1, 1));
    }
}