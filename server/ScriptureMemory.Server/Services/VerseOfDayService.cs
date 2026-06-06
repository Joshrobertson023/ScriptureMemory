using DataAccess.Data;
using DataAccess.Models;
using ScriptureMemory.Server.DataAccess.Models;
using ScriptureMemory.Server.Tools;
using System.Text.Json;
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

        List<Verse> verses = await _verseData.GetVerses(reference.Book, reference.Chapter, reference.VerseNumbers);

        await _vodData.InsertVerses(verses, newId);

        await _activityLogger.LogAdminAction(
            new AdminAction
            {
                AdminId = adminId,
                ActionType = Enums.AdminActionType.VodAdded,
                Timestamp = DateTime.UtcNow,
                JsonMetadata = JsonSerializer.Serialize(new
                {
                    VodId = newId
                })
            });

        return newId;
    }

    public async Task DeleteVods(List<int> vodPassageIds, int adminId)
    {
        // Delete one at a time so each date compensation is calculated correctly
        foreach (var id in vodPassageIds)
            await _vodData.DeleteVod(id);

        await _activityLogger.LogAdminAction(
            new AdminAction
            {
                AdminId = adminId,
                ActionType = Enums.AdminActionType.VodDeleted,
                Timestamp = DateTime.UtcNow,
                JsonMetadata = JsonSerializer.Serialize(new
                {
                    VodIds = vodPassageIds
                })
            });
    }
}
