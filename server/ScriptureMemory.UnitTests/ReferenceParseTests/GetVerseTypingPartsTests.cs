using VerseAppLibrary;
using Xunit;
namespace VerseApp.UnitTests.ReferenceParseTests;

public class GetVerseTypingPartsTests
{
    [Fact]
    public void GetVerseTypingParts_DoubleDigitRange_ReturnsCorrectParts()
    {
        var result = ReferenceParse.GetVerseTypingParts("Psalms 119:2-21");
        Assert.Equal(new List<string> { "Psalms", "119", "2", "21" }, result);
    }
    [Fact]
    public void GetVerseTypingParts_DoubleDigitRangeWithSeparateVerse_ReturnsCorrectParts()
    {
        var result = ReferenceParse.GetVerseTypingParts("Psalms 119:2-21, 24");
        Assert.Equal(new List<string> { "Psalms", "119", "2", "21", "24" }, result);
    }
    [Fact]
    public void GetVerseTypingParts_DoubleDigitNumberedBook_ReturnsCorrectParts()
    {
        var result = ReferenceParse.GetVerseTypingParts("1 John 4:18-19, 20");
        Assert.Equal(new List<string> { "1 John", "4", "18", "19", "20" }, result);
    }
    [Fact]
    public void GetVerseTypingParts_SingleVerse_ReturnsCorrectParts()
    {
        var result = ReferenceParse.GetVerseTypingParts("John 3:16");
        Assert.Equal(new List<string> { "John", "3", "16" }, result);
    }
    [Fact]
    public void GetVerseTypingParts_MultipleCommaSeparatedDoubleDigitVerses_ReturnsCorrectParts()
    {
        var result = ReferenceParse.GetVerseTypingParts("Psalms 119:18,21,24");
        Assert.Equal(new List<string> { "Psalms", "119", "18", "21", "24" }, result);
    }
}