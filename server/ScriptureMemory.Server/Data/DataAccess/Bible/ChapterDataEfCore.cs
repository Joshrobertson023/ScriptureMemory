namespace ScriptureMemory.Server.Data.DataAccess.Bible;

public class ChapterDataEfCore
{
    private readonly ApplicationDbContext _dbContext;
    
    public ChapterDataEfCore(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InsertNewChapter(Chapter chapter)
    {
        if (string.IsNullOrEmpty(chapter.Version)
            || string.IsNullOrEmpty(chapter.Book)
            || string.IsNullOrEmpty(chapter.Id))
        {
            throw new InvalidOperationException("Chapter is missing one or more required properties.");
        }

        var existingChapters = _dbContext.Chapters.ToList();
        
        if (existingChapters.Any(x => x.Version == chapter.Version && x.Book == chapter.Book))
            throw new InvalidOperationException("Chapter already exists.");
        
        _dbContext.Chapters.Add(chapter);
        await _dbContext.SaveChangesAsync();
    }
}