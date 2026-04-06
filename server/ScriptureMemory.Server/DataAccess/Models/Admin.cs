using ScriptureMemory.Server.DataAccess.Models;
using static ScriptureMemory.Server.Tools.Enums;

namespace DataAccess.Models;

public class Admin : Account
{
    public int Id { get; set; }
    public string? AdminEmail { get; set; }
    public string? PersonalEmail { get; set; }
}