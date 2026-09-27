namespace ScriptureMemory.Server.Data.Models;

[PrimaryKey(nameof(FromVerseId), nameof(ToPassageId))]
public class CrossReference
{
    public Verse FromVerse { get; set; }
    public string FromVerseId { get; set; }
    public Passage ToPassage { get; set; }
    public string ToPassageId { get; set; }
    public int Votes { get; set; }
}
