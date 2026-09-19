using Dapper;
using DataAccess.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using ScriptureMemory.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Data;

public class VerseOfDayData
{
    private readonly ApplicationDbContext _context;

    public VerseOfDayData(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task InsertVod(VerseOfDay verseOfDay)
    {
        var verseOfDayReference = new Reference(verseOfDay.Reference);

        var existingPassage = _context.Passages.AsNoTracking().FirstOrDefault(p => p.Id == verseOfDayReference.VerseId);

        Passage newPassage;

        if (existingPassage == null)
        {
            List<string> verseIds = verseOfDayReference.VerseIds.ToList();

            var verseOfDayVerses = _context.Verses.Where(v => verseIds.Contains(v.Id)).ToList();

            newPassage = new Passage
            {
                Id = verseOfDayReference.VerseId,
                Reference = verseOfDayReference,
                Verses = verseOfDayVerses
            };

            await _context.Passages.AddAsync(newPassage);

            verseOfDay.PassageNavigation = newPassage;
            verseOfDay.PassageId = newPassage.Id;
        }
        else
        {
            verseOfDay.PassageNavigation = existingPassage;
            verseOfDay.PassageId = existingPassage.Id;
        }

        await _context.VerseOfDays.AddAsync(verseOfDay);
        await _context.SaveChangesAsync();
    }

    public async Task<VerseOfDay?> GetVod()
    {
        var result = await _context.VerseOfDays
            .AsNoTracking()
            .Include(v => v.PassageNavigation)
                .ThenInclude(p => p.Verses)
            .Where(vod => vod.PassageId == vod.PassageId)
            .OrderByDescending(v => v.Date)
            .FirstOrDefaultAsync();

        if (result is null)
        {
            return null;
        }

        return result.Date == DateTime.Now.Date ? result : null;
    }
}
