using DataAccess.Data;
using DataAccess.Models;
using System.DirectoryServices.Protocols;
using static ScriptureMemory.Server.Tools.Enums;

namespace VerseAppNew.Server.Services;

public sealed class SearchService
{
    private readonly ActivityLogger logger;
    private readonly UserData userContext;

    public SearchService(ActivityLogger logger, UserData userContext)
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
}
