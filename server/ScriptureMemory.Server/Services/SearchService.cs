using Azure.Core;
using DataAccess.Data;
using DataAccess.Models;
using Pgvector;
using ScriptureMemory.Server.DataAccess.Models;
using ScriptureMemory.Server.Files.CsvRecordModels;
using ScriptureMemory.Server.Services;
using ScriptureMemory.Server.Tools;
using System.DirectoryServices.Protocols;
using static ScriptureMemory.Server.Tools.Enums;

namespace VerseAppNew.Server.Services;

public sealed class SearchService
{
    private readonly ActivityLogger logger;
    private readonly UserData userContext;
    private readonly VerseData _verseData;
    private readonly EmbeddingGenerator _embeddingGenerator;

    public SearchService(
        ActivityLogger logger, 
        UserData userContext,
        VerseData verseData,
        EmbeddingGenerator embeddingGenerator)
    {
        this.logger = logger;
        this.userContext = userContext;
        _verseData = verseData;
        _embeddingGenerator = embeddingGenerator;
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

    public async Task<IResult> Search(DataAccess.Requests.SearchRequest request)
    {
        // Implement Bible searching, recent searches, save user and collection searching for later
        // Be able to return whatever search results wanted, so mix of passages and collections based on weights from semantic search
        // If search book, return top results for passages in book
        // Use AI/embeddings to implement semantic search

        // before this:
            // Figure out style for categories on passages
            // Clicking on any passage loads metadata for passage (categories, cross references paginated, in your lists, num saved, etc)
            // 

        if (request.SearchType == SearchType.Passage)
        {
            return Results.Ok(await GetPassageSearchResults(request.Search));
        }
        else
        {
            return Results.Problem("Not implemented");
        }
    }

    private async Task<List<SearchResult>> GetPassageSearchResults(string search)
    {
        // If single verse search, do this normal semantic search:
        // If multiple verses / passage, get passage, then semantic results per verse

        const int MAX_RESULTS = 50;

        Reference? reference = null;
        bool searchByReference = true;
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
            var passage = await _verseData.GetPassage(reference);
            passage.Reference = reference;

            results.Add(new SearchResult
            {
                Type = SearchResultType.ExactPassage,
                Passage = passage,
                Rank = 1
            });

            var passageResults = await _verseData.GetVersesSemanticSearch(
                await _embeddingGenerator.GenerateEmbeddings(
                    passage.Verses.Select(v => v.GetEmbeddingText()).ToList()));

            //List<Vector> verseEmbeddings = await _embeddingGenerator.GenerateEmbeddings(passage.Verses.Select(v => v.GetEmbeddingText()).ToList());

            //Verse[][] semanticResultsPerVerse = new Verse[passage.Verses.Count][];
            //List<Verse> allVerses = new();

            //for (int i = 0; i < passage.Verses.Count; i++)
            //{
            //    List<Verse> _results = await _verseData.GetVersesSemanticSearch(verseEmbeddings.ElementAt(i));

            //    semanticResultsPerVerse[i] = new Verse[_results.Count];

            //    for (int j = 0; j < _results.Count; j++)
            //    {
            //        semanticResultsPerVerse[i][j] = _results[j];
            //        allVerses.Add(_results[j]);
            //    }
            //}

            //// Remove original passage verses if present
            //List<int> passageVerseIds = (passage.Verses.Select(v => v.Id == v.Id).ToList());
            //allVerses = allVerses.Where(v => v.Id != passage.Verses.)

            //    // TODO: COmment out all this and save for later

            //int totalResults = semanticResultsPerVerse.Sum(row => row.Length);
            //List<(Verse verse, float rank)> ranks = new();

            //float totalMemorized = allVerses.Sum(v => v.UsersMemorizedCount);
            //float totalSaved = allVerses.Sum(v => v.UsersSavedCount);

            //float averageMemorized = totalMemorized / allVerses.Count;
            //float averageSaved = totalSaved / allVerses.Count;

            //for (int i = 0; i < allVerses.Count(); i++)
            //{
            //    float memorizedRank = allVerses[i].UsersMemorizedCount / averageMemorized;
            //    float savedRank = allVerses[i].UsersSavedCount / averageSaved;

            //    float rank = (memorizedRank + savedRank) > 0
            //        ? (memorizedRank + savedRank) / 2
            //        : 0;

            //    ranks.Add((allVerses[i], rank));
            //}

            //int totalReturns = totalResults < MAX_RESULTS ? totalResults : MAX_RESULTS;

            //ranks.Sort((a, b) => b.rank.CompareTo(a.rank));

            //for (int i = 0; i < totalReturns; i++)
            //{
            //    results.Add(new SearchResult
            //    {
            //        Type = SearchResultType.SemanticVerse,
            //        Verse = ranks[i].verse,
            //        Rank = ranks[i].rank
            //    });
            //}

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
                            VerseNumbers = _passage.Reference.Verses,
                            ReadableReference = ReferenceParser.ConvertToReadableReference(
                                _passage.Reference.Book,
                                _passage.Reference.Chapter,
                                _passage.Reference.Verses)
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

            var singleVerseSearchResults = await _verseData.GetVersesSemanticSearch(embedding);

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
                            VerseNumbers = verse.Reference.Verses,
                            ReadableReference = ReferenceParser.ConvertToReadableReference(
                                verse.Reference.Book,
                                verse.Reference.Chapter,
                                verse.Reference.Verses)
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
