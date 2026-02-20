using ScriptureMemoryLibrary;
using Xunit;
namespace VerseApp.UnitTests.ReferenceParseTests;

public class ConvertToReadableReferenceTests
{
    [Fact]
    public void ConvertToReadableReference_DoubleDigitConsecutiveRangeWithSeparateVerse_ReturnsCorrectString()
    {
        var result = ReferenceParse.ConvertToReadableReference("Psalms", 119, new List<int> { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 24 });
        Assert.Equal("Psalms 119:2-21, 24", result);
    }
    [Fact]
    public void ConvertToReadableReference_NumberedBookDoubleDigitRange_ReturnsCorrectString()
    {
        var result = ReferenceParse.ConvertToReadableReference("1 John", 4, new List<int> { 18, 19, 20 });
        Assert.Equal("1 John 4:18-20", result);
    }
    [Fact]
    public void ConvertToReadableReference_NumberedBookRangeWithSeparateVerse_ReturnsCorrectString()
    {
        var result = ReferenceParse.ConvertToReadableReference("1 John", 4, new List<int> { 18, 19, 20 });
        Assert.Equal("1 John 4:18-19, 20", result);
    }
    [Fact]
    public void ConvertToReadableReference_SingleVerse_ReturnsCorrectString()
    {
        var result = ReferenceParse.ConvertToReadableReference("John", 3, new List<int> { 16 });
        Assert.Equal("John 3:16", result);
    }
    [Fact]
    public void ConvertToReadableReference_UnsortedDoubleDigitInput_ReturnsSortedCorrectString()
    {
        var result = ReferenceParse.ConvertToReadableReference("Psalms", 119, new List<int> { 24, 21, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 });
        Assert.Equal("Psalms 119:2-21, 24", result);
    }
}
