using DataAccess.Models;

namespace DataAccess.DataInterfaces;

public interface IVerseOfDayData
{
    Task<VerseOfDayInfo> GetLastUsedVerseOfDayData();
    Task<VerseOfDay> GetNextPassageInSequence(int lastUsedId);
    Task CreateVerseOfDay(VerseOfDay verseOfDay);
    Task SetTodayVod(VerseOfDayInfo info);
    Task<VerseOfDay?> GetCurrentVerseOfDay();
    Task DeleteVerseOfDay(int id);
    Task<List<VerseOfDay>> GetUpcomingVerseOfDay();
    Task ResetQueueToBeginning();
}