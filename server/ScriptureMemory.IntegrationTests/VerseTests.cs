using DataAccess.Models;
using Microsoft.Extensions.DependencyInjection;
using ScriptureMemory.Server.Data.DataAccess.Bible;

namespace ScriptureMemory.IntegrationTests;

public class VerseTests : BaseIntegrationTest
{
    public VerseTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }
    
    // [Fact]
    // public async Task InsertVerse_Should_Insert_Verse_And_TranslationContent()
    // {
    //     var verseContext = _scope.ServiceProvider.GetRequiredService<VerseDataEfCore>();
    //
    //     Verse newVerse = new Verse()
    // }

    [Fact]
    public async Task InsertVerse_Should_Create_Valid_Verse_Id()
    {
        
    }
}