using DataAccess.Data;
using DataAccess.Models;
using ScriptureMemory.Server.Tools;

namespace ScriptureMemory.Server.Services;

public class VerseOfDayService
{
    private readonly VerseOfDayData _data;
    private readonly VerseData _verseData;

    public VerseOfDayService(
        VerseOfDayData data,
        VerseData verseData)
    {
        _data = data;
        _verseData = verseData;
    }

    public async Task<int> InsertVod(string readableReference, int adminId)
    {
        Reference reference = ReferenceParser.Parse(readableReference);

        int newId = await _data.InsertPassage(reference.ReadableReference, adminId);

        List<Verse> verses = await _verseData.GetVerses(reference.Book, reference.Chapter, reference.Verses);

        await _data.InsertVerses(reference.Verses, newId);

        return newId;
    }
}
