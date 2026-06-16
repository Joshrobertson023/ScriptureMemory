using System.ComponentModel;
using static ScriptureMemory.Server.Tools.Enums;

namespace ScriptureMemory.Server.DataAccess.Models;

public class Account
{
    [Key]
    public int UserId { get; set; }

    public string HashedPassword { get; set; }

    public UserRole? Role { get; set; }
    
    public List<Session> Sessions { get; set; } = new();
    
    public DateTime DateCreated { get; set; } // Date user opened app for the first time
    
    public DateTime? DateRegistered { get; set; }
}
