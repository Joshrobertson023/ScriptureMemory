using System;
using System.Linq;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace VerseAppNew.Server.Endpoints;

public static class VerseOfDayEndpoint
{
    public static void ConfigureVerseOfDayEndpoints(this WebApplication app)
    {
        app.MapGet("/verseofday/current", GetCurrentVerseOfDay);
        app.MapGet("/verseofday/current/userverse", GetCurrentVerseOfDayAsUserVerse);
        app.MapPost("/verseofday/suggest", SuggestVerseOfDay);
        app.MapPost("/verseofday/reset-queue", ResetQueueToBeginning);
    }
}

