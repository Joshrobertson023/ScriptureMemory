using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VerseAppLibrary.Enums;

namespace DataAccess.Requests;

public sealed class SearchRequest
{
    [Required] public string Username { get; set; } = string.Empty;
    [Required] public string Search { get; set; } = string.Empty;
    [Required] public SearchType SearchType { get; set; }

    public SearchRequest(string username, string search, SearchType searchType)
    {
        Username = username.Trim();
        Search = search.Trim();
        SearchType = searchType;
    }
}