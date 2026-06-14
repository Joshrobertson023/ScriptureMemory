using System.ComponentModel.DataAnnotations;

namespace ScriptureMemory.Server.DataAccess.Requests;

public sealed class AdminLoginRequest
{
    [Required] public string Username { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
}
