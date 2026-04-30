using System.ComponentModel.DataAnnotations;

namespace ScriptureMemory.Server.DataAccess.Requests.UpdateRequests;

public sealed class UpdateAdminPasswordRequest
{
    [Required] public int AdminId { get; set; }
    [Required] public string NewPassword { get; set; } = string.Empty;
}
