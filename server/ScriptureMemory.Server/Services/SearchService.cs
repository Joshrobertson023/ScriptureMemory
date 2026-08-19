using DataAccess.Data;
using DataAccess.Models;
using Microsoft.Extensions.Caching.Memory;
using Pgvector;
using ScriptureMemory.Server.CustomExceptions;
using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.DataAccess.Models;
using ScriptureMemory.Server.Services;
using ScriptureMemory.Server.Tools;
using System.Security.Claims;
using static ScriptureMemory.Server.Tools.Enums;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace VerseAppNew.Server.Services;

public sealed class SearchService
{
    // ActivityLogger is left out for now -- see conversation notes; it doesn't compile
    // as of this refactor (references a nonexistent AdminData class).
    // private readonly ActivityLogger logger;
    private readonly IUserData _userData;
    private readonly UserDataDapper _userDataDapper;
    private readonly UserDataEFCore _userDataEfCore;
    private readonly BibleRepository _bibleRepository;
    private readonly EmbeddingGenerator _embeddingGenerator;
    private readonly ILogger<SearchService> _logger;
    private readonly IMemoryCache _memoryCache;

    public SearchService(
        // ActivityLogger logger,
        IUserData userData,
        UserDataDapper userDataDapper,
        UserDataEFCore userDataEfCore,
        BibleRepository bibleRepository,
        EmbeddingGenerator embeddingGenerator,
        ILogger<SearchService> logger,
        IMemoryCache memoryCache)
    {
        // this.logger = logger;
        _userData = userData;
        _userDataDapper = userDataDapper;
        _userDataEfCore = userDataEfCore;
        _bibleRepository = bibleRepository;
        _embeddingGenerator = embeddingGenerator;
        _logger = logger;
        _memoryCache = memoryCache;
    }

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

    private async Task<List<SearchResult>> GetReferenceSearchResults(string translation, Reference reference)
    {
        var results = new List<SearchResult>();
        
        // Get the exact passage searched
        var passage = await _bibleRepository.GetKjvPassageForSemanticSearch(reference);

        results.Add(new SearchResult
        {
            Type = SearchResultType.ExactPassage,
            Passage = passage,
            Rank = 1
        });

        // Add the semantically similar to the searched passage to the search results
        
        var embeddingTexts = passage.Verses
            .SelectMany(v => v.TranslationContents ?? new List<VerseTranslationContent>())
            .Select(c => c.GetEmbeddingText())
            .Where(t => !string.IsNullOrEmpty(t))
            .Select(t => t!)
            .ToList();
        
        var versesResult = embeddingTexts.Count > 0
            ? await _bibleRepository.GetVersesSemanticSearchResults(
                await _embeddingGenerator.GenerateEmbeddings(embeddingTexts),
                translation)
            : new List<Verse>();

        if (translation != "kjv")
        {
            // Fetch verse content from api.bible
            _logger.LogInformation("Fetching from api.bible verse content.");
        }

        foreach (var verse in versesResult)
        {
            // if (verse.Id == passage.Verses.First().Id)
            //     continue;
            
            results.Add(new SearchResult
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

        return results;
    }

    private async Task<List<SearchResult>> GetPassageSearchResults(string search, string translation)
    {
        var searchResults = new List<SearchResult>();

        var searchEmbedding = await _embeddingGenerator.GenerateEmbedding(search);

        var result = await _bibleRepository.GetVersesSemanticSearchResults(searchEmbedding, translation);

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

        return searchResults;
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