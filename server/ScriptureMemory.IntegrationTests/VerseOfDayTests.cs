using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace ScriptureMemory.IntegrationTests;

public class VerseOfDayTests : BaseIntegrationTest
{
    public VerseOfDayTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task GetVerseOfDay_ReturnsVod()
    {
        var response = await Api.GetAsync("/verseofday");

        response.EnsureSuccessStatusCode();

        var verseOfDay = await response.Content.ReadFromJsonAsync<VerseOfDay>();
        Assert.NotNull(verseOfDay);
        Assert.False(string.IsNullOrEmpty(verseOfDay.Verses.First().Text));
        Assert.False(string.IsNullOrEmpty(verseOfDay.Reference));
    }
}
