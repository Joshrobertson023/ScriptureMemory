namespace ScriptureMemory.Server.Data.Models.Logs;

public class PracticeLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int Stage { get; set; }
    public float Accuracy { get; set; }
    public int SecondsOnStage { get; set; }
    public DateTime Timestamp { get; set; }
}