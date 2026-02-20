using VerseAppLibrary;
using Xunit;
namespace VerseApp.UnitTests.ReferenceParseTests;

public class GetIndividualVersesTests
{
    [Fact]
    public void GetIndividualVerses_DoubleDigitRangeWithSeparateVerse_ReturnsAllVerses()
    {
        var result = ReferenceParse.GetIndividualVerses("Psalms 119:2-21, 24");
        Assert.Equal(new List<int> { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 24 }, result);
    }
    [Fact]
    public void GetIndividualVerses_NumberedBookDoubleDigitRangeWithSeparateVerse_ReturnsAllVerses()
    {
        var result = ReferenceParse.GetIndividualVerses("1 John 4:18-19, 20");
        Assert.Equal(new List<int> { 18, 19, 20 }, result);
    }
    [Fact]
    public void GetIndividualVerses_SingleVerse_ReturnsSingleItem()
    {
        var result = ReferenceParse.GetIndividualVerses("John 3:16");
        Assert.Equal(new List<int> { 16 }, result);
    }
    [Fact]
    public void GetIndividualVerses_DoubleDigitCommaSeparatedVerses_ReturnsAllVerses()
    {
        var result = ReferenceParse.GetIndividualVerses("Psalms 119:18,21,24");
        Assert.Equal(new List<int> { 18, 21, 24 }, result);
    }
}