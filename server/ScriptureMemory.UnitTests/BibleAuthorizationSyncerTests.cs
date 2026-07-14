using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ScriptureMemory.Server;
using ScriptureMemory.Server.Data.DataAccess.Bible;
using ScriptureMemory.Server.DataAccess.Models;
using ScriptureMemory.Server.Services;
using ScriptureMemory.Server.Tools;

namespace ScriptureMemory.UnitTests;

public class BibleAuthorizationSyncerTests
{
    [Fact]
    public void EnsureReturnsBibles_ShouldReturnValidMergedBibles()
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

        (var merged, var needingLogged) = BibleHelper.MergeBiblesToSet(dbBibles, authorizedBibles);

        Assert.Single(merged, resultBible => resultBible.AbbreviationLocal == "F35");
        Assert.Single(merged, resultBible => resultBible.AbbreviationLocal == "RV");

        Assert.Single(needingLogged, resultBible => resultBible.AbbreviationLocal == "F35");
        
        Assert.False(merged.Single(b => b.AbbreviationLocal == "F35").Authorized);
        Assert.True(merged.Single(b => b.AbbreviationLocal == "F35").Active);
        Assert.True(merged.Single(b => b.AbbreviationLocal == "RV").Authorized);
        Assert.True(merged.Single(b => b.AbbreviationLocal == "RV").Active);
    }
}