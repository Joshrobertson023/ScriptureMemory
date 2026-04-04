using DataAccess.Data;

namespace VerseAppNew.Server.Services;

public sealed class VerseService
{
    private readonly VerseData verseContext;

    public VerseService(VerseData verseContext)
    {
        this.verseContext = verseContext;
    }
}
