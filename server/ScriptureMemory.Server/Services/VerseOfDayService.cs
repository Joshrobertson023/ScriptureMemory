using DataAccess.Data;
using DataAccess.Models;
using Microsoft.Extensions.Caching.Distributed;
using ScriptureMemory.Server.Data.Models.Vod;
using ScriptureMemory.Server.DataAccess.Models;
using ScriptureMemory.Server.Tools;
using System.Text.Json;
using VerseAppNew.Server.Services;
using static System.Net.WebRequestMethods;

namespace ScriptureMemory.Server.Services;

public class VerseOfDayService(
    VerseOfDayData _verseOfDayData,
    ILogger<VerseOfDayService> _logger,
    HttpClient _http,
    IDistributedCache _distributedCache,
    BibleApi _bibleApi,
    VerseCacherQueue _verseCacherQueue,
    IConfiguration _config)
{
    public async Task<Passage> GetVod(string translation)
    {
        Passage returnPassage = new();

        VerseOfDay existingVod = await _verseOfDayData.GetVod();
        Reference existingReference;

        if (existingVod is null)
        {
            existingReference = await InsertTodayVod();
            existingVod = await _verseOfDayData.GetVod();
        }
        else
        {
            existingReference = new Reference(existingVod.Reference);
        }

        List<Verse> cachedVerses = new List<Verse>();

        returnPassage.Verses = new();

        foreach (var verse in existingVod.PassageNavigation.Verses)
        {
            var cachedVerse = await _distributedCache.GetStringAsync(
                CacheKeyGenerator.GetVerseCacheKey(
                    existingVod.PassageId,
                    translation.ToLower().Trim(),
                    MemoryCacheType.PlainText));

            if (cachedVerse is not null)
            {
                returnPassage.Verses.Add(JsonSerializer.Deserialize<Verse>(
                    cachedVerse,
                    VectorJsonConverter.SerializerOptions)
                    ?? throw new Exception("Error deserializing cached verse"));

                _logger.LogInformation("Verse of day found in cache: {Id}:{Translation}.", verse.Id, translation);
            }
            else
            {
                if (verse.TranslationContents == null ||
                    string.IsNullOrEmpty(verse.TranslationContents.FirstOrDefault()?.PlainText))
                {
                    verse.TranslationContents = new();
                    verse.TranslationContents.Add(new VerseTranslationContent());
                    verse.TranslationContents.First().PlainText = await _bibleApi.GetVersePlaintext(AvailableBibles.GetBible(translation).Id, verse.Id);
                    verse.TranslationContents.First().Version = translation;
                }

                returnPassage.Id = existingVod.PassageId;
                returnPassage.Reference = existingReference;
                returnPassage.Verses.Add(new Verse()
                {
                    Id = verse.Id,
                    Reference = verse.Reference,
                    TranslationContents = new List<VerseTranslationContent>()
                    {
                        verse.TranslationContents.First()
                    },
                    SavedCount = verse.SavedCount,
                    MemorizedCount = verse.MemorizedCount,
                    PassageId = returnPassage.Id,
                    PassageNavigation = returnPassage
                });

                await _verseCacherQueue.EnqueueAsync(new Data.Models.CacheQueueItem()
                {
                    Verses = new List<Verse>() { verse },
                    Translation = translation.ToLower().Trim(),
                    CacheType = MemoryCacheType.PlainText
                });
            }
        }

        return returnPassage;
    }

    private record details(string text, string reference, string version);

    private class vodResponse
    {
        public verse Verse { get; set; }
    }

    private class verse
    {
        public details Details { get; set; }
    }

    public async Task<Reference> InsertTodayVod()
    {
        var results = await _http.GetFromJsonAsync<vodResponse>("https://beta.ourmanna.com/api/v1/get?format=json&order=daily");

        var reference = new Reference(results.Verse.Details.reference);

        var vod = new VerseOfDay
        {
            PassageId = reference.VerseId,
            Reference = reference.ReadableReference
        };

        await _verseOfDayData.InsertVod(vod);

        _logger.LogInformation("Successfully inserted vod: {Reference}", reference.VerseId);

        return reference;
    }
}
