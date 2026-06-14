using System.ComponentModel;
using static ScriptureMemory.Server.Tools.Enums;

namespace ScriptureMemory.Server.DataAccess.Models;

public class Account
{
    [Key]
    public int UserId { get; set; }

    [MaxLength(100)]
    public string HashedPassword { get; set; }

    public UserRole? Role { get; set; }
    
    [InverseProperty(nameof(Session.Account))]
    public List<Session> Sessions { get; set; } = new(); // One-to-many
    
    [DefaultValue("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")]
    public DateTime DateCreated { get; set; } // Date user opened app for the first time
    
    public DateTime? DateRegistered { get; set; } // Date user created an account
}
