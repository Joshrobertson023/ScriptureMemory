using DataAccess.Models;

namespace DataAccess.DataInterfaces;
public interface IUserPassageData
{
    Task<List<UserPassage>> GetMemorized(string username);
    Task<List<UserPassage>> GetInProgress(string username);
    Task<List<UserPassage>> GetNotStarted(string username);
    Task DeleteUserVerse(UserPassage uv);
    Task DeleteUserVersesByCollection(int collectionId);
    Task DeleteUserVersesByUsername(string username);
    Task<UserPassage?> GetUserVerse(int id);
    Task<List<UserPassage>> GetUserVersesByIds(List<int> ids);
    Task<IEnumerable<UserPassage>> GetUserVersesByCollection(int collectionId);
    Task<IEnumerable<UserPassage>> GetUserVersesByUsername(string username);
    Task InsertUserVerse(UserPassage uv);
    Task UpdateUserVerse(UserPassage uv);
    Task InsertUserVersesToNewCollection(List<UserPassage> userVerses, int newCollectionId);
    Task AddUserVersesToNewlyPublishedCollection(List<UserPassage> userVerses, int publishedId);
    Task<List<UserPassage>> GetRecentPractice(string username, int limit = 3);
    Task<List<UserPassage>> GetOverdueVerses(string username);
    Task DeleteUserVersesNotInOrder(int collectionId, IEnumerable<string> readableReferences);
    Task<bool> UserVerseExistsInCollection(int collectionId, string readableReference);
}
