using DataAccess.Models;

namespace DataAccess.DataInterfaces;

public interface ICategoryData
{
    Task<IEnumerable<Category>> GetAll();
    Task<IEnumerable<Category>> GetTop(int limit);
    Task<Category?> GetCategoryByName(string name);
    Task<IEnumerable<int>> GetCategoryIdsForCollection(int collectionId);
    Task<IEnumerable<int>> GetCollectionIdsForCategory(int categoryId);
    Task SetCategoriesForCollection(int collectionId, IEnumerable<int> categoryIds);
    Task Create(string name);
    Task Delete(int categoryId);
    Task RemoveCategoryLinks(int categoryId);
    Task<List<string>> GetVersesInCategory(int categoryId);
    Task AddVerseToCategory(int categoryId, string verseReference);
    Task DeleteVerseFromCategory(int categoryId, string verseReference);
}



