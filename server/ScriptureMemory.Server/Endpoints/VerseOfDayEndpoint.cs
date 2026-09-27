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
        app.MapGet("/verseofday/{translation}", async (
            [FromServices] VerseOfDayService service,
            string translation) =>
        {
            return Results.Ok(await service.GetVod(translation));
        });
    }
}

