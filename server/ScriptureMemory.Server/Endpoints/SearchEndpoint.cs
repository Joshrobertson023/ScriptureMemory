using DataAccess.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VerseAppNew.Server.Services;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace ScriptureMemory.Server.Endpoints;

public static class SearchEndpoint
{
     public static void ConfigureSearchEndpoints(this WebApplication app)
     {
         app.MapPost("search", async (
             [FromBody] SearchRequest request,
             [FromServices] SearchService service,
             ClaimsPrincipal user) =>
         {
             return await service.Search(request, user);
         });
     }
}
