using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Requests.UpdateRequests;

public sealed class UpdateEmailRequest
{
    [Required] public string Username { get; set; } = string.Empty;
    [Required] public string NewEmail { get; set; } = string.Empty;
}
