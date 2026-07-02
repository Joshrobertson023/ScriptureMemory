using DataAccess.Models;
using Microsoft.Extensions.DependencyInjection;
using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.DataAccess.Models;
using ScriptureMemory.Server.Tools;
using VerseAppNew.Server.Services;

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
        var verseService = _scope.ServiceProvider.GetRequiredService<VerseService>();
        
        await verseService.AddNewVerse("Genesis", 1, 1, "kjv");
        
        // Left off here. Add data access for getting verse by reference and version(s),
        // then complete this test.
        var newVerse = _dbContext.Verses.Where(v => v.Reference.Book == "Genesis" && )
        
        Assert.NotNull(newVerse);
        Assert.NotNull(newVerse.TranslationContents.First());
        Assert.Equal("kjv", newVerse.TranslationContents.First().Version);
        Assert.NotEmpty(newVerse.TranslationContents.First().ContentUsx);
        Assert.NotEmpty(newVerse.TranslationContents.First().PlainText);
        Assert.NotNull(newVerse.TranslationContents.First().Embedding);
        Assert.NotEmpty(newVerse.TranslationContents.First().VerseId);
    }

    [Fact]
    public async Task InsertNewTranslation_Should_Insert_New_TranslationContent_For_Verse()
    {
        
    }
}