using System.ComponentModel.DataAnnotations;

namespace ScriptureMemory.Server.DataAccess.Requests;

public class DeleteVodsRequest
{
    [Required] public List<int> Ids { get; set; } = new();
    [Required] public int AdminId { get; set; }
}
