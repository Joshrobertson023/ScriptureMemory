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
    
    string book = "Genesis";
    int chapter = 1;
    int verseNumber = 1;
    string version = "kjv";
    
    /// <summary>
    /// Test adding a new verse into the database
    /// Gets verse content from API.Bible
    /// </summary>
    [Fact]
    public async Task InsertVerse_And_GetVerseById_Should_Insert_Verse_And_TranslationContent()
    {
        var verseService = _scope.ServiceProvider.GetRequiredService<VerseService>();
        var verseContext = _scope.ServiceProvider.GetRequiredService<VerseDataEfCore>();
        
        // Depends on API.Bible
        var newVerse = await verseService.AddNewVerse(book, chapter, verseNumber, version);
        
        Assert.NotNull(newVerse);
        Assert.NotNull(newVerse.TranslationContents.First());
        Assert.Equal("kjv", newVerse.TranslationContents.First().Version);
        Assert.NotEmpty(newVerse.TranslationContents.First().ContentUsx);
        Assert.NotEmpty(newVerse.TranslationContents.First().PlainText);
        Assert.NotNull(newVerse.TranslationContents.First().Embedding);
    }

    /// <summary>
    /// Test getting a verse by reference
    /// </summary>
    [Fact]
    public async Task GetVerseByReference_Should_Return_Verse()
    {
        var verseService = _scope.ServiceProvider.GetRequiredService<VerseService>();
        var verseContext = _scope.ServiceProvider.GetRequiredService<VerseDataEfCore>();
        
        var verse = await verseContext.GetVerse(book, chapter, verseNumber);
        
        Assert.NotNull(verse);
        Assert.NotNull(verse.TranslationContents.First());
        Assert.Equal("kjv", verse.TranslationContents.First().Version);
        Assert.NotEmpty(verse.TranslationContents.First().ContentUsx);
        Assert.NotEmpty(verse.TranslationContents.First().PlainText);
        Assert.NotNull(verse.TranslationContents.First().Embedding);
        Assert.NotEmpty(verse.TranslationContents.First().VerseId);
    }

    /// <summary>
    /// Test getting a verse by id
    /// </summary>
    [Fact]
    public async Task GetVerseById_Should_Return_Verse()
    {
        var verseService = _scope.ServiceProvider.GetRequiredService<VerseService>();
        var verseContext = _scope.ServiceProvider.GetRequiredService<VerseDataEfCore>();
        
        var verse = await verseContext.GetVerse("GEN.1.1");
        
        Assert.NotNull(verse);
        Assert.NotNull(verse.TranslationContents.First());
        Assert.Equal("kjv", verse.TranslationContents.First().Version);
        Assert.NotEmpty(verse.TranslationContents.First().ContentUsx);
        Assert.NotEmpty(verse.TranslationContents.First().PlainText);
        Assert.NotNull(verse.TranslationContents.First().Embedding);
        Assert.NotEmpty(verse.TranslationContents.First().VerseId);
    }

    /// <summary>
    /// Test adding a new Bible version content for a verse
    /// </summary>
    [Fact]
    public async Task InsertNewTranslation_Should_Insert_New_TranslationContent_For_Verse()
    {
        var verseService = _scope.ServiceProvider.GetRequiredService<VerseService>();
        var verseContext = _scope.ServiceProvider.GetRequiredService<VerseDataEfCore>();
    }
}