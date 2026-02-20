using DataAccess.Models;

namespace DataAccess.DataInterfaces;

public interface IVerseOfDaySuggestionData
{
    Task<List<VerseOfDaySuggestion>> GetAllSuggestions();
    Task<VerseOfDaySuggestion?> GetSuggestion(int id);
    Task CreateSuggestion(VerseOfDaySuggestion suggestion);
    Task DeleteSuggestion(int id);
    Task ApproveSuggestion(int id);
}



