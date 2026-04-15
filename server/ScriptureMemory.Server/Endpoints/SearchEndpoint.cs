using DataAccess.Requests;
using Microsoft.AspNetCore.Mvc;
using VerseAppNew.Server.Services;

namespace ScriptureMemory.Server.Endpoints;

public static class SearchEndpoint
{
    public static void ConfigureSearchEndpoints(this WebApplication app)
    {
        app.MapPost("search", async (
            [FromBody] SearchRequest request,
            [FromServices] SearchService service) =>
        {
            return await service.Search(request);
        });
    }
}
