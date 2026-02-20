using DataAccess.Data;
using DataAccess.Models;
using VerseAppLibrary;

namespace DataAccess.DataInterfaces;

public interface IActivityLoggingData
{
    Task Create(ActivityLog log);
    Task<ActivityLog?> GetById(int id);
    Task<PagedLogs<ActivityLog>> GetByUser(string username, int page = 1, int pageSize = 50);
    Task<PagedLogs<ActivityLog>> GetByEntity(Enums.EntityType entityType, int entityId, int page = 1, int pageSize = 50);
    Task<PagedLogs<ActivityLog>> GetByActionType(Enums.ActionType actionType, int page = 1, int pageSize = 50);
    Task<PagedLogs<ActivityLog>> GetByDateRange(DateTime from, DateTime to, int page = 1, int pageSize = 50);
    Task<PagedLogs<ActivityLog>> GetAdminActions(int page = 1, int pageSize = 50);
    Task<int> DeleteOlderThan(DateTime cutoff);
    Task<int> DeleteLogsForUser(string username);
}