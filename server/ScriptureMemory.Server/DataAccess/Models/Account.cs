using static ScriptureMemory.Server.Tools.Enums;

namespace ScriptureMemory.Server.DataAccess.Models;

public class Account
{
    public string? HashedPassword { get; set; }
    public UserRole? Role { get; set; }
    public List<Session> Sessions { get; set; } = new();
}
