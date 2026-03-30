namespace ScriptureMemory.Server.Files.CsvRecordModels;

public sealed class OracleVerses
{
    public int VERSE_ID { get; set; }
    public string VERSE_REFERENCE { get; set; } = string.Empty;
    public string VERSE_TEXT { get; set; } = string.Empty;
    public int USERS_SAVED_VERSE { get; set; }
    public int USERS_MEMORIZED { get; set; }
}
