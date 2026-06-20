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
    /// Inserts a brand new verse and all the translation's contents.
    /// </summary>
    /// <param name="verse"></param>
    /// <returns></returns>
    public async Task<string> InsertVerse(Verse verse)
    {
        if (_dbContext.Verses.Any(v => v.Id == verse.Id.Trim()))
            throw new InvalidOperationException("Verse already exists");
        
        _dbContext.Verses.Add(verse);

        foreach (var verseInTranslation in verse.Translations)
        {
            _dbContext.VerseTranslationContents
        }

        return "build";
    }

    /// <summary>
    /// Adds a new translation for a verse.
    /// </summary>
    /// <param name="verse"></param>
    /// <param name="newTranslation"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task AddTranslationContent(Verse verse, VerseTranslationContent newTranslation)
    {
        if (_dbContext.VerseTranslationContents.Any(t => t.VerseId == verse.Id)
            && _dbContext.VerseTranslationContents.Any(t => t.Version == newTranslation.Version))
        {
            throw new InvalidOperationException($"Translation already exists for {verse.Reference.ReadableReference}");
        }
    }
}