using ScriptureMemoryLibrary;
using Xunit;
namespace VerseApp.UnitTests.ReferenceParseTests;
public class ConvertToReferenceStringMultipleVersesTests
{
    [Fact]
    public void ConvertToReferenceString_DoubleDigitVerseRange_ReturnsCommaSeparatedString()
    {
        var result = ReferenceParse.ConvertToReferenceString("Psalms", 119, new List<int> { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 24 });
        Assert.Equal("Psalms 119:2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,24", result);
    }
    [Fact]
    public void ConvertToReferenceString_NumberedBookDoubleDigitVerses_ReturnsCorrectString()
    {
        var result = ReferenceParse.ConvertToReferenceString("1 John", 4, new List<int> { 18, 19, 20 });
        Assert.Equal("1 John 4:18,19,20", result);
    }
    [Fact]
    public void ConvertToReferenceString_SingleVerseInList_ReturnsCorrectString()
    {
        var result = ReferenceParse.ConvertToReferenceString("John", 3, new List<int> { 16 });
        Assert.Equal("John 3:16", result);
    }
    [Fact]
    public void ConvertToReferenceString_TwoDoubleDigitVerses_ReturnsCorrectString()
    {
        var result = ReferenceParse.ConvertToReferenceString("Psalms", 119, new List<int> { 21, 24 });
        Assert.Equal("Psalms 119:21,24", result);
    }
}
