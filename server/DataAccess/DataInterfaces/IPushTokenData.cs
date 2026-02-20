using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Models;

namespace DataAccess.DataInterfaces;

public interface IPushTokenData
{
    Task UpsertTokenAsync(string username, string expoPushToken, string platform);
    Task RemoveTokenAsync(string username, string expoPushToken);
    Task RemoveTokensAsync(IEnumerable<string> expoPushTokens);
    Task<IEnumerable<PushToken>> GetTokensForUserAsync(string username);
    Task<IEnumerable<PushToken>> GetTokensForUsersAsync(IEnumerable<string> usernames);
    Task<IEnumerable<PushToken>> GetAllTokensAsync();
}


