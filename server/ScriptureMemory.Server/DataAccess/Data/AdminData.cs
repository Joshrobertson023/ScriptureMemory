//using System.Linq;
//using DataAccess.DataInterfaces;
//using DataAccess.Models;

//namespace DataAccess.Data;

//public class AdminData : IAdminData
//{

//    public async Task<IEnumerable<string>> GetAdminUsernames()
//    {
//        const string sql = @"
//SELECT
//    USERNAME AS Username
//FROM ADMINS
//ORDER BY USERNAME";

//        var results = await _db.GetData<AdminUser, dynamic>(sql, new { }, "Default");
//        return results.Select(r => r.Username);
//    }

//    public async Task<IEnumerable<AdminSummary>> GetAdminsWithDetails()
//    {
//        const string sql = @"
//SELECT
//    U.USERNAME AS Username,
//    U.EMAIL AS Email
//FROM USERS U
//INNER JOIN ADMINS A ON A.USERNAME = U.USERNAME
//ORDER BY U.USERNAME";

//        var results = await _db.GetData<AdminSummary, dynamic>(sql, new { }, "Default");
//        return results;
//    }

//    public async Task<bool> IsAdmin(string username)
//    {
//        const string sql = @"
//SELECT
//    USERNAME AS Username
//FROM ADMINS
//WHERE USERNAME = :Username";

//        var results = await _db.GetData<AdminUser, dynamic>(sql, new { Username = username }, "Default");
//        return results.Any();
//    }

//    public async Task AddAdmin(string username)
//    {
//        const string sql = @"
//INSERT INTO ADMINS (USERNAME)
//VALUES (:Username)";

//        await _db.SaveData<dynamic, dynamic>(sql, new { Username = username }, "Default");
//    }

//    public async Task RemoveAdmin(string username)
//    {
//        const string sql = @"
//DELETE FROM ADMINS
//WHERE USERNAME = :Username";

//        await _db.SaveData<dynamic, dynamic>(sql, new { Username = username }, "Default");
//    }
//}

