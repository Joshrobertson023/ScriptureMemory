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
    public async Task<Verse> InsertVerse(Verse verse)
    {
        foreach (var translation in verse.TranslationContents)
        {
            translation.LastUpdated = DateTime.UtcNow;
        }
        
        _dbContext.Verses.Add(verse);
        
        await _dbContext.SaveChangesAsync();

        return verse;
    }

    /// <summary>
    /// Gets a verse by reference
    /// </summary>
    /// <param name="book"></param>
    /// <param name="chapter"></param>
    /// <param name="verse"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<Verse> GetVerse(string book, int chapter, int verse)
    {
        if (!Books.TryGetBook(book, out var bookResult))
            throw new InvalidOperationException($"{book} is not a valid book.");

        return _dbContext.Verses
            .Single(v =>
                v.Reference.Book.DisplayName == bookResult.DisplayName
                && v.Reference.Chapter == chapter
                && v.Reference.VerseNumbers.First() == verse);
    }

    /// <summary>
    /// Gets a verse by id
    /// </summary>
    /// <param name="verseId"></param>
    /// <returns></returns>
    public async Task<Verse> GetVerse(string verseId)
    {
        return _dbContext.Verses
            .Single(v =>
                v.Id == verseId.Trim());
    }

    /// <summary>
    /// Gets a verse translation content by reference and version
    /// </summary>
    /// <param name="book"></param>
    /// <param name="chapter"></param>
    /// <param name="verse"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<VerseTranslationContent> GetVerseTranslationContent(string book, int chapter, int verse, string version)
    {
        if (!Books.TryGetBook(book, out var bookResult))
            throw new InvalidOperationException($"{book} is not a valid book.");

        if (!Tools.AvailableBibles.TryGetBible(version, out var bibleResult))
            throw new InvalidOperationException($"{version} Bible not found.");

        return _dbContext.VerseTranslationContents
            .First(c =>
                c.VerseNavigation.Reference.Book.DisplayName == bookResult.DisplayName
                && c.VerseNavigation.Reference.Chapter == chapter
                && c.VerseNavigation.Reference.VerseNumbers.First() == verse
                && c.Version == version);
    }
}