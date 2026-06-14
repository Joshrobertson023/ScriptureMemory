using System.ComponentModel.DataAnnotations;

namespace ScriptureMemory.Server.DataAccess.Requests;

public class GetVerseCountsRequest
{
    [Required] public int UserId { get; set; }
    [Required] public List<int> VerseIds { get; set; } = new();
}
