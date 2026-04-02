using System.ComponentModel.DataAnnotations;
using static ScriptureMemory.Server.Tools.Enums;

namespace ScriptureMemory.Server.DataAccess.Requests;

public sealed class CreateAdminRequest
{
    [Required] public string AdminEmail { get; set; } = string.Empty;
    [Required] public AdminRole Role { get; set; }
}
