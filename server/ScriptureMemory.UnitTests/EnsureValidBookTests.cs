using ScriptureMemory.Server.Data.Models;
using ScriptureMemory.Server.Tools;

namespace ScriptureMemory.UnitTests.BookTests;

public class EnsureValidBookTests
{
    [Fact]
    public void EnsureValidBook_TryGetBookReturnsValidBook()
    {
        Book? book = Books.GetBook("Genesis");
        Assert.Equal("Genesis", book?.DisplayName);
        Assert.Equal("gen", book?.Abbreviation);
    }
    
    [Fact]
    public void EnsureValidBook_TryGetBookReturnsValidBookFromAbbreviation()
    {
        Book? book = Books.GetBook("gen");
        Assert.Equal("Genesis", book?.DisplayName);
        Assert.Equal("gen", book?.Abbreviation);
    }
    
    [Fact]
    public void EnsureValidBook_TryGetBookReturnsValidBookFromFuzzyMatch()
    {
        Book? book = Books.GetBook("geneses");
        Assert.Equal("Genesis", book?.DisplayName);
        Assert.Equal("gen", book?.Abbreviation);
    }

    [Fact]
    public void EnsureValidBook_InvalidTryGetBookReturnsNull()
    {
        Book? book = Books.GetBook("random_book");
        Assert.Null(book);
    }
}