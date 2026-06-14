using ScriptureMemory.Server.DataAccess.Models;
using static ScriptureMemory.Server.Tools.Enums;

namespace DataAccess.Models;

public class Admin : Account
{
    [MaxLength(50)]
    public string? AdminEmail { get; set; }
    
    [MaxLength(50)]
    public string? PersonalEmail { get; set; }
}