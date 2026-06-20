using DataAccess.Models;
using ScriptureMemory.Server.Data.Models;
using ScriptureMemory.Server.Tools;

namespace ScriptureMemory.UnitTests.VerseTests;

public class CreateId
{
    [Fact]
    public async Task InsertVerse_Should_Create_Valid_Verse_Id()
    {
        Verse newVerse = new(Books.GetBook("Genesis"))
    }
}