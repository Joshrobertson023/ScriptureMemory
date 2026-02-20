using VerseAppLibrary;
using Xunit;
namespace VerseApp.UnitTests.ReferenceParseTests;

public class GetVersesHalfOfReferenceTests
{
    [Fact]
    public void GetVersesHalfOfReference_DoubleDigitRangeWithSeparateVerse_ReturnsVersesPortion()
    {
        var result = ReferenceParse.GetVersesHalfOfReference("Psalms 119:2-21, 24");
        Assert.Equal("2-21, 24", result);
    }
    [Fact]
    public void GetVersesHalfOfReference_NumberedBookDoubleDigitRange_ReturnsVersesPortion()
    {
        var result = ReferenceParse.GetVersesHalfOfReference("1 John 4:18-19, 20");
        Assert.Equal("18-19, 20", result);
    }
    [Fact]
    public void GetVersesHalfOfReference_SingleDoubleDigitVerse_ReturnsVersesPortion()
    {
        var result = ReferenceParse.GetVersesHalfOfReference("John 3:16");
        Assert.Equal("16", result);
    }
    [Fact]
    public void GetVersesHalfOfReference_NoColon_ReturnsEmptyString()
    {
        var result = ReferenceParse.GetVersesHalfOfReference("Psalms 119");
        Assert.Equal(string.Empty, result);
    }
    [Fact]
    public void GetVersesHalfOfReference_DoubleDigitCommaSeparatedVerses_ReturnsVersesPortion()
    {
        var result = ReferenceParse.GetVersesHalfOfReference("Psalms 119:18,21,24");
        Assert.Equal("18,21,24", result);
    }
}