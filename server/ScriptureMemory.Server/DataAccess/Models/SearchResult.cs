using DataAccess.Models;
using ScriptureMemory.Server.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ScriptureMemory.Server.Tools.Enums;

namespace DataAccess.Models;

public sealed class SearchResult
{
    public int Rank { get; set; }

    public SearchResultType Type { get; set; }

    public Passage? Passage { get; set; }
    public Verse? Verse { get; set; }
    public double? Score { get; set; }
}
