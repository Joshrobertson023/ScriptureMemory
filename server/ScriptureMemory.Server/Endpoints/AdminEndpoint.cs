using System;
using System.Linq;
using DataAccess.Data;
using DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ScriptureMemory.Server.DataAccess.Data;
using ScriptureMemory.Server.DataAccess.Requests;
using ScriptureMemory.Server.DataAccess.Requests.UpdateRequests;
using ScriptureMemory.Server.Services;
using ScriptureMemory.Server.Tools;
using VerseAppNew.Server.Services;

namespace VerseAppNew.Server.Endpoints;

public static class AdminEndpoint
{
    public static void ConfigureAdminEndpoints(this WebApplication app)
    {
        app.MapPost("admin", async (
            [FromBody] CreateAdminRequest request,
            [FromServices] AdminData data) =>
        {
            var result = await data.InsertAdmin(new Admin
            {
                AdminEmail = request.AdminEmail,
                Role = request.Role,
            });
            return Results.Ok(result);
        }).RequireAuthorization("SuperAdmin");

        app.MapPost("admin/login", async (
            [FromBody] AdminLoginRequest request,
            [FromServices] AdminService service) =>
        {
            return await service.Login(request.Username, request.Password!);
        });

        app.MapPut("admin/password", async (
            [FromBody] UpdateAdminPasswordRequest request,
            [FromServices] AdminData data) =>
        {
            var passwordHasher = new PasswordHasher<Admin>();
            var hashedPassword = passwordHasher.HashPassword(null, request.NewPassword);
            await data.UpdatePassword(request.AdminId, hashedPassword);
            return Results.Ok();
        }).RequireAuthorization("SuperAdmin");

        app.MapPost("admin/category/create", async (
            [FromBody] CreateCategoryRequest request,
            [FromServices] CategoryData data,
            [FromServices] CategoryService service,
            [FromServices] EmbeddingGenerator embeddingGenerator) =>
        {
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
            };
            category.Embedding = await embeddingGenerator.GenerateEmbedding(category.GetEmbeddingText());
            var result = await data.CreateCategory(category);
            await service.AssignVersesToNewCategory(category);
            return Results.Ok(result);
        }).RequireAuthorization("SuperAdmin");
    }
}