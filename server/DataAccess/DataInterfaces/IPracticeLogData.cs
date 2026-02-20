using DataAccess.Models;

namespace DataAccess.DataInterfaces;
public interface IPracticeLogData
{
    Task<int> RecordPractice(string username);
    Task<int> GetCurrentStreakLength(string username);
    Task<List<DateTime>> GetPracticeHistory(string username);
}

