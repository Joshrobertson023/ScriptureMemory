using System.ComponentModel.DataAnnotations;

namespace ScriptureMemory.Server.DataAccess.Requests;

public class GetVerseCardRequest
{
    [Required] public int UserId { get; set; }
    [Required] public List<string> VerseIds { get; set; } = new();
    public bool FetchVerseText { get; set; }
    public string? Translation { get; set; }
}
