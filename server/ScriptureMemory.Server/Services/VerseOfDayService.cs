using DataAccess.Data;
using DataAccess.Models;
using ScriptureMemory.Server.Tools;

namespace ScriptureMemory.Server.Services;

public class VerseOfDayService
{
    private readonly VerseOfDayData _vodData;
    private readonly VerseData _verseData;

    public VerseOfDayService(
        VerseOfDayData vodData,
        VerseData verseData)
    {
        _vodData = vodData;
        _verseData = verseData;
    }

    public async Task<int> InsertVod(string readableReference, int adminId)
    {
        Reference reference = ReferenceParser.Parse(readableReference);

        int newId = await _vodData.InsertPassage(reference.ReadableReference, adminId);

        List<Verse> verses = await _verseData.GetVerses(reference.Book, reference.Chapter, reference.Verses);

        await _vodData.InsertVerses(verses, newId);

        return newId;
    }
}
