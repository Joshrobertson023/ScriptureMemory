namespace DataAccess.Data;

public interface IAdminData
{
    Task<int> InsertAdmin(Admin admin);
    Task UpdatePassword(int adminId, string password);
    Task UpdatePersonalEmail(int adminId, string personalEmail);
    Task<Admin?> GetAdminByUsername(string username);
    Task<Admin?> GetAdminById(int id);
    Task<List<Admin>> GetAllAdmins();
    Task<int> InsertAdminAction(AdminAction action);
    Task<List<AdminAction>> GetAdminActions(int? page = null, int? pageSize = null);
}