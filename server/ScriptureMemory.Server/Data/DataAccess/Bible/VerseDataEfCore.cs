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

    public async Task InsertContentForVerse(string verseId, VerseTranslationContent content)
    {
        if (string.IsNullOrWhiteSpace(content.VerseId) || string.IsNullOrWhiteSpace(verseId))
            throw new InvalidOperationException("Content must reference a verse by Id");
        if (verseId != content.VerseId)
            throw new InvalidOperationException("Content is not referencing correct verse Id");

        if (_dbContext.VerseTranslationContents.Any(c =>
                c.VerseId == verseId && c.Version == content.Version.Trim()))
        {
            throw new InvalidOperationException("Content already exists for verse in this version");
        }
        
        content.LastUpdated = DateTime.UtcNow;
        
        _dbContext.VerseTranslationContents.Add(content);
        
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Inserts a brand new verse and all its translation's contents.
    /// </summary>
    /// <param name="verse"></param>
    /// <returns></returns>
    public async Task InsertVerse(Verse verse)
    {
        if (_dbContext.Verses.Any(v => v.Id == verse.Id.Trim()))
            throw new InvalidOperationException("Verse already exists");

        foreach (var translation in verse.Translations)
        {
            translation.LastUpdated = DateTime.UtcNow;
        }
        
        _dbContext.Verses.Add(verse);

        foreach (var verseInTranslation in verse.Translations)
        {
            _dbContext.VerseTranslationContents.Add(verseInTranslation);
        }
        
        await _dbContext.SaveChangesAsync();
    }
}