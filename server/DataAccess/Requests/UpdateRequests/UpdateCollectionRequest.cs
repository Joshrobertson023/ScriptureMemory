using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ScriptureMemoryLibrary.Enums;

namespace DataAccess.Requests.UpdateRequests;

public sealed class UpdateCollectionRequest
{
    [Required]
    public int Id { get; set; }
    public string? Title { get; set; }
    public CollectionVisibility? CollectionVisibility { get; set; }
    public string? Description { get; set; }
}
