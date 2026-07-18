using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.DataAccess.Models;
using ScriptureMemory.Server.Services;
using ScriptureMemory.Server.Tools;

namespace ScriptureMemory.IntegrationTests;

public class BibleSyncerTests : BaseIntegrationTest
{
    public BibleSyncerTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }
    
    [Fact]
    public async Task ShouldSyncBibles()
    { 
        List<Bible> dbBibles = new List<Bible>() 
        { 
            new Bible 
            { 
                Id = "40072c4a5aba4022-01", 
                Abbreviation = "engRV", 
                AbbreviationLocal = "RV", 
                Name = "Revised Version 1885", 
                NameLocal = "Revised Version 1885", 
                Copyright = null, 
                Info = "", 
                Active = true, 
                Authorized = true, 
                NextScheduledAutoSync = null 
            },
            new Bible 
            { 
                Id = "ec290b5045ff54a5-01", 
                Abbreviation = "engOKE", 
                AbbreviationLocal = "OKE", 
                Name = "Targum Onkelos Etheridge", 
                NameLocal = "Targum Onkelos Etheridge", 
                Copyright = null, 
                Info = "", 
                Active = true, 
                Authorized = true, 
                NextScheduledAutoSync = null 
            },
            new Bible 
            { 
                Id = "2f0fd81d7b85b923-01", 
                Abbreviation = "engF35", 
                AbbreviationLocal = "F35", 
                Name = "The English New Testament According to Family 35", 
                NameLocal = "The English New Testament According to Family 35", 
                Copyright = null, 
                Info = "", 
                Active = true, 
                Authorized = true, 
                NextScheduledAutoSync = null 
            }
        };

        List<Bible> authorizedBibles = new List<Bible>() 
        { 
            new Bible 
            { 
                Id = "40072c4a5aba4022-01", 
                Abbreviation = "engRV", 
                AbbreviationLocal = "RV", 
                Name = "Revised Version 1885", 
                NameLocal = "Revised Version 1885", 
                Copyright = null, 
                Info = "", 
                Active = false, 
                Authorized = false, 
                NextScheduledAutoSync = null 
            },
            new Bible 
            { 
                Id = "ec290b5045ff54a5-01", 
                Abbreviation = "engOKE", 
                AbbreviationLocal = "OKE", 
                Name = "Targum Onkelos Etheridge", 
                NameLocal = "Targum Onkelos Etheridge", 
                Copyright = null, 
                Info = "", 
                Active = false, 
                Authorized = false, 
                NextScheduledAutoSync = null 
            },
            new Bible 
            { 
                Id = "06125adad2d5898a-01", 
                Abbreviation = "ASV", 
                AbbreviationLocal = "ASV", 
                Name = "The Holy Bible, American Standard Version", 
                NameLocal = "The Holy Bible, American Standard Version", 
                Copyright = null, 
                Info = "", 
                Active = false, 
                Authorized = false, 
                NextScheduledAutoSync = null 
            }
        };

        var _bibleSyncer = _scope.ServiceProvider.GetRequiredService<BibleSyncer>();
        var _bibleContext = _scope.ServiceProvider.GetRequiredService<BibleData>();

        foreach (var bible in dbBibles)
        {
            await _bibleContext.InsertBible(bible);
        }

        await _bibleSyncer.SyncBibleAuthorization("", authorizedBibles);

        var resultingBiblesInDb = await _bibleContext.GetBibles();
        
        Assert.Single(resultingBiblesInDb, resultBible => resultBible.AbbreviationLocal == "F35");
        Assert.Single(resultingBiblesInDb, resultBible => resultBible.AbbreviationLocal == "RV");
        Assert.Single(resultingBiblesInDb, resultBible => resultBible.AbbreviationLocal == "OKE");
        
        // If new authorized Bible appears, add to db
        Assert.Single(resultingBiblesInDb, resultBible => resultBible.AbbreviationLocal == "ASV");
        
        // If unauthorized appears, start removal process
        Assert.False(resultingBiblesInDb.Single(b => b.AbbreviationLocal == "F35").Authorized);
        Assert.False(resultingBiblesInDb.Single(b => b.AbbreviationLocal == "F35").Active);
        
        Assert.False(resultingBiblesInDb.Single(b => b.AbbreviationLocal == "ASV").Active);
        Assert.True(resultingBiblesInDb.Single(b => b.AbbreviationLocal == "ASV").Authorized);
        Assert.True(resultingBiblesInDb.Single(b => b.AbbreviationLocal == "RV").Authorized);
        Assert.True(resultingBiblesInDb.Single(b => b.AbbreviationLocal == "RV").Active);
        Assert.True(resultingBiblesInDb.Single(b => b.AbbreviationLocal == "OKE").Authorized);
        Assert.True(resultingBiblesInDb.Single(b => b.AbbreviationLocal == "OKE").Active);
    }
}