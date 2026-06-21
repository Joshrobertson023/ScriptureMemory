using DataAccess.Models;
using Microsoft.Extensions.DependencyInjection;
using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.Tools;

namespace ScriptureMemory.IntegrationTests;

public class VerseTests : BaseIntegrationTest
{
    public VerseTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }
    
    [Fact]
    public async Task GetVerse_Should_Insert_Verse_And_TranslationContent()
    {
        var verseContext = _scope.ServiceProvider.GetRequiredService<VerseDataEfCore>();
        var bibleApi = _scope.ServiceProvider.GetRequiredService<BibleApi>();

        Verse newVerse = new("Genesis", 1, 1);
        
        newVerse.TranslationContents.Add(bibleApi.GetVerseUsx());
    }
}