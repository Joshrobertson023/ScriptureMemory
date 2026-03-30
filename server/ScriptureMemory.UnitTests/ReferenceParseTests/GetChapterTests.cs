using ScriptureMemory.Server.Tools;
using ScriptureMemory.Server.Tools;
using Xunit;
namespace VerseApp.UnitTests.ReferenceParseTests;

public class GetChapterTests
{
    [Fact]
    public void GetChapter_TripleDigitChapterDoubleDigitVerseRange_ReturnsChapterNumber()
    {
        var result = ReferenceParser.GetChapter("Psalms 119:2-21, 24");
        Assert.Equal(119, result);
    }
    [Fact]
    public void GetChapter_NumberedBookDoubleDigitVerses_ReturnsChapterNumber()
    {
        var result = ReferenceParser.GetChapter("1 John 4:18-19, 20");
        Assert.Equal(4, result);
    }
    [Fact]
    public void GetChapter_SingleDigitChapter_ReturnsChapterNumber()
    {
        var result = ReferenceParser.GetChapter("John 3:16");
        Assert.Equal(3, result);
    }
    [Fact]
    public void GetChapter_InvalidReference_ThrowsException()
    {
        Assert.Throws<Exception>(() => ReferenceParser.GetChapter("FakeBook 1:1"));
    }
}
