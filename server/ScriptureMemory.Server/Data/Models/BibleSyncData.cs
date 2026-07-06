namespace ScriptureMemory.Server.Data.Models;

[NotMapped]
public class BibleSyncData
{
    public Bible Bible { get; set; } = new();
    public bool Authorized { get; set; } = new();
    public bool InDatabase { get; set; } = new();
}