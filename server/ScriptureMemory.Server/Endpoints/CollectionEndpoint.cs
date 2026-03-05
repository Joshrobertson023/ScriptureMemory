using Azure.Core;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Xml;
using VerseAppNew.Server.Services;
using ScriptureMemoryLibrary;
using DataAccess.DataInterfaces;
using ScriptureMemory.Server.Services;
using DataAccess.Requests.UpdateRequests;

namespace VerseAppNew.Server.Endpoints;

public static class CollectionEndpoint
{
    public static void ConfigureCollectionEndpoints(this WebApplication app)
    {
        app.MapPost("/collections", async (
            [FromBody] Collection newCollection,
            [FromServices] ICollectionService collectionService) =>
        {
            int newCollectionId = await collectionService.CreateCollection(newCollection);
            return Results.Ok(newCollectionId);
        });

        app.MapPut("/collections", async (
            [FromBody] UpdateCollectionRequest request,
            [FromServices] ICollectionService collectionService) =>
        {
            await collectionService.UpdateCollection(request);
            return Results.NoContent();
        });
    }
}
