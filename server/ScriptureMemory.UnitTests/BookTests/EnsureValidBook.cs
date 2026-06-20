using ScriptureMemory.Server.Tools;

namespace ScriptureMemory.UnitTests.BookTests;

public class EnsureValidBook
{
    [Fact]
    public void EnsureValidBook_TryGetBookReturnsValidBook()
    {
        var results = Books.TryGetBook("Genesis", out var book);
        Assert.True(results);
        Assert.Equal("Genesis", book?.DisplayName);
        Assert.Equal("gen", book?.Abbreviation);
    }
    
    [Fact]
    public void EnsureValidBook_TryGetBookReturnsValidBookFromAbbreviation()
    {
        var results = Books.TryGetBook("gen", out var book);
        Assert.True(results);
        Assert.Equal("Genesis", book?.DisplayName);
        Assert.Equal("gen", book?.Abbreviation);
    }
    
    [Fact]
    public void EnsureValidBook_TryGetBookReturnsValidBookFromFuzzyMatch()
    {
        var results = Books.TryGetBook("geneses", out var book);
        Assert.True(results);
        Assert.Equal("Genesis", book?.DisplayName);
        Assert.Equal("gen", book?.Abbreviation);
    }

    [Fact]
    public void EnsureValidBook_InvalidTryGetBookReturnsNull()
    {
        var results = Books.TryGetBook("random_book", out var book);
        Assert.False(results);
        Assert.Null(book);
    }
}