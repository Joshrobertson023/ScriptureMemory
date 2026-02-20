using ScriptureMemoryLibrary;
using Xunit;
namespace VerseApp.UnitTests.ReferenceParseTests;

public class GetReferencesFromVersesInReferenceTests
{
    [Fact]
    public void GetReferencesFromVersesInReference_DoubleDigitRangeWithSeparateVerse_ReturnsAllReferences()
    {
        var result = ReferenceParse.GetReferencesFromVersesInReference("Psalms 119:2-21, 24");
        Assert.Equal(new List<string>
        {
            "Psalms 119:2", "Psalms 119:3", "Psalms 119:4", "Psalms 119:5",
            "Psalms 119:6", "Psalms 119:7", "Psalms 119:8", "Psalms 119:9",
            "Psalms 119:10", "Psalms 119:11", "Psalms 119:12", "Psalms 119:13",
            "Psalms 119:14", "Psalms 119:15", "Psalms 119:16", "Psalms 119:17",
            "Psalms 119:18", "Psalms 119:19", "Psalms 119:20", "Psalms 119:21",
            "Psalms 119:24"
        }, result);
    }
    [Fact]
    public void GetReferencesFromVersesInReference_NumberedBookDoubleDigitRangeWithSeparateVerse_ReturnsAllReferences()
    {
        var result = ReferenceParse.GetReferencesFromVersesInReference("1 John 4:18-19, 20");
        Assert.Equal(new List<string> { "1 John 4:18", "1 John 4:19", "1 John 4:20" }, result);
    }
    [Fact]
    public void GetReferencesFromVersesInReference_SingleVerse_ReturnsSingleReference()
    {
        var result = ReferenceParse.GetReferencesFromVersesInReference("John 3:16");
        Assert.Equal(new List<string> { "John 3:16" }, result);
    }
}
