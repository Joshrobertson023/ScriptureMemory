using DataAccess.Models;
using static ScriptureMemoryLibrary.Enums;

namespace DataAccess.DataInterfaces;

public interface ISearchData
{
    Task TrackSearch(string searchTerm, SearchType searchType);
    Task InsertSearch(Search search);
    Task<Search> GetSearch(int id);
    Task<List<int>> GetTrendingSearchesFromSearches();
    Task InsertTrendingSearch(int searchId);
    Task<List<Search>> GetTrendingSearches();
}









































































