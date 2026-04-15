using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using ScriptureMemory.Server.DataAccess.Data;
using ScriptureMemory.Server.DataAccess.Models;
using ScriptureMemory.Server.DataAccess.Requests;
using ScriptureMemory.Server.Tools;

namespace VerseAppNew.Server.Endpoints;

public static class CategoriesEndpoint
{
    public static void ConfigureCategoriesEndpoints(this WebApplication app)
    {
        app.MapPost("admin/category/create", async (
            [FromBody] CreateCategoryRequest request,
            [FromServices] CategoryData data,
            [FromServices] CategoryService service,
            [FromServices] EmbeddingGenerator embeddingGenerator) =>
        {
            // Todo: Generate an embedding for each sentence in the category description
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
            };
            category.Embedding = await embeddingGenerator.GenerateEmbedding(category.GetEmbeddingText());
            category.Id = await data.CreateCategory(category);
            await service.AssignVersesToCategory(category);
            return Results.Ok(category.Id);
        }).RequireAuthorization("Admin");

        app.MapGet("categories", async (
            [FromServices] CategoryData data) =>
        {
            var categories = await data.GetCategories();
            return Results.Ok(categories);
        });

        app.MapGet("categories/{categoryId:int}/verses", async (
            [FromRoute] int categoryId,
            [FromServices] CategoryData data) =>
        {
            var verses = await data.GetVersesInCategory(categoryId);
            return Results.Ok(verses);
        });

        app.MapPost("categories/assign", async (
            [FromBody] AssignVerseCategoryRequest request,
            [FromServices] CategoryData data) =>
        {
            await data.AssignVerseToCategory(new VerseCategory
            {
                VerseId = request.VerseId,
                CategoryId = request.CategoryId,
                AssignmentSource = request.AssignmentSource,
                Confidence = request.Confidence,
            });

            return Results.Ok();
        }).RequireAuthorization("Admin");

        app.MapPost("categories/unassign", async (
            [FromBody] UnassignVerseCategoryRequest request,
            [FromServices] CategoryData data) =>
        {
            await data.UnassignVerseToCategory(request.VerseId, request.CategoryId);
            return Results.Ok();
        }).RequireAuthorization("Admin");

        app.MapPost("categories/delete", async (
            [FromBody] int categoryId,
            [FromServices] CategoryData data) =>
        {
            await data.DeleteCategory(categoryId);
            return Results.Ok();
        }).RequireAuthorization("Admin");
    }

    public sealed record AssignVerseCategoryRequest(
        int VerseId,
        int CategoryId,
        string AssignmentSource,
        float Confidence);

    public sealed record UnassignVerseCategoryRequest(
        int VerseId,
        int CategoryId);
}
