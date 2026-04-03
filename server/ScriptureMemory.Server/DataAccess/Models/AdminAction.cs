using static ScriptureMemory.Server.Tools.Enums;

namespace ScriptureMemory.Server.DataAccess.Models;

public class AdminAction
{
    public int Id { get; set; }
    public int AdminId { get; set; }
    public AdminActionType ActionType { get; set; }
    public DateTime Timestamp { get; set; }
    public object? JsonMetadata { get; set; }
}
