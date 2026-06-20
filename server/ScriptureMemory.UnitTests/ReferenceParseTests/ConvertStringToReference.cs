using DataAccess.Models;
using ScriptureMemory.Server.Tools;
using Xunit;

namespace VerseApp.UnitTests.ReferenceParseTests;

public class ConvertStringToReference
{
    [Fact]
    public void ConvertStringToReference_ValidInput_ReturnsCorrectReference()
    {
        Reference reference = ReferenceParser.Parse("Psalms 119:12-14, 17");

        Assert.Equal("Psalms", reference.Book.DisplayName);
        Assert.Equal(119, reference.Chapter);
        Assert.Equal(new List<int> { 12, 13, 14, 17 }, reference.VerseNumbers);
    }

    [Fact]
    public void ConvertStringToReference_SingleVerse_ReturnsCorrectReference()
    {
        var reference = ReferenceParser.Parse("John 3:16");

        Assert.Equal("John", reference.Book.DisplayName);
        Assert.Equal(3, reference.Chapter);
        Assert.Equal(new List<int> { 16 }, reference.VerseNumbers);
    }

    [Fact]
    public void ConvertStringToReference_NumberedBook_ReturnsCorrectReference()
    {
        var reference = ReferenceParser.Parse("1 John 4:8");

        Assert.Equal("1 John", reference.Book.DisplayName);
        Assert.Equal(4, reference.Chapter);
        Assert.Equal(new List<int> { 8 }, reference.VerseNumbers);
    }

    [Fact]
    public void ConvertStringToReference_InvalidBook_ReferenceBookDisplayNameIsInvalid()
    {
        var reference = ReferenceParser.Parse("random_book 1:2-3");
        
        Assert.Null(reference);
    }
}
