using System.ComponentModel.DataAnnotations;

namespace ScriptureMemory.Server.DataAccess.Requests;

public class GetVerseCardRequest
{
    [Required] public int UserId { get; set; }
    [Required] public int VerseId { get; set; }
}
