using DataAccess.DataInterfaces;
using DataAccess.Models;
using System.DirectoryServices.Protocols;
using static VerseAppLibrary.Enums;

namespace VerseAppNew.Server.Services;

public interface ISearchService
{
    Task<IResult> SearchVerses(DataAccess.Requests.SearchRequest request);
}

public sealed class SearchService : ISearchService
{
    private readonly IActivityLogger logger;
    private readonly IUserData userContext;

    public SearchService(IActivityLogger logger, IUserData userContext)
    {
        this.logger = logger;
        this.userContext = userContext;
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
                request.Username,
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
}
