using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Requests;

public sealed class GetLeaderboardRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
