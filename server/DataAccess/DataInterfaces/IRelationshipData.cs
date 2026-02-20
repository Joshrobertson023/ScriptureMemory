using DataAccess.Models;

namespace DataAccess.DataInterfaces;

public interface IRelationshipData
{
    Task CreateRelationship(Relationship relationship);
    Task UpdateRelationship(string username1, string username2, int type);
    Task DeleteRelationship(string username1, string username2);
    Task<Relationship?> GetRelationship(string username1, string username2);
    Task<List<User>> GetFriends(string username);
    Task<List<User>> GetFriendNames(string username);
    Task<List<string>> GetPendingRequests(string username);
    Task<bool> AreFriends(string username1, string username2);
}

