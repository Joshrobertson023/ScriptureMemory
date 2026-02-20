using System.Collections.Generic;
using DataAccess.Models;

namespace DataAccess.DataInterfaces;
public interface ICollectionData
{
    Task DeleteCollection(int id, IUserPassageData uvData);
    Task DeleteCollectionsByUsername(string username, IUserPassageData uvData);
    Task<Collection?> GetCollection(Collection collection);
    Task<Collection?> GetCollectionById(int collectionId);
    Task<Collection?> GetCollectionByPublishedId(int publishedId);
    Task<int> GetMostRecentCollection(string username);
    Task<IEnumerable<Collection>> GetUserCollectionsWithUserVerses(string username);
    Task<IEnumerable<Collection>> GetUserPublicCollections(string username);
    Task<IEnumerable<Collection>> GetUserFriendCollections(string username, string viewerUsername);
    Task<Collection?> GetFriendCollectionWithVerses(int collectionId, string? viewerUsername);
    Task<IEnumerable<PublishedCollection>> GetPopularCollections(int top);
    Task<IEnumerable<PublishedCollection>> GetRecentCollections(int top);
    Task<IEnumerable<PublishedCollection>> GetPublishedCollectionsByAuthor(string username);
    Task InsertCollection(Collection collection);
    Task SetUserSavedCollection(int id);
    Task IncrementPublishedCollectionSaves(int publishedId);
    Task UpdateCollection(Collection collection);
    Task<IEnumerable<PublishedCollection>> SearchPublishedCollections(string query, IEnumerable<string> verseReferences, int limit);
    Task<int> GetMostRecentPublishedCollection(string username);
    Task<int> PublishCollection(Collection collectionBeingPublished, string description);
    Task AssignNewPublishedCollectionCategories(int latestId, List<int> categoryIds);
    Task<IEnumerable<PublishedCollection>> GetPublishedCollectionsByCategory(int categoryId);
    Task<PublishedCollection?> GetPublishedCollectionById(int publishedId);
    Task UpdateCollectionVerseOrder(int collectionId, string verseOrder);
    Task InsertCollectionNote(CollectionNote note);
    Task UpdateCollectionNote(CollectionNote note);
    Task DeleteCollectionNote(string noteId, int collectionId);
    Task DeleteCollectionNotesByCollection(int collectionId);
    Task<IEnumerable<PublishedCollection>> GetPendingCollections();
    Task ApprovePublishedCollection(int publishedId);
    Task RejectPublishedCollection(int publishedId);
}