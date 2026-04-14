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

    public async Task TrackSearch(DataAccess.Requests.SearchRequest request)
    {
        switch(request.SearchType)
        {
            case SearchType.Verse:
                //await 
                break;
        }
    }

    public async Task<IResult> SearchVerses(DataAccess.Requests.SearchRequest request)
    {
        // Log the search
        await logger.Log(
            new ActivityLog(
                request.UserId,
                ActionType.Search,
                EntityType.Verse,
                null,
                $"Searched for '{request.Search}'",
                null
            )
        );

        await TrackSearch(request);

        return Results.Ok();
    }

    public async Task<IResult> Search(DataAccess.Requests.SearchRequest request)
    {
        // Implement Bible searching, recent searches, save user and collection searching for later
        // Be able to return whatever search results wanted, so mix of passages and collections based on weights from semantic search
        // If search book, return top results for passages in book
        // Have AI implement semantic search

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

    private async Task<List<Verse>> GetPassageSearchResults(string search)
    {
        // If single verse search, do this normal semantic search:
        // If multiple verses / passage, get passage, then semantic results per verse

        Reference? reference = null;
        bool searchByReference = true;

        try
        {
            reference = ReferenceParser.Parse(search);
        }
        catch
        {
            searchByReference = false;
        }

        if ((searchByReference || reference is not null)
            && reference.Verses.Count > 1)
        {
            // Searching multiple verses / a passage
        }
        else
        {
            // Single verse search
            return await _verseData.GetVersesSemanticSearch(
                    new Vector(await _embeddingGenerator.GetEmbedding(search)));
        }
    }
}
