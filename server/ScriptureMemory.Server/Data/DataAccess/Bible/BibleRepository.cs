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

        ResponseChapterDto apiChapterDto = new(
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

    public async Task<Passage> GetKjvPassageForSemanticSearch(Reference reference)
    {
        var cachedPassage = await _distributedCache.GetAsync(reference.CacheKey);

        if (cachedPassage is not null)
        {
            _logger.LogInformation("Passage found in cache.");
            
            return JsonSerializer.Deserialize<Passage>(cachedPassage)
                ?? throw new Exception("Error deserializing passage");
        }
        
        var passage = await _verseData.GetPassage(reference);
        
        var cacheOptions = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(CacheExpirations.VerseContentExpiration);
        
        await _distributedCache.SetStringAsync(reference.CacheKey, 
            JsonSerializer.Serialize(passage),
            cacheOptions);
        
        _logger.LogInformation("Cached passage: {Reference}", passage.Reference.ReadableReference);
        
        return passage;
    }

    public async Task<List<Verse>> GetKjvContentForSemanticSearch(Vector embedding, string translation)
    {
        HashSet<string> verseIdsNotFoundInCache = new();
        
        var embeddingResultVerses = await _verseData.GetKjvContentForSemanticSearch(
            embedding, 
            translation == "kjv" ? 20 : 5);
        
        if (translation == "kjv")
            return embeddingResultVerses;
        
        HashSet<string> embeddingResultVerseIds = embeddingResultVerses.Select(v => v.Id).ToHashSet();

        // Check cache for verse content
        foreach (var verse in embeddingResultVerses)
        {
            var cachedVerse = await _distributedCache.GetStringAsync(verse.CacheKey
                ?? throw new Exception("Error getting verse CacheKey"));

            if (cachedVerse is not null)
            {
                verse.TranslationContents = (JsonSerializer.Deserialize<Verse>(cachedVerse)
                    ?? throw new Exception("Error deserializing cached verse")).TranslationContents;
                
                _logger.LogInformation("Verse found in cache: {Reference}.", verse.Id);
            }
            else
            {
                verseIdsNotFoundInCache.Add(verse.Id);
            }
        }

        List<Verse> versesFetchedFromApi = new();

        // Fetch verse content for verses not found in cache
        foreach (var verseId in verseIdsNotFoundInCache)
        {
            _logger.LogInformation("Verse not found in cache and fetching: {VerseId}", verseId);
            
            var verse = embeddingResultVerses.Single(v => v.Id == verseId);
            
            if (verse.TranslationContents is null)
                verse.TranslationContents = new();
            
            if (!AvailableBibles.TryGetBible(translation, out var bible))
                throw new BibleUnavailableException("{Translation} not available", translation);
                
            verse.TranslationContents.Add(new VerseTranslationContent
            {
                PlainText = await _bibleApi.GetVersePlaintext(bible!.Id, verseId)
            });
            
            versesFetchedFromApi.Add(verse);
        }
        
        // Compile list of ordered verses
        List<Verse> returnVerses = new();

        foreach (var id in embeddingResultVerseIds)
        {
            if (verseIdsNotFoundInCache.Contains(id))
                returnVerses.Add(versesFetchedFromApi.Single(v => v.Id == id));
            else
                returnVerses.Add(embeddingResultVerses.Single(v => v.Id == id));
        }
        
        // Cache verses
        foreach (var verse in returnVerses)
        {
            await _distributedCache.SetStringAsync(
                verse.CacheKey ?? throw new Exception("Error getting verse CacheKey"),
                JsonSerializer.Serialize(verse),
                new DistributedCacheEntryOptions().SetAbsoluteExpiration(CacheExpirations.VerseContentExpiration));
            
            _logger.LogInformation("Cached verse: {VerseId}", verse.Id);
        }
        
        return embeddingResultVerses;
    }

    public async Task<List<Verse>> GetKjvContentForSemanticSearch(List<Vector> embeddings, string translation)
    {
        var verses = await _verseData.GetKjvContentForSemanticSearch(embeddings);
        
        return verses;
    }
}