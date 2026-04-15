using Azure.Core;
using DataAccess.Data;
using DataAccess.Models;
using Pgvector;
using ScriptureMemory.Server.DataAccess.Models;
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
            // Don't search by reference
        }

        if (reference is not null
            && reference.Verses.Count > 1)
        {
            // Searching multiple verses / a passage
            var passage = await _verseData.GetPassage(reference);

            results.Add(new SearchResult
            {
                Type = SearchResultType.ExactPassage,
                Passage = passage,
                Rank = 1
            });

            List<Vector> verseEmbeddings = await _embeddingGenerator.GenerateEmbeddings(passage.Verses.Select(v => v.GetEmbeddingText()).ToList());

            Verse[][] semanticResultsPerVerse = new Verse[passage.Verses.Count][];
            int totalResults = semanticResultsPerVerse.Sum(row => row.Length);
            List<Verse> allResults = new();
            Dictionary<float, Verse[]> ranks = new();

            for (int i = 0; i < passage.Verses.Count; i++)
            {
                List<Verse> _results = await _verseData.GetVersesSemanticSearch(verseEmbeddings.ElementAt(i));

                for (int j = 0; j < _results.Count; j++)
                {
                    semanticResultsPerVerse[i][j] = _results[j];
                    allResults.Add(_results[j]);
                }

                // Todo: compute the ranks here
            }

            // Rank by num of semantic results per row, and num saved / memorized, later refactor to include others

            foreach (var result in allResults)
            {
                // First rank is by number of results for the Verse compared to everything else
                double averageLength = semanticResultsPerVerse.Average(row => row.Length);
                double resultsRank = rank.Value.Length / averageLength;

                double totalAverageMemorized = semanticResultsPerVerse.Average(row => row.Average(v => v.UsersMemorizedCount));
                double totalAverageSaved = semanticResultsPerVerse.Average(row => row.Average(v => v.UsersSavedCount));

                double averageMemorized = rank.Value.Average(v => v.UsersMemorizedCount);
                double averageSaved = rank.Value.Average(v => v.UsersSavedCount);

                double memorizedRank = averageMemorized / totalAverageMemorized;
                double savedRank = averageSaved / totalAverageSaved;

                 = (float)(memorizedRank + savedRank) / 2;
            }



            int totalReturns = totalResults < MAX_RESULTS ? totalResults : MAX_RESULTS;

            for (int i = 0; i < totalReturns; i++)
            {

            }



            for (int i = 0; i < semanticResultsPerVerse.Count; i++)
            {
                results.Add(new SearchResult
                {
                    Type = SearchResultType.SemanticVerse,
                    Verse = semanticResultsPerVerse[i],
                    Rank = 2
                });
            }

            return results
                .OrderByDescending(r => r.Rank)
                .ToList();
        }
        else
        {
            // Single verse search
            var passage = await _verseData.GetPassage(reference);

            results.Add(new SearchResult
            {
                Type = SearchResultType.ExactPassage,
                Passage = passage,
                Rank = 2
            });

            var embedding = await _embeddingGenerator.GenerateEmbedding(passage.Verses.First().GetEmbeddingText());

            var singleVerseSearchResults = await _verseData.GetVersesSemanticSearch(embedding);

            foreach (var verse in singleVerseSearchResults)
            {
                if (verse.Id == passage.Verses.First().Id)
                    continue;

                results.Add(new SearchResult
                {
                    Type = SearchResultType.SemanticVerse,
                    Verse = verse,
                    Rank = 1
                });
            }

            return results
                .OrderByDescending(r => r.Rank)
                .ToList();
        }
    }
}
