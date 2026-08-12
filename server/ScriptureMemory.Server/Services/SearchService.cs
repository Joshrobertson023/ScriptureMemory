using DataAccess.Data;
using DataAccess.Models;
using Pgvector;
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

    public SearchService(
        // ActivityLogger logger,
        IUserData userData,
        UserDataDapper userDataDapper,
        UserDataEFCore userDataEfCore,
        BibleRepository bibleRepository,
        EmbeddingGenerator embeddingGenerator,
        ILogger<SearchService> logger)
    {
        // this.logger = logger;
        _userData = userData;
        _userDataDapper = userDataDapper;
        _userDataEfCore = userDataEfCore;
        _bibleRepository = bibleRepository;
        _embeddingGenerator = embeddingGenerator;
        _logger = logger;
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
        
        _logger.LogInformation($"User #{userId} has searched for \"{request.Search}\"");
        
        if (string.IsNullOrEmpty(userId))
            _logger.LogInformation("UserId not found.");
        
        // Implement Bible searching, recent searches, save user and collection searching for later
        // Be able to return whatever search results wanted, so mix of passages and collections based on weights from semantic search
        // If search book, return top results for passages in book
        // Use AI/embeddings to implement semantic search

        // before this:
            // Figure out style for categories on passages
            // Clicking on any passage loads metadata for passage (categories, cross references paginated, in your lists, num saved, etc)
            //

        return Results.Ok(await GetPassageSearchResults(request.Search, request.Translation));
    }

    private async Task<List<SearchResult>> GetPassageSearchResults(string search, string translation)
    {
        // If single verse search, do this normal semantic search:
        // If multiple verses / passage, get passage, then semantic results per verse

        Reference? reference = null;
        var results = new List<SearchResult>();

        try
        {
            reference = ReferenceParser.Parse(search);
        }
        catch
        {
            reference = null;
            // Don't search by reference
        }

        if (reference is not null)
        {
            // Searching multiple verses / a passage
            var passage = await _bibleRepository.GetPassage(reference, translation);

            results.Add(new SearchResult
            {
                Type = SearchResultType.ExactPassage,
                Passage = passage,
                Rank = 1
            });

            var embeddingTexts = passage.Verses
                .SelectMany(v => v.TranslationContents ?? new List<VerseTranslationContent>())
                .Select(c => c.GetEmbeddingText())
                .Where(t => !string.IsNullOrEmpty(t))
                .Select(t => t!)
                .ToList();

            var passageResults = embeddingTexts.Count > 0
                ? await _bibleRepository.GetVersesSemanticSearch(
                    await _embeddingGenerator.GenerateEmbeddings(embeddingTexts),
                    translation)
                : new List<Verse>();

            foreach (var _passage in passageResults)
            {
                results.Add(new SearchResult
                {
                    Type = SearchResultType.SemanticVerse,
                    Passage = new Passage
                    {
                        Reference = new Reference
                        {
                            Book = _passage.Reference.Book,
                            Chapter = _passage.Reference.Chapter,
                            VerseNumbers = _passage.Reference.VerseNumbers
                            // ReadableReference is left unset -- Reference.ReadableReference lazily
                            // computes it from Book/Chapter/VerseNumbers on first access.
                        },
                        Verses = new List<DataAccess.Models.Verse> { _passage }
                    },
                    Rank = 2
                });
            }

            return results;
        }
        else
        {
            var embedding = await _embeddingGenerator.GenerateEmbedding(search);

            var singleVerseSearchResults = await _bibleRepository.GetVersesSemanticSearch(embedding, translation);

            foreach (var verse in singleVerseSearchResults)
            {
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
                            // ReadableReference is left unset -- Reference.ReadableReference lazily
                            // computes it from Book/Chapter/VerseNumbers on first access.
                        },
                        Verses = new List<DataAccess.Models.Verse> { verse }
                    },
                    Rank = 1
                });
            }

            return results
                .OrderByDescending(r => r.Rank)
                .ToList();
        }
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