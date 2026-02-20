using ScriptureMemoryLibrary;
using Xunit;
namespace VerseApp.UnitTests.ReferenceParseTests;

public class GetBookTests
{
    [Fact]
    public void GetBook_StandardBookDoubleDigitChapter_ReturnsBookName()
    {
        var result = ReferenceParse.GetBook("Psalms 119:2-21, 24");
        Assert.Equal("Psalms", result);
    }
    [Fact]
    public void GetBook_NumberedBookDoubleDigitVerses_ReturnsFullBookName()
    {
        var result = ReferenceParse.GetBook("1 John 4:18-19, 20");
        Assert.Equal("1 John", result);
    }
    [Fact]
    public void GetBook_SingleWordBook_ReturnsBookName()
    {
        var result = ReferenceParse.GetBook("John 3:16");
        Assert.Equal("John", result);
    }
    [Fact]
    public void GetBook_InvalidBook_ThrowsException()
    {
        Assert.Throws<Exception>(() => ReferenceParse.GetBook("FakeBook 1:1"));
    }
}
