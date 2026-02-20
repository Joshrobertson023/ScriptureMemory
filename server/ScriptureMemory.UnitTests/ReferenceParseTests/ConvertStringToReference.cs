using VerseAppLibrary;
using Xunit;

namespace VerseApp.UnitTests.ReferenceParseTests;

public class ConvertStringToReference
{
    [Fact]
    public void ConvertStringToReference_ValidInput_ReturnsCorrectReference()
    {
        Reference reference = ReferenceParse.ConvertStringToReference("Psalms 119:12-14, 17");

        Assert.Equal("Psalms", reference.Book);
        Assert.Equal(119, reference.Chapter);
        Assert.Equal(new List<int> { 12, 13, 14, 17 }, reference.Verses);
    }

    [Fact]
    public void ConvertStringToReference_SingleVerse_ReturnsCorrectReference()
    {
        var reference = ReferenceParse.ConvertStringToReference("John 3:16");

        Assert.Equal("John", reference.Book);
        Assert.Equal(3, reference.Chapter);
        Assert.Equal(new List<int> { 16 }, reference.Verses);
    }

    [Fact]
    public void ConvertStringToReference_NumberedBook_ReturnsCorrectReference()
    {
        var reference = ReferenceParse.ConvertStringToReference("1 John 4:8");

        Assert.Equal("1 John", reference.Book);
        Assert.Equal(4, reference.Chapter);
        Assert.Equal(new List<int> { 8 }, reference.Verses);
    }

    [Fact]
    public void ConvertStringToReference_InvalidBook_ThrowsException()
    {
        Assert.Throws<Exception>(() =>
            ReferenceParse.ConvertStringToReference("FakeBook 1:1"));
    }
}
