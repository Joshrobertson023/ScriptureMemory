using static ScriptureMemory.Server.Tools.Enums;

namespace DataAccess.Models;

public class Admin
{
    public int Id { get; set; }
    public string? HashedPassword { get; set; }
    public string? AdminEmail { get; set; }
    public string? PersonalEmail { get; set; }
    public UserRole? Role { get; set; }
}

