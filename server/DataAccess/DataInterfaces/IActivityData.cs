using DataAccess.Models;

namespace DataAccess.DataInterfaces;

public interface IActivityData
{
    Task CreateActivity(Activity activity);
    Task<List<Activity>> GetUserActivity(string username, int limit = 10);
    Task<List<Activity>> GetFriendsActivity(string username, int limit = 10);
    Task<List<Activity>> GetFriendActivity(string username, string viewerUsername, int limit = 10);
}





