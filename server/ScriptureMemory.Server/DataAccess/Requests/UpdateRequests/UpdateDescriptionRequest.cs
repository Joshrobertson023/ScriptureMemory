using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Requests.UpdateRequests;

public sealed class UpdateDescriptionRequest
{
    [Required] public int UserId { get; set; }
    [Required] public string Description { get; set; } = string.Empty;
}
