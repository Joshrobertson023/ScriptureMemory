namespace ScriptureMemory.Server.Data.DataAccess.Bible;

public class BibleData
{
    private readonly ApplicationDbContext _dbContext;

    public BibleData(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
}