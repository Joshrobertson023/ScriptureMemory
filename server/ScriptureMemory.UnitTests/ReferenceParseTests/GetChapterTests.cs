using VerseAppLibrary;
using Xunit;
namespace VerseApp.UnitTests.ReferenceParseTests;

public class GetChapterTests
{
    [Fact]
    public void GetChapter_TripleDigitChapterDoubleDigitVerseRange_ReturnsChapterNumber()
    {
        var result = ReferenceParse.GetChapter("Psalms 119:2-21, 24");
        Assert.Equal(119, result);
    }
    [Fact]
    public void GetChapter_NumberedBookDoubleDigitVerses_ReturnsChapterNumber()
    {
        var result = ReferenceParse.GetChapter("1 John 4:18-19, 20");
        Assert.Equal(4, result);
    }
    [Fact]
    public void GetChapter_SingleDigitChapter_ReturnsChapterNumber()
    {
        var result = ReferenceParse.GetChapter("John 3:16");
        Assert.Equal(3, result);
    }
    [Fact]
    public void GetChapter_InvalidReference_ThrowsException()
    {
        Assert.Throws<Exception>(() => ReferenceParse.GetChapter("FakeBook 1:1"));
    }
}