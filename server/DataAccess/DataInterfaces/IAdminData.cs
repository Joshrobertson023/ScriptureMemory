using System.Collections.Generic;
using DataAccess.Models;

namespace DataAccess.DataInterfaces;

public interface IAdminData
{
    Task<IEnumerable<string>> GetAdminUsernames();
    Task<IEnumerable<AdminSummary>> GetAdminsWithDetails();
    Task<bool> IsAdmin(string username);
    Task AddAdmin(string username);
    Task RemoveAdmin(string username);
}

