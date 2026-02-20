//using DataAccess.DataInterfaces;
//using Microsoft.AspNetCore.Mvc;

//namespace VerseAppNew.Server.Endpoints;

//public static class PopularSearchEndpoint
//{
//    public static void ConfigurePopularSearchEndpoints(this WebApplication app)
//    {
//        app.MapPost("/searches/track", TrackSearch);
//        app.MapGet("/searches/popular", GetPopularSearches).WithName("GetPopularSearches");
//    }

//    private static async Task<IResult> TrackSearch(
//        [FromQuery] string searchTerm,
//        [FromServices] ISearchData popularSearchData)
//    {
//        try
//        {
//            await popularSearchData.TrackSearch(searchTerm);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetPopularSearches(
//        int? limit,
//        [FromServices] ISearchData popularSearchData)
//    {
//        try
//        {
//            int searchLimit = limit.HasValue && limit.Value > 0 ? limit.Value : 10;
//            var searches = await popularSearchData.GetPopularSearches(searchLimit);
//            return Results.Ok(searches);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }
//}





























