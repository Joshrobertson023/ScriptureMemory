using System;
using System.Linq;
using DataAccess.Data;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using ScriptureMemory.Server.DataAccess.Requests;
using ScriptureMemory.Server.Services;

namespace VerseAppNew.Server.Endpoints;

public static class VerseOfDayEndpoint
{
    public static void ConfigureVerseOfDayEndpoints(this WebApplication app)
    {
        app.MapGet("/verseofday", async (
            [FromServices] VerseOfDayData verseOfDayData) =>
        {
            return Results.Ok(await verseOfDayData.GetActive());
        });

        app.MapPost("/verseofday/paged", async (
            [FromBody] GetVerseOfDaysRequest request,
            [FromServices] VerseOfDayData data) =>
        {
            return Results.Ok(await data.GetVods(request.Page, request.Offset));
        });

        app.MapPost("/verseofday", async (
            [FromBody] InsertVerseOfDayRequest request,
            [FromServices] VerseOfDayService service) =>
        {
            return Results.Ok(await service.InsertVod(request.Reference, request.AdminId));
        });
    }
}

