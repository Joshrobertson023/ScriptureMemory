using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ScriptureMemoryLibrary.Enums;

namespace DataAccess.Models;

public sealed class Search
{
    public int Id { get; set; }
    public string SearchTerm { get; set; } = string.Empty;
    public DateTime SearchDate { get; set; }
    public SearchType SearchType { get; set; }
    public int SearchCount { get; set; }

    public Search(string searchTerm, SearchType searchType)
    {
        SearchTerm = searchTerm.Trim();
        SearchDate = DateTime.UtcNow;
        SearchType = searchType;
        SearchCount = 1;
    }
}
