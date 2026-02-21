using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Requests.UpdateRequests;

public sealed class UpdateUsernameRequest
{
    [Required] public int UserId { get; set; }
    [Required] public string OldUsername { get; set; } = string.Empty;
    [Required] public string NewUsername { get; set;} = string.Empty;
}
