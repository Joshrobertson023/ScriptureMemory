using DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.DataInterfaces;

public interface IBanData
{
    Task<Ban?> GetActiveBan(string username);
    Task<List<Ban>> GetAllBans(string username);
    Task<Ban> CreateBan(string username, string adminBanned, string? reason, DateTime? banExpireDate);
    Task DeleteBan(int banId);
    Task<bool> IsUserBanned(string username);
}































