using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Pgvector;
using ScriptureMemory.Server.CustomExceptions;
using ScriptureMemory.Server.Data.DtoMappings;
using ScriptureMemory.Server.Data.Dtos;
using ScriptureMemory.Server.Data.Models;
using ScriptureMemory.Server.Services;

namespace ScriptureMemory.Server.Data.DataAccess.Bible;

public class BibleRepository(
    BibleApi _bibleApi,
    BibleData _bibleData,
    VerseDataDapper _verseData,
    IDistributedCache _distributedCache,
    ILogger<BibleRepository> _logger)
{
    /// <summary>
    /// Gets a chapter from either the db or cache,and caches if not found in cache
    /// </summary>
    /// <param name="bible"></param>
    /// <param name="book"></param>
    /// <param name="chapterNum"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<ResponseChapterDto> GetChapterDto(Server.DataAccess.Models.Bible bible, Book book, int chapterNum)
    {
        Reference referenceToFetch = new(book, chapterNum);

        var cachedChapterDto = await _distributedCache.GetAsync(referenceToFetch.CacheKey);

        if (cachedChapterDto is not null)
        {
            ResponseChapterDto cacheChapterDto = JsonSerializer.Deserialize<ResponseChapterDto>(cachedChapterDto)
                ?? throw new Exception($"Error deserializing {nameof(Chapter)}");
            
            _logger.LogInformation("Chapter found in cache: {Reference}", referenceToFetch.ReadableReference);

            return cacheChapterDto;
        }

        var apiResponse = await _bibleApi.GetFullChapter(bible, referenceToFetch);

        if (apiResponse.Data is null)
        {
            throw new InvalidOperationException("Api response returned null");
        }

        ResponseChapterDto apiChapterDto = new ResponseChapterDto(
            apiResponse.Data.Reference,
            apiResponse.Data.Content,
            apiResponse.Data.Copyright);

        var serializedChapter = JsonSerializer.Serialize(apiChapterDto);

        var cacheOptions = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(CacheExpirations.ChapterContentExpiration);

        await _distributedCache.SetStringAsync(referenceToFetch.CacheKey, serializedChapter, cacheOptions);
        
        _logger.LogInformation("Cached chapter: {Reference}", referenceToFetch.ReadableReference);

        return apiChapterDto;
    }

    /// <summary>
    /// Gets every verse (with its translation content) making up a passage, checking the distributed
    /// cache per-verse first and only querying the database for the verses that missed
    /// </summary>
    /// <param name="reference"></param>
    /// <param name="translation"></param>
    /// <returns></returns>
    public async Task<Passage> GetPassage(Reference reference, string translation)
    {
        var verseNumbers = reference.VerseNumbers
            ?? throw new InvalidOperationException("Reference has no verse numbers.");

        var cachedVerses = new List<Verse>();
        var missingVerseNumbers = new List<int>();

        foreach (var verseNum in verseNumbers)
        {
            var cacheKey = $"{translation}.{reference.Book.Abbreviation.ToUpper()}.{reference.Chapter}.{verseNum}";
            var cached = await _distributedCache.GetAsync(cacheKey);

            if (cached is null)
            {
                missingVerseNumbers.Add(verseNum);
                continue;
            }

            var content = JsonSerializer.Deserialize<VerseTranslationContent>(cached)
                ?? throw new Exception($"Error deserializing {nameof(VerseTranslationContent)}");

            var verse = new Verse(reference.Book, reference.Chapter, verseNum);
            content.VerseId = verse.Id;
            content.VerseNavigation = verse;
            verse.TranslationContents = new List<VerseTranslationContent> { content };

            cachedVerses.Add(verse);

            _logger.LogInformation("Verse content found in cache: {CacheKey}", cacheKey);
        }

        if (missingVerseNumbers.Count == 0)
        {
            return new Passage
            {
                Reference = reference,
                Verses = cachedVerses.OrderBy(v => v.Reference.VerseNumbers!.First()).ToList()
            };
        }

        var missingReference = new Reference(reference.Book, reference.Chapter, missingVerseNumbers);
        var fetchedPassage = await _verseData.GetPassage(missingReference, translation);

        await CacheVerseContents(fetchedPassage.Verses, translation);

        var allVerses = cachedVerses
            .Concat(fetchedPassage.Verses)
            .OrderBy(v => v.Reference.VerseNumbers!.First())
            .ToList();

        return new Passage
        {
            Reference = reference,
            Verses = allVerses
        };
    }

    /// <summary>
    /// Finds the verses whose translation content embedding is closest to the given embedding,
    /// caching each result's translation content for later lookups
    /// </summary>
    public async Task<List<Verse>> GetVersesSemanticSearch(Vector embedding, string translation)
    {
        var verses = await _verseData.GetVersesSemanticSearch(embedding, translation);
        await CacheVerseContents(verses, translation);
        return verses;
    }

    /// <summary>
    /// Finds the verses whose translation content embedding is closest to any of the given embeddings,
    /// caching each result's translation content for later lookups
    /// </summary>
    public async Task<List<Verse>> GetVersesSemanticSearch(List<Vector> embeddings, string translation)
    {
        var verses = await _verseData.GetVersesSemanticSearch(embeddings, translation);
        await CacheVerseContents(verses, translation);
        return verses;
    }

    private async Task CacheVerseContents(List<Verse> verses, string translation)
    {
        var cacheOptions = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(CacheExpirations.VerseContentExpiration);

        foreach (var verse in verses)
        {
            var content = verse.TranslationContents?.FirstOrDefault(c => c.Version == translation);
            if (content is null)
                continue;

            await _distributedCache.SetStringAsync(
                content.CacheKey,
                JsonSerializer.Serialize(content),
                cacheOptions);

            _logger.LogInformation("Cached verse content: {CacheKey}", content.CacheKey);
        }
    }
}