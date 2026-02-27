using System.Collections.Generic;
using DataAccess.Models;

namespace DataAccess.DataInterfaces;
public interface ICollectionData
{
    Task<int> InsertCollection(Collection collection);
    Task<int> GetNextOrderPosition(int userId);
    Task<bool> FavoritesExists(int userId);
    Task<bool> SavedFromPublishedExists(Collection collection);
    Task<int> GetAuthorId(int publishedId);
    Task<List<Collection>> GetUserCollections(int userId);
    Task<string> GetAuthorName(int publishedId);
}
