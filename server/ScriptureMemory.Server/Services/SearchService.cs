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
        BibleRepository _bibleRepository,
        BibleApi _bibleApi,
        EmbeddingGenerator _embeddingGenerator,
        ILogger<SearchService> _logger,
        IMemoryCache _memoryCache,
        IDistributedCache _distributedCache,
        VerseDataDapper _verseData)
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

    public async Task<IResult> Search(DataAccess.Requests.SearchRequest request, ClaimsPrincipal user)
    {
        var userId = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userId))
            _logger.LogInformation("UserId not found.");

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
        var passage = await _bibleRepository.GetKjvPassageForSemanticSearch(requestedReference, requestedTranslation);

        // Add the semantically similar to the searched passage to the search results

        IEnumerable<Vector> referenceMatchVectors = passage.Verses.Select(
            v => v.TranslationContents?.First().Embedding);
        IEnumerable<Verse> versesSemanticSearchResult
            = await _bibleRepository.GetVersesSemanticSearchResults(
                referenceMatchVectors,
                passage.Verses.Select(v => v.Id).ToArray(),
                requestedTranslation);

        if (requestedTranslation != "kjv")
        {
            // Fetch verse content from api.bible
            _logger.LogInformation("Fetching from api.bible verse content.");
        }
        else
        {
            searchResults.Add(new SearchResult
            {
                Type = SearchResultType.ExactPassage,
                Passage = passage,
                Rank = 1
            });
        }

        foreach (var verse in versesSemanticSearchResult)
        {
            // if (verse.Id == passage.Verses.First().Id)
            //     continue;

            searchResults.Add(new SearchResult
            {
                Type = SearchResultType.SemanticVerse,
                Passage = new Passage
                {
                    Reference = new Reference
                    {
                        Book = verse.Reference.Book,
                        Chapter = verse.Reference.Chapter,
                        VerseNumbers = verse.Reference.VerseNumbers
                    },
                    Verses = new List<DataAccess.Models.Verse> { verse }
                },
                Rank = 2
            });
        }

        return await EnsureAllResultsContainContent(searchResults, requestedTranslation);
    }

    /// <summary>
    /// Double checks all search results, ensuring the correct translation, and that the plain text is in each verse
    /// </summary>
    /// <param name="results"></param>
    /// <param name="requestedTranslation"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private async Task<List<SearchResult>> EnsureAllResultsContainContent(List<SearchResult> results, string requestedTranslation)
    {
        Dictionary<int, string> verseIdsInResults = new(); // Track verses and index found to keep in the same location in results

        for (int i = 0; i < results.Count; i++)
        {
            verseIdsInResults[i] = results[i].Verse?.Id
                ?? throw new ArgumentNullException(nameof(Verse));
        }

        foreach (var result in results)
        {
            string resultVerseId = result.Passage.Verses.First().Id;

            if (result.Passage.Verses.Count <= 0
                || result.Passage.Verses.First().TranslationContents.Count <= 0
                || result.Passage.Verses.First().TranslationContents.First().Version != requestedTranslation
                || string.IsNullOrEmpty(result.Passage.Verses.First().TranslationContents.First().PlainText))
            {
                _logger.LogInformation("Found missing verse content from results: {Id}", resultVerseId);

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

        return results;
    }

    private async Task<List<SearchResult>> GetPassageSearchResults(string userSearchQuery, string requestedTranslation)
    {
        var searchResults = new List<SearchResult>();

        var searchEmbedding = await _embeddingGenerator.GenerateEmbedding(userSearchQuery);

        var result = await _bibleRepository.GetVersesSemanticSearchResults(
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

        return await EnsureAllResultsContainContent(searchResults, requestedTranslation);
    }

    /// <summary>
    /// Gets a kjv passage for it's embeddings, used for semantic search, checking cache
    /// </summary>
    /// <param name="reference"></param>
    /// <param name="translation"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<Passage> GetKjvPassageForSemanticSearch(Reference reference, string translation)
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
                _logger.LogInformation("Verse found in cache: {Reference}", verseId);

                var deserializedCachedVerse = JsonSerializer.Deserialize<Verse>(cachedVerse)
                                    ?? throw new Exception("Error deserializing cached verse");

                if (deserializedCachedVerse.TranslationContents.First().Version == translation)
                {
                    versesFromCache.Add(deserializedCachedVerse);
                }

            }
        }

        List<Verse> versesFetched = await _verseData.GetVersesFromIds(versesNotFoundInCache);

        var cacheOptions = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(CacheExpirations.VerseContentExpiration);

        foreach (var verseFetched in versesFetched)
        {
            verseFetched.TranslationContents.ForEach(c => c.Version = translation);

            await _distributedCache.SetStringAsync(CacheKeyGenerator.GetVerseCacheKey(verseFetched.Id, translation),
                JsonSerializer.Serialize(verseFetched),
                cacheOptions);

            _logger.LogInformation("Cached verse: {Reference}", verseFetched.Reference.ReadableReference);
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