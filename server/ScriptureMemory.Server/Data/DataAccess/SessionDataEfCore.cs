namespace ScriptureMemory.Server.Data.DataAccess;

public class SessionDataEfCore : ISessionData
{
    private readonly ApplicationDbContext _context;
    
    public SessionDataEfCore(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateSession(int userId, Session session)
    {
        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();
    }
}