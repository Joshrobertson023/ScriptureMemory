using DataAccess.Data;
using DataAccess.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Pgvector;
using ScriptureMemory.Server.CustomExceptions;
using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.DataAccess.Models;
using ScriptureMemory.Server.Files.CsvRecordModels;
using ScriptureMemory.Server.Services;
using ScriptureMemory.Server.Tools;
using System.Security.Claims;
using static ScriptureMemory.Server.Tools.Enums;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace VerseAppNew.Server.Services;

public sealed class SearchService(
        IUserData _userData,
        UserDataDapper _userDataDapper,
        UserDataEFCore _userDataEfCore,
        BibleApi _bibleApi,
        EmbeddingGenerator _embeddingGenerator,
        ILogger<SearchService> _logger,
        IMemoryCache _memoryCache,
        IDistributedCache _distributedCache,
        VerseDataDapper _verseData,
        IConfiguration _config)
{

    //public async Task TrackSearch(DataAccess.Requests.SearchRequest request)
    //{
    //    switch(request.SearchType)
    //    {
    //        case SearchType.Verse:
    //            //await
    //            break;
    //    }
    //}

    public async Task<IResult> Search(SearchRequest request, ClaimsPrincipal user)
    {
        var userId = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userId))
            _logger.LogWarning("UserId not found.");

        if (!AvailableBibles.TryGetBible(request.Translation, out var bible))
            throw new BibleUnavailableException("The Bible {bible} is not available.", request.Translation);

        Reference? reference = null;
        var results = new List<SearchResult>();

        // TODO: Refactor to create a method that checks if it's a valid reference instead of throwing
        try
        {
            reference = ReferenceParser.Parse(request.Search);
        }
        catch
        {
            reference = null;
            // Don't search by reference
        }

        // ***********************************************************
        //
        // Todo list:
        //  - Hide caching behind interface that checks that the cached
        //    content isn't broken to minimize times I need to wipe the cache
        //
        // ***********************************************************

        if (reference is not null)
        {
            _logger.LogInformation($"User #{userId} has searched by reference for \"{request.Search}\"");

            return Results.Ok(await GetReferenceSearchResults(request.Translation.ToLower().Trim(), reference));
        }
        else
        {
            _logger.LogInformation($"User #{userId} has searched by keyword for \"{request.Search}\"");

            return Results.Ok(await GetPassageSearchResults(request.Search, request.Translation));
        }

        return Results.Ok(await GetPassageSearchResults(request.Search, request.Translation));
    }

    private async Task<List<SearchResult>> GetReferenceSearchResults(string requestedTranslation, Reference requestedReference)
    {
        var searchResults = new List<SearchResult>();

        // Get the exact passage searched
        var refernceMatchPassageResult = await getKjvPassageForSemanticSearch(requestedReference, requestedTranslation);

        // Add the semantically similar to the searched passage to the search results

        IEnumerable<Vector> referenceMatchVectors = refernceMatchPassageResult.Verses
            .Select(v => v.TranslationContents?.First().Embedding 
                ?? throw new Exception("An embedding was null inside the reference match passage"));

        List<Verse> versesSemanticSearchResult
            = (await GetVersesSemanticSearchResults(
                referenceMatchVectors,
                refernceMatchPassageResult.Verses.Select(v => v.Id).ToArray(),
                requestedTranslation))
            .ToList();

        //if (requestedTranslation != _config["ApiContent:DefaultTranslation"])
        //{
        //    // Fetch verse content from api.bible
        //    _logger.LogDebug("Fetching from api.bible verse content.");

        //    string[] verseIdsToFetch = new string[referenceMatchVectors.Count() + 1];
        //    verseIdsToFetch[0] = passage.Verses.First().Id;

        //    for (int i = 0; i < versesSemanticSearchResult.Count; i++)
        //    {
        //        verseIdsToFetch[i + 1] = versesSemanticSearchResult[i].Id;
        //    }

        //    List<(Verse verse, Task<string> contentTask)> verseContentFetches = versesSemanticSearchResult
        //        .Select(v => (v, contentTask: _bibleApi.GetVersePlaintext(v.Id, requestedTranslation)))
        //        .ToList();

        //    List<string> fetchedVerseCacheKeys = new();

        //    if (bool.Parse(_config["ApiContent:FetchAllAtOnce"] ?? "false"))
        //    {
        //        try
        //        {
        //            await Task.WhenAll(verseContentFetches.Select(f => f.contentTask));

        //            fetchedVerseCacheKeys = verseContentFetches.Select(
        //                v => CacheKeyGenerator.GetVerseCacheKey(v.verse.Id, requestedTranslation))
        //                .ToList();
        //        }
        //        catch (HttpRequestException)
        //        {
        //            _logger.LogError("Failed to fetch verse content with Task.WhenAll, switching to sequential fetch...");

        //            await FetchVerseContentSequentially(verseContentFetches, fetchedVerseCacheKeys, requestedTranslation);
        //        }
        //    }
        //    else
        //    {
        //        await FetchVerseContentSequentially(verseContentFetches, fetchedVerseCacheKeys, requestedTranslation);
        //    }

        //    List<Verse> fetchedVerses = new();

        //    foreach (var verseContentFetch in verseContentFetches)
        //    {
        //        try
        //        {
        //            if (verseContentFetch.verse.TranslationContents is null ||
        //                verseContentFetch.verse.TranslationContents.Count <= 0)
        //            {
        //                verseContentFetch.verse.TranslationContents = new()
        //                {
        //                    new VerseTranslationContent()
        //                };
        //            }

        //            verseContentFetch.verse.TranslationContents?.First().PlainText = verseContentFetch.contentTask.Result;

        //            fetchedVerses.Add(verseContentFetch.verse);
        //        }
        //        catch (AggregateException)
        //        {
        //            // Expected if a Task failed to fetch verse content earlier
        //            continue;
        //        }
        //    };

        //    // Cache recently fetched verses
        //    foreach (var fetchedVerse in fetchedVerses)
        //    {
        //        await _distributedCache.SetStringAsync(
        //            CacheKeyGenerator.GetVerseCacheKey(fetchedVerse.Id, requestedTranslation),
        //            JsonSerializer.Serialize(fetchedVerse),
        //            new DistributedCacheEntryOptions().SetAbsoluteExpiration(CacheExpirations.VerseContentExpiration));

        //        _logger.LogDebug("Cached verse {Id}:{Translation}", fetchedVerse.Id, requestedTranslation);
        //    }

        //    searchResults.Add(new SearchResult
        //    {
        //        Type = SearchResultType.ExactPassage,
        //        Passage = passage,
        //        Rank = 1
        //    });

        //    foreach (var fetchedVerse in fetchedVerses)
        //    {
        //        searchResults.Add(new SearchResult
        //        {
        //            Type = SearchResultType.ExactPassage,
        //            Passage = new Passage
        //            {
        //                Reference = fetchedVerse.Reference,
        //                Verses = new List<Verse> { fetchedVerse }
        //            },
        //            Rank = 2
        //        });
        //    }
        //}
        //else
        //{
        searchResults.Add(new SearchResult
        {
            Type = SearchResultType.ExactPassage,
            Passage = refernceMatchPassageResult,
            Rank = 1
        });

        foreach (var verse in versesSemanticSearchResult)
        {
            searchResults.Add(new SearchResult
            {
                Type = SearchResultType.SemanticVerse,
                Passage = new Passage
                {
                    Reference = verse.Reference,
                    Verses = new List<Verse> { verse }
                },
                Rank = 2
            });
        }
        //}

        await EnsureAllResultsContainContent(searchResults, requestedTranslation);

        return searchResults;
    }

    //private async Task FetchVerseContentSequentially(
    //    List<(Verse verse, Task<string> contentTask)> verseContentFetches,
    //    List<string> fetchedVerseCacheKeys,
    //    string requestedTranslation)
    //{
    //    foreach (var verseContentFetch in verseContentFetches)
    //    {
    //        try
    //        {
    //            _logger.LogDebug(
    //                "Fetching content for {Id}:{Translation}",
    //                verseContentFetch.verse.Id,
    //                verseContentFetch.verse.TranslationContents?.First().Version);

    //            await verseContentFetch.contentTask;

    //            fetchedVerseCacheKeys.Add(
    //                CacheKeyGenerator.GetVerseCacheKey(
    //                    verseContentFetch.verse.Id, 
    //                    requestedTranslation));

    //            break;
    //        }
    //        catch (HttpRequestException)
    //        {
    //            _logger.LogWarning(
    //                "Failed to fetch verse content for {Id}:{Translation}",
    //                verseContentFetch.verse.Id,
    //                verseContentFetch.verse.TranslationContents?.First().Version);
    //        }
    //    }
    //}

    /// <summary>
    /// Gets verse content from embedding results from cache, api, and sets cache
    /// </summary>
    /// <param name="embeddingResultVerses"></param>
    /// <param name="translation"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="BibleUnavailableException"></exception>
    private async Task<List<Verse>> getVersesContent(List<Verse> embeddingResultVerses, string translation)
    {
        HashSet<string> verseIdsNotFoundInCache = new();

        HashSet<string> embeddingResultVerseIds = embeddingResultVerses.Select(v => v.Id).ToHashSet();

        // Check cache for verse content
        foreach (var verse in embeddingResultVerses)
        {
            var cachedVerse = await _distributedCache.GetStringAsync(CacheKeyGenerator.GetVerseCacheKey(verse.Reference, translation)
                ?? throw new Exception("Error getting verse CacheKey"));

            if (cachedVerse is not null)
            {
                verse.TranslationContents = (JsonSerializer.Deserialize<Verse>(cachedVerse)
                    ?? throw new Exception("Error deserializing cached verse")).TranslationContents;

                _logger.LogDebug("Verse found in cache: {Id}:{Translation}.", verse.Id, translation);
            }
            else
            {
                verseIdsNotFoundInCache.Add(verse.Id);

                _logger.LogDebug("Verse not found in cache: {Id}:{Translation}.", verse.Id, translation);
            }
        }

        List<Verse> versesFetchedFromApi = new();

        // Fetch verse content for verses not found in cache
        foreach (var verseId in verseIdsNotFoundInCache)
        {
            _logger.LogDebug("Verse not found in cache and fetching: {VerseId}:{Translation}", verseId, translation);

            var verse = embeddingResultVerses.Single(v => v.Id == verseId);

            if (verse.TranslationContents is null)
                verse.TranslationContents = new();

            if (!AvailableBibles.TryGetBible(translation, out var bible))
                throw new BibleUnavailableException("{Translation} not available", translation);

            try
            {
                verse.TranslationContents.Add(new VerseTranslationContent
                {
                    PlainText = await _bibleApi.GetVersePlaintext(bible?.Id, verseId),
                    Version = translation
                });
            }
            catch (HttpRequestException)
            {
                _logger.LogWarning(
                    "Failed to fetch verse content for {Id}:{Translation}",
                    verseId,
                    bible?.Abbreviation);
            }

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
                CacheKeyGenerator.GetVerseCacheKey(verse.Reference, translation),
                JsonSerializer.Serialize(verse),
                new DistributedCacheEntryOptions().SetAbsoluteExpiration(CacheExpirations.VerseContentExpiration));

            _logger.LogDebug("Cached verse: {VerseId}:{Translation}", verse.Id, translation);
        }

        return embeddingResultVerses;
    }

    public async Task<IEnumerable<Verse>> GetVersesSemanticSearchResults(
    IEnumerable<Vector> embeddings,
    string[] originalVerseIds,
    string translation)
    {
        string defaultTranslation = _config["ApiContent:DefaultTranslation"] ?? "kjv";
        int.TryParse(
            translation == defaultTranslation
                ? _config["ApiContent:FetchCountWhenDefault"]
                : _config["ApiContent:FetchCountWhenNotDefault"], out var numVersesToFetch);

        var embeddingResultVerses = await _verseData.GetKjvContentForSemanticSearch(
            embeddings,
            originalVerseIds,
            numVersesToFetch);

        if (translation == defaultTranslation)
            return embeddingResultVerses;

        return await getVersesContent(embeddingResultVerses, translation);
    }

    public async Task<IEnumerable<Verse>> GetVersesSemanticSearchResults(Vector embedding, string translation)
    {
        string defaultTranslation = _config["ApiContent:DefaultTranslation"] ?? "kjv";
        int.TryParse(
            translation == defaultTranslation
                ? _config["ApiContent:FetchCountWhenDefault"]
                : _config["ApiContent:FetchCountWhenNotDefault"], out var numVersesToFetch);

        var embeddingResultVerses = await _verseData.GetKjvContentForSemanticSearch(
            embedding,
            numVersesToFetch);

        if (translation == defaultTranslation)
            return embeddingResultVerses;

        return await getVersesContent(embeddingResultVerses, translation);
    }

    /// <summary>
    /// Double checks all search results, ensuring the correct translation, and that the plain text is in each verse
    /// </summary>
    /// <param name="results"></param>
    /// <param name="requestedTranslation"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private async Task EnsureAllResultsContainContent(List<SearchResult> results, string requestedTranslation)
    {
        Dictionary<int, string> verseIdsInResults = new(); // Track verses and index found to keep in the same location in results

        for (int i = 0; i < results.Count; i++)
        {
            verseIdsInResults[i] = results[i].Passage?.Verses.First()?.Id
                ?? throw new ArgumentNullException(nameof(Verse));
        }

        foreach (var result in results.ToList())
        {
            string resultVerseId = result.Passage?.Verses.First().Id
                ?? throw new Exception("resultVerseId was null");

            if (result.Passage.Verses.Count <= 0
                || result.Passage.Verses.First().TranslationContents?.Count <= 0
                || result.Passage.Verses.First().TranslationContents?.First().Version != requestedTranslation
                || string.IsNullOrEmpty(result.Passage.Verses.First().TranslationContents?.First().PlainText))
            {
                _logger.LogDebug("Found missing verse content from results: {Id}:{Translation}", resultVerseId, requestedTranslation);

                if (result.Passage.Reference.VerseId is null)
                {
                    _logger.LogError("Failed to insert missing verse content: {VerseId} was null", nameof(result.Passage.Reference.VerseId));

                    continue;
                }

                results.Insert(
                    verseIdsInResults.First(v => v.Value == resultVerseId).Key,
                    new SearchResult
                    {
                        Type = SearchResultType.SemanticVerse,
                        Passage = new Passage
                        {
                            Reference = result.Passage.Reference,
                            Verses = new List<Verse> {
                                new Verse(result.Passage.Reference)
                                {
                                    TranslationContents = new List<VerseTranslationContent>
                                    {
                                        new VerseTranslationContent
                                        {
                                            PlainText = await _bibleApi.GetVersePlaintext(
                                                AvailableBibles.GetBible(requestedTranslation).Id,
                                                result.Passage.Reference.VerseId)
                                        }
                                     }
                                }
                            }
                        }
                    }
                );
            }
        }
    }

    private async Task<List<SearchResult>> GetPassageSearchResults(string userSearchQuery, string requestedTranslation)
    {
        var searchResults = new List<SearchResult>();

        var searchEmbedding = await _embeddingGenerator.GenerateEmbedding(userSearchQuery);

        var result = await GetVersesSemanticSearchResults(
            searchEmbedding,
            requestedTranslation);

        foreach (var _verse in result)
        {
            searchResults.Add(new SearchResult
            {
                Type = SearchResultType.SemanticVerse,
                Passage = new Passage
                {
                    Reference = new Reference
                    {
                        Book = _verse.Reference.Book,
                        Chapter = _verse.Reference.Chapter,
                        VerseNumbers = _verse.Reference.VerseNumbers
                    },
                    Verses = new List<DataAccess.Models.Verse> { _verse }
                },
                Rank = 2
            });
        }

        await EnsureAllResultsContainContent(searchResults, requestedTranslation);

        return searchResults;
    }

    /// <summary>
    /// Gets a kjv passage for it's embeddings, used for semantic search, checking cache
    /// </summary>
    /// <param name="reference"></param>
    /// <param name="translation"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<Passage> getKjvPassageForSemanticSearch(Reference reference, string translation)
    {
        Passage passage;

        List<Verse> versesFromCache = new();
        List<string> versesNotFoundInCache = new();

        foreach (var verseId in reference.VerseIds)
        {
            var cachedVerse = await _distributedCache.GetAsync(CacheKeyGenerator.GetVerseCacheKey(verseId, translation));

            if (cachedVerse is null)
            {
                versesNotFoundInCache.Add(verseId);
            }
            else
            {
                _logger.LogDebug("Verse found in cache: {Reference}:{Translation}", verseId, translation);

                var deserializedCachedVerse = JsonSerializer.Deserialize<Verse>(cachedVerse)
                                    ?? throw new Exception("Error deserializing cached verse");

                if (deserializedCachedVerse.TranslationContents?.First().Version == translation)
                {
                    versesFromCache.Add(deserializedCachedVerse);
                }

                if (deserializedCachedVerse.TranslationContents?.First().Embedding is null)
                {
                    throw new Exception("Embedding returned null from cache.");
                }
            }
        }

        List<Verse> versesFetched = await _verseData.GetVersesFromIds(versesNotFoundInCache);

        var cacheOptions = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(CacheExpirations.VerseContentExpiration);

        foreach (var verseFetched in versesFetched)
        {
            verseFetched.TranslationContents?.ForEach(c => c.Version = translation);

            if (verseFetched.TranslationContents?.First().Embedding is null)
            {
                _logger.LogError("Could not cache verse: embedding was null");
            }

            await _distributedCache.SetStringAsync(CacheKeyGenerator.GetVerseCacheKey(verseFetched.Id, translation),
                JsonSerializer.Serialize(verseFetched),
                cacheOptions);

            _logger.LogDebug("Cached verse: {Reference}:{Translation}", verseFetched.Reference.ReadableReference, translation);
        }

        return new Passage()
        {
            Reference = reference,
            Verses = versesFromCache.Concat(versesFetched).OrderBy(v => v.Id).ToList()
        };
    }
}

 // //Verse[][] semanticResultsPerVerse = new Verse[passage.Verses.Count][];                                                                                                                                                                         
 //      106 -            //List<Verse> allVerses = new();                                                                                                                                                                                                                 
 //      107 -                                                                                                                                                                                                                                                             
 //      108 -            //for (int i = 0; i < passage.Verses.Count; i++)                                                                                                                                                                                                 
 //      109 -            //{                                                                                                                                                                                                                                              
 //      110 -            //    List<Verse> _results = await _verseData.GetVersesSemanticSearch(verseEmbeddings.ElementAt(i));                                                                                                                                             
 //      111 -                                                                                                                                                                                                                                                             
 //      112 -            //    semanticResultsPerVerse[i] = new Verse[_results.Count];                                                                                                                                                                                    
 //      113 -                                                                                                                                                                                                                                                             
 //      114 -            //    for (int j = 0; j < _results.Count; j++)                                                                                                                                                                                                   
 //      115 -            //    {                                                                                                                                                                                                                                          
 //      116 -            //        semanticResultsPerVerse[i][j] = _results[j];                                                                                                                                                                                           
 //      117 -            //        allVerses.Add(_results[j]);                                                                                                                                                                                                            
 //      118 -            //    }                                                                                                                                                                                                                                          
 //      119 -            //}                                                                                                                                                                                                                                              
 //      120 -                                                                                                                                                                                                                                                             
 //      121 -            //// Remove original passage verses if present                                                                                                                                                                                                   
 //      122 -            //List<int> passageVerseIds = (passage.Verses.Select(v => v.Id == v.Id).ToList());                                                                                                                                                               
 //      123 -            //allVerses = allVerses.Where(v => v.Id != passage.Verses.)                                                                                                                                                                                      
 //      124 -                                                                                                                                                                                                                                                             
 //      125 -            //    // TODO: COmment out all this and save for later                                                                                                                                                                                           
 //      126 -                                                                                                                                                                                                                                                             
 //      127 -            //int totalResults = semanticResultsPerVerse.Sum(row => row.Length);                                                                                                                                                                             
 //      128 -            //List<(Verse verse, float rank)> ranks = new();                                                                                                                                                                                                 
 //      129 -                                                                                                                                                                                                                                                             
 //      130 -            //float totalMemorized = allVerses.Sum(v => v.UsersMemorizedCount);                                                                                                                                                                              
 //      131 -            //float totalSaved = allVerses.Sum(v => v.UsersSavedCount);                                                                                                                                                                                      
 //      132 -                                                                                                                                                                                                                                                             
 //      133 -            //float averageMemorized = totalMemorized / allVerses.Count;                                                                                                                                                                                     
 //      134 -            //float averageSaved = totalSaved / allVerses.Count;                                                                                                                                                                                             
 //      135 -                                                                                                                                                                                                                                                             
 //      136 -            //for (int i = 0; i < allVerses.Count(); i++)                                                                                                                                                                                                    
 //      137 -            //{                                                                                                                                                                                                                                              
 //      138 -            //    float memorizedRank = allVerses[i].UsersMemorizedCount / averageMemorized;                                                                                                                                                                 
 //      139 -            //    float savedRank = allVerses[i].UsersSavedCount / averageSaved;                                                                                                                                                                             
 //      140 -                                                                                                                                                                                                                                                             
 //      141 -            //    float rank = (memorizedRank + savedRank) > 0                                                                                                                                                                                               
 //      142 -            //        ? (memorizedRank + savedRank) / 2                                                                                                                                                                                                      
 //      143 -            //        : 0;                                                                                                                                                                                                                                   
 //      144 -                                                                                                                                                                                                                                                             
 //      145 -            //    ranks.Add((allVerses[i], rank));                                                                                                                                                                                                           
 //      146 -            //}                                                                                                                                                                                                                                              
 //      147 -                                                                                                                                                                                                                                                             
 //      148 -            //int totalReturns = totalResults < MAX_RESULTS ? totalResults : MAX_RESULTS;                                                                                                                                                                    
 //      149 -                                                                                                                                                                                                                                                             
 //      150 -            //ranks.Sort((a, b) => b.rank.CompareTo(a.rank));                                                                                                                                                                                                
 //      151 -                                                                                                                                                                                                                                                             
 //      152 -            //for (int i = 0; i < totalReturns; i++)                                                                                                                                                                                                         
 //      153 -            //{                                                                                                                                                                                                                                              
 //      154 -            //    results.Add(new SearchResult                                                                                                                                                                                                               
 //      155 -            //    {                                                                                                                                                                                                                                          
 //      156 -            //        Type = SearchResultType.SemanticVerse,                                                                                                                                                                                                 
 //      157 -            //        Verse = ranks[i].verse,                                                                                                                                                                                                                
 //      158 -            //        Rank = ranks[i].rank                                                                                                                                                                                                                   
 //      159 -            //    });                                                                                                                                                                                                                                        
 //      160 -            //}    