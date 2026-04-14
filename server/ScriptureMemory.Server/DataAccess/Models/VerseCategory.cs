namespace ScriptureMemory.Server.DataAccess.Models;

public class VerseCategory
{
    public int VerseId { get; set; }
    public int CategoryId { get; set; }
    public string AssignmentSource { get; set; } = string.Empty;
    public float Confidence { get; set; }
}
