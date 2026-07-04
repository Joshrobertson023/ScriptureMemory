using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using ScriptureMemory.Server;
using ScriptureMemory.Server.DataAccess.Models;

namespace DataAccess.Data;

public class AdminDataEfCore : IAdminData
{
    private readonly IConfiguration _config;
    private readonly ApplicationDbContext _dbContext;

    public AdminDataEfCore(IConfiguration config, ApplicationDbContext dbContext)
    {
        _config = config;
        _dbContext = dbContext;
    }

    public async Task<int> InsertAdmin(Admin admin)
    {
        _dbContext.Admins.Add(admin);

        await _dbContext.SaveChangesAsync();

        return admin.UserId;
    }

    public async Task UpdatePassword(int adminId, string password)
    {
        var admin = await _dbContext.Admins
            .SingleAsync(a => a.UserId == adminId);

        admin.HashedPassword = password;

        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdatePersonalEmail(int adminId, string personalEmail)
    {
        var admin = await _dbContext.Admins
            .SingleAsync(a => a.UserId == adminId);

        admin.PersonalEmail = personalEmail;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<Admin?> GetAdminByUsername(string username)
    {
        return await _dbContext.Admins
            .SingleOrDefaultAsync(a => a.AdminEmail == username);
    }

    public async Task<Admin?> GetAdminById(int id)
    {
        return await _dbContext.Admins
            .SingleOrDefaultAsync(a => a.UserId == id);
    }

    public async Task<List<Admin>> GetAllAdmins()
    {
        return await _dbContext.Admins
            .ToListAsync();
    }

    public async Task<int> InsertAdminAction(AdminAction action)
    {
        _dbContext.Set<AdminAction>().Add(action);

        await _dbContext.SaveChangesAsync();

        return action.Id;
    }

    public async Task<List<AdminAction>> GetAdminActions(int? page = null, int? pageSize = null)
    {
        page ??= 1;
        pageSize ??= 50;

        var offset = (page.Value - 1) * pageSize.Value;

        return await _dbContext.Set<AdminAction>()
            .OrderByDescending(a => a.Timestamp)
            .Skip(offset)
            .Take(pageSize.Value)
            .ToListAsync();
    }
}