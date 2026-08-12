using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ScriptureMemory.Server.Tools.Enums;

namespace DataAccess.Requests;

public sealed class SearchRequest
{
    [Required] public string Search { get; set; } = string.Empty;
    [Required] public string Translation { get; set; } = string.Empty;
}
