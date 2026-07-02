namespace ScriptureMemory.Server.Data.DataAccess.Bible;

public class VerseDataEfCore : IVerseData
{
    private readonly IConfiguration _config;
    private readonly ApplicationDbContext _dbContext;
    
    public VerseDataEfCore(IConfiguration config, ApplicationDbContext dbContext)
    {
        _config = config;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Inserts verse translation content for a verse id
    /// </summary>
    /// <param name="verseId"></param>
    /// <param name="content"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task InsertContentForVerse(string verseId, VerseTranslationContent content)
    {
        if (verseId != content.VerseId)
            throw new InvalidOperationException("Content is not referencing correct verse Id");
        
        content.LastUpdated = DateTime.UtcNow;
        
        _dbContext.VerseTranslationContents.Add(content);
        
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Inserts a new verse and its translation contents
    /// </summary>
    /// <param name="verse"></param>
    /// <returns></returns>
    public async Task InsertVerse(Verse verse)
    {
        foreach (var translation in verse.TranslationContents)
        {
            translation.LastUpdated = DateTime.UtcNow;
        }
        
        _dbContext.Verses.Add(verse);
        
        await _dbContext.SaveChangesAsync();
    }
}