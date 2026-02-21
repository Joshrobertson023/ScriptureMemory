using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ScriptureMemoryLibrary.Enums;

namespace DataAccess.Requests;

public sealed class SearchRequest
{
    [Required] public int UserId { get; set; }
    [Required] public string Search { get; set; } = string.Empty;
    [Required] public SearchType SearchType { get; set; }

    public SearchRequest(int userId, string search, SearchType searchType)
    {
        UserId = userId;
        Search = search.Trim();
        SearchType = searchType;
    }
}
