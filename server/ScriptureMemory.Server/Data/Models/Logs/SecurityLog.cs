namespace ScriptureMemory.Server.Data.Models;

public class SecurityLog
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string? Ip { get; set; }
    public bool? Success { get; set; }
    public SecurityLogType Type { get; set; }
    public object? JsonContext { get; set; }
}