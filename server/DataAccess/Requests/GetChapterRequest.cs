using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Requests;

public sealed class GetChapterRequest
{
    [Required] public string Book { get; set; } = string.Empty;
    [Required] public int Chapter { get; set; }
}
