using DataAccess.Data;
using DataAccess.Models;
using ScriptureMemory.Server.DataAccess.Models;
using ScriptureMemory.Server.Tools;
using VerseAppNew.Server.Services;

namespace ScriptureMemory.Server.Services;

public class VerseOfDayService
{
    private readonly VerseOfDayData _vodData;
    private readonly VerseData _verseData;
    private readonly ActivityLogger _activityLogger;

    public VerseOfDayService(
        VerseOfDayData vodData,
        VerseData verseData,
        ActivityLogger activityLogger)
    {
        _vodData = vodData;
        _verseData = verseData;
        _activityLogger = activityLogger;
    }

    public async Task<int> InsertVod(string readableReference, int adminId)
    {
        Reference reference = ReferenceParser.Parse(readableReference);

        int newId = await _vodData.InsertPassage(reference.ReadableReference, adminId);

        List<Verse> verses = await _verseData.GetVerses(reference.Book, reference.Chapter, reference.Verses);

        await _vodData.InsertVerses(verses, newId);

        await _activityLogger.LogAdminAction(
            new AdminAction
            {
                AdminId = adminId,
                ActionType = Enums.AdminActionType.VodAdded,
                Timestamp = DateTime.UtcNow,
                JsonMetadata = new
                {
                    VodId = newId
                }
            });

        return newId;
    }
}
