using ScriptureMemory.Server.Tools;

namespace ScriptureMemory.UnitTests.BookTests;

public class EnsureValidBook
{
    [Fact]
    public void EnsureValidBook_EnsuresValidBook()
    {
        Assert.True(AllBooksInitializer.ValidateBook("Genesis"));
        Assert.False(AllBooksInitializer.ValidateBook("Genesiss"));
    }
}