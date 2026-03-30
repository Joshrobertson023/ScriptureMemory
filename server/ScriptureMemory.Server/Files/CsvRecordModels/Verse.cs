namespace ScriptureMemory.Server.Files.CsvRecordModels;

public sealed class Verse
{
    public int Id { get; set; }
    public string Book { get; set; } = string.Empty;
    public int Chapter { get; set; }
    public int VerseNum { get; set; }
    public string Text { get; set; } = string.Empty;
}
