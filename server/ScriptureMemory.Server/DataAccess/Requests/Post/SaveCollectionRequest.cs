using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Requests;

public sealed class SaveCollectionRequest
{
    [Required] public int UserId { get; set; }
    [Required] public int PublishedId { get; set; }
    public int? OrderPosition { get; set; }
    public DateTime DateSaved { get; set; } = DateTime.UtcNow;
}
