//using DataAccess.DataInterfaces;
//using Microsoft.AspNetCore.Mvc;
//using ScriptureMemoryLibrary;

//namespace VerseAppNew.Server.Endpoints;

//public static class CategoryEndpoint
//{
//    public static void ConfigureCategoryEndpoints(this WebApplication app)
//    {
//        app.MapGet("/categories", GetAllCategories);
//        app.MapGet("/categories/top/{limit:int}", GetTopCategories);
//        app.MapPost("/categories", CreateCategory);
//        app.MapDelete("/categories/{categoryId:int}", DeleteCategory);
//        app.MapGet("/categories/{categoryId:int}/collections", GetCollectionsForCategory);
//        app.MapPost("/collections/{collectionId:int}/publish", PublishCollection);
//        app.MapDelete("/collections/{collectionId:int}/publish", UnpublishCollection);
//        app.MapGet("/collections/{collectionId:int}/publish", GetPublishedInfo);
//        app.MapPut("/collections/{collectionId:int}/categories", SetCategoriesForCollection);
//        app.MapGet("/collections/{collectionId:int}/categories", GetCategoriesForCollection);
//        app.MapGet("/categories/verses/{categoryId}/{top}", GetAllVersesForCategory);
//        app.MapGet("/categories/{categoryId:int}/verses", GetCategoryVerses);
//        app.MapPost("/categories/{categoryId:int}/verses", AddVerseToCategory);
//        app.MapDelete("/categories/{categoryId:int}/verses", DeleteVerseFromCategory);
//    }

//    private static async Task<IResult> GetAllVersesForCategory(
//        int categoryId,
//        int top,
//        [FromServices] ICategoryData categoryData,
//        [FromServices] IVerseData verseData)
//    {
//        try
//        {
//            var verseResults = await verseData.GetTopVersesInCategory(top, categoryId);
//            return Results.Ok(verseResults);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetAllCategories([FromServices] ICategoryData categoryData)
//    {
//        try
//        {
//            var cats = await categoryData.GetAll();
//            return Results.Ok(cats);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetTopCategories(
//        int limit,
//        [FromServices] ICategoryData categoryData)
//    {
//        try
//        {
//            if (limit <= 0)
//            {
//                limit = 8; // Default to 8 if invalid limit
//            }
//            var cats = await categoryData.GetTop(limit);
//            return Results.Ok(cats);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private sealed record CreateCategoryRequest(string Name);

//    private static async Task<IResult> CreateCategory(
//        [FromBody] CreateCategoryRequest request,
//        [FromServices] ICategoryData categoryData)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(request.Name))
//            {
//                return Results.BadRequest("Name is required");
//            }
//            await categoryData.Create(request.Name.Trim());
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> DeleteCategory(
//        int categoryId,
//        [FromServices] ICategoryData categoryData)
//    {
//        try
//        {
            
//            await categoryData.Delete(categoryId);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetCollectionsForCategory(
//        int categoryId,
//        [FromServices] ICollectionData collectionData)
//    {
//        try
//        {
//            var collections = await collectionData.GetPublishedCollectionsByCategory(categoryId);
//            return Results.Ok(collections);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private sealed record PublishRequest(string? Description, int[] CategoryIds);

//    private static async Task<IResult> PublishCollection(
//        int collectionId,
//        [FromBody] PublishRequest request,
//        [FromServices] IPublishedCollectionData publishData,
//        [FromServices] ICategoryData categoryData)
//    {
//        try
//        {
//            await publishData.Publish(collectionId, request.Description);
//            if (request.CategoryIds != null && request.CategoryIds.Length > 0)
//            {
//                await categoryData.SetCategoriesForCollection(collectionId, request.CategoryIds);
//            }
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> UnpublishCollection(
//        int collectionId,
//        [FromServices] IPublishedCollectionData publishData)
//    {
//        try
//        {
//            await publishData.Unpublish(collectionId);
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetPublishedInfo(
//        int collectionId,
//        [FromServices] IPublishedCollectionData publishData)
//    {
//        try
//        {
//            var info = await publishData.Get(collectionId);
//            if (info == null) return Results.NotFound();
//            return Results.Ok(info);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private sealed record SetCategoriesRequest(int[] CategoryIds);

//    private static async Task<IResult> SetCategoriesForCollection(
//        int collectionId,
//        [FromBody] SetCategoriesRequest request,
//        [FromServices] ICategoryData categoryData)
//    {
//        try
//        {
//            await categoryData.SetCategoriesForCollection(collectionId, request.CategoryIds ?? Array.Empty<int>());
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetCategoriesForCollection(
//        int collectionId,
//        [FromServices] ICategoryData categoryData)
//    {
//        try
//        {
//            var ids = await categoryData.GetCategoryIdsForCollection(collectionId);
//            return Results.Ok(ids);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private static async Task<IResult> GetCategoryVerses(
//        int categoryId,
//        [FromServices] ICategoryData categoryData)
//    {
//        try
//        {
//            var verses = await categoryData.GetVersesInCategory(categoryId);
//            return Results.Ok(verses);
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private sealed record AddVerseRequest(string VerseReference);

//    private static async Task<IResult> AddVerseToCategory(
//        int categoryId,
//        [FromBody] AddVerseRequest request,
//        [FromServices] ICategoryData categoryData)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(request.VerseReference))
//            {
//                return Results.BadRequest("Verse reference is required");
//            }

//            // Normalize the verse reference and get all individual verses
//            var normalizedReferences = NormalizeVerseReference(request.VerseReference.Trim());
//            if (normalizedReferences == null || normalizedReferences.Count == 0)
//            {
//                return Results.BadRequest("Invalid verse reference format");
//            }

//            // Add all individual verses from the reference (handles ranges)
//            foreach (var normalizedReference in normalizedReferences)
//            {
//                await categoryData.AddVerseToCategory(categoryId, normalizedReference);
//            }
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }

//    private sealed record DeleteVerseRequest(string VerseReference);

//    private static async Task<IResult> DeleteVerseFromCategory(
//        int categoryId,
//        [FromBody] DeleteVerseRequest request,
//        [FromServices] ICategoryData categoryData)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(request.VerseReference))
//            {
//                return Results.BadRequest("Verse reference is required");
//            }

//            await categoryData.DeleteVerseFromCategory(categoryId, request.VerseReference.Trim());
//            return Results.Ok();
//        }
//        catch (Exception ex)
//        {
//            return Results.Problem(ex.Message);
//        }
//    }


//    // MOVE TO REFERENCE PARSE
//    private static List<string> NormalizeVerseReference(string reference)
//    {
//        if (string.IsNullOrWhiteSpace(reference))
//            return new List<string>();

//        reference = reference.Trim();
        
//        // Remove extra spaces
//        reference = System.Text.RegularExpressions.Regex.Replace(reference, @"\s+", " ");
        
//        // Ensure space before colon if missing
//        reference = System.Text.RegularExpressions.Regex.Replace(reference, @"(\d+):", "$1 :");
        
//        // Remove space after colon
//        reference = reference.Replace(": ", ":");
        
//        // Try to parse and normalize using ReferenceParse to get all individual verses
//        try
//        {
//            var individualVerses = ReferenceParse.GetReferencesFromVersesInReference(reference);
//            if (individualVerses != null && individualVerses.Count > 0)
//            {
//                return individualVerses;
//            }
//        }
//        catch
//        {
//            // If parsing fails, try basic normalization
//        }

//        // Basic normalization: ensure format is "Book Chapter:Verse"
//        var match = System.Text.RegularExpressions.Regex.Match(reference, @"^(.+?)\s+(\d+)\s*:\s*(\d+(?:-\d+)?)$");
//        if (match.Success)
//        {
//            var book = match.Groups[1].Value.Trim();
//            var chapter = match.Groups[2].Value.Trim();
//            var verse = match.Groups[3].Value.Trim();
            
//            // If it's a range, expand it
//            if (verse.Contains('-'))
//            {
//                var parts = verse.Split('-');
//                if (parts.Length == 2 && int.TryParse(parts[0], out int start) && int.TryParse(parts[1], out int end))
//                {
//                    var result = new List<string>();
//                    for (int i = start; i <= end; i++)
//                    {
//                        result.Add($"{book} {chapter}:{i}");
//                    }
//                    return result;
//                }
//            }
            
//            return new List<string> { $"{book} {chapter}:{verse}" };
//        }

//        // Return as single-item list if we can't normalize
//        return new List<string> { reference };
//    }
//}


