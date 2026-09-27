/*using ScriptureMemory.Server.Data.Models;
using ScriptureMemory.Server.Tools;

namespace ScriptureMemory.UnitTests.BookTests;

public class EnsureValidBookTests
{
    [Fact]
    public void EnsureValidBook_TryGetBookReturnsValidBook()
    {
        Book? book = Books.TryGetBook("Genesis");
        Assert.Equal("Genesis", book?.DisplayName);
        Assert.Equal("gen", book?.Abbreviation);
    }
    
    [Fact]
    public void EnsureValidBook_TryGetBookReturnsValidBookFromAbbreviation()
    {
        Book? book = Books.TryGetBook("gen");
        Assert.Equal("Genesis", book?.DisplayName);
        Assert.Equal("gen", book?.Abbreviation);
    }
    
    [Fact]
    public void EnsureValidBook_TryGetBookReturnsValidBookFromFuzzyMatch()
    {
        Book? book = Books.TryGetBook("geneses");
        Assert.Equal("Genesis", book?.DisplayName);
        Assert.Equal("gen", book?.Abbreviation);
    }

    [Fact]
    public void EnsureValidBook_InvalidTryGetBookReturnsNull()
    {
        Book? book = Books.TryGetBook("random_book");
        Assert.Null(book);
    }
}*/