using DataAccess.Models;

namespace DataAccess.DataInterfaces;
public interface IPracticeSessionData
{
    Task<int> InsertPracticeSession(PracticeSession session);
    Task<List<PracticeSession>> GetPracticeSessionsByVerse(string username, string readableReference, int limit = 5);
    Task<List<PracticeSession>> GetPracticeSessionsByUserVerseId(string username, int userVerseId, int limit = 5);
}





















