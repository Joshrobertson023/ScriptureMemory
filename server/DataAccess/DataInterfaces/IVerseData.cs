using DataAccess.Models;

namespace DataAccess.DataInterfaces;
public interface IVerseData
{
    Task InsertVerse(Verse verse);
    Task<Verse?> GetVerse(string reference);
    Task<List<Verse>> GetChapterVerses(string book, int chapter);
    Task UpdateUsersMemorizedVerse(string reference);
    Task UpdateUsersSavedVerse(string reference);
    Task<Verse?> GetVerseFromId(int id);
    Task<List<Verse>> GetAllVersesFromReferenceList(List<string> references);
    Task<List<Verse>> GetTopSavedVerses(int top);
    Task<List<Verse>> GetTopMemorizedVerses(int top);
    //Task<SearchData> GetVerseSearchResults(string search);
    //Task<string> GetPassageTextFromListOfReferences(List<string> references);
    //Task<List<Verse>> GetTopVersesInCategory(int top, int categoryId);
}
