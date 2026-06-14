namespace ScriptureMemory.Server.Data.DataAccess;

public interface ISessionData
{
    Task CreateSession(int userId, Session session);
}