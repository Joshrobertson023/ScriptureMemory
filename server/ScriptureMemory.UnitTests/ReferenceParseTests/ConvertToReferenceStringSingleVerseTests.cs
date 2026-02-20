using VerseAppLibrary;
using Xunit;
namespace VerseApp.UnitTests.ReferenceParseTests;

public class ConvertToReferenceStringSingleVerseTests
{
    [Fact]
    public void ConvertToReferenceString_DoubleDigitVerse_ReturnsCorrectString()
    {
        var result = ReferenceParse.ConvertToReferenceString("Psalms", 119, 21);
        Assert.Equal("Psalms 119:21", result);
    }
    [Fact]
    public void ConvertToReferenceString_NumberedBookDoubleDigitVerse_ReturnsCorrectString()
    {
        var result = ReferenceParse.ConvertToReferenceString("1 John", 4, 18);
        Assert.Equal("1 John 4:18", result);
    }
    [Fact]
    public void ConvertToReferenceString_SingleVerse_ReturnsCorrectString()
    {
        var result = ReferenceParse.ConvertToReferenceString("John", 3, 16);
        Assert.Equal("John 3:16", result);
    }
}