using DataAccess.Models;
using DataAccess.Requests;
using System.Collections.Generic;

namespace DataAccess.DataInterfaces;
public interface ICollectionData
{
    Task<int> InsertCollection(Collection collection);
    Task<int> GetNextOrderPosition(int userId);
    Task<bool> FavoritesExists(int userId);
    Task<bool> SavedFromPublishedExists(int publishedId, int userId);
    Task SaveCollection(SaveCollectionRequest request);
    Task<int> GetAuthorId(int publishedId);
    Task<string> GetAuthorName(int publishedId);
    Task<List<Collection>> GetUserCreatedCollections(int userId);
    Task<List<CollectionNote>> GetCollectionNotes(int collectionId);
    Task<int> InsertCollectionNote(CollectionNote note);
}
