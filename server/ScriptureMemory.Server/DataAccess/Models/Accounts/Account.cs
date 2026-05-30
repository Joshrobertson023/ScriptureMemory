using static ScriptureMemory.Server.Tools.Enums;

namespace ScriptureMemory.Server.DataAccess.Models;

public class Account
{
    [Key]
    public int UserId { get; set; }

    public string HashedPassword { get; set; }

    public UserRole? Role { get; set; }
    
    [InverseProperty(nameof(Session.Account))]
    public List<Session> Sessions { get; set; } = new(); // One-to-many
}
