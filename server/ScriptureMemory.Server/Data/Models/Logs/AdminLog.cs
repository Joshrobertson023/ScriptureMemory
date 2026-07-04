namespace ScriptureMemory.Server.Data.Models.Logs;

public class AdminLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public AdminLogType LogType { get; set; }
    public EntityType? EntityType { get; set; }
    public object? JsonContext { get; set; }
}